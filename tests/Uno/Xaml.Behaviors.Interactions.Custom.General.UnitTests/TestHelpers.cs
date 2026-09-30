// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Interactivity;

namespace Xaml.Interactions.Custom.General.UnitTests;

public sealed class TestViewModel : INotifyPropertyChanged
{
    private int _count;
    private bool _flag;
    private string? _name;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Count
    {
        get => _count;
        set { _count = value; OnPropertyChanged(); }
    }

    public bool Flag
    {
        get => _flag;
        set { _flag = value; OnPropertyChanged(); }
    }

    public string? Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RecordingCommand : ICommand
{
    public List<object?> Parameters { get; } = [];

    public bool CanExecuteResult { get; set; } = true;

    public event EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter) => CanExecuteResult;

    public void Execute(object? parameter) => Parameters.Add(parameter);
}

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public List<object?> Senders { get; } = [];

    public DependencyObject? ObservedHost => Host;

    public override object? Execute(object? sender, object? parameter)
    {
        Senders.Add(sender);
        Parameters.Add(parameter);
        return null;
    }
}

public partial class BindableRecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Values { get; } = [];

    [StyledProperty]
    public partial object? Value { get; set; }

    public override object? Execute(object? sender, object? parameter)
    {
        Values.Add(Value);
        return null;
    }
}

public partial class RecordingStyledAction : StyledElementAction
{
    public List<object?> Parameters { get; } = [];

    public DependencyObject? ObservedHost => Host;

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

/// <summary>
/// A minimal observable used to drive <see cref="ObservableTriggerBehavior{T}"/>.
/// </summary>
public sealed class TestObservable<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    public int SubscriberCount => _observers.Count;

    public void OnNext(T value)
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnNext(value);
        }
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(() => _observers.Remove(observer));
    }

    private sealed class Unsubscriber(System.Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}

internal static class TestInput
{
    public static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    public static void Click(Button button) => new ButtonAutomationPeer(button).Invoke();

    private static InputInjector? s_injector;

    /// <summary>
    /// Gets the shared injector (the injected mouse position is tracked per injector instance).
    /// </summary>
    public static InputInjector? CreateInjector() => s_injector ??= InputInjector.TryCreate();

    // Uno Platform injects mouse moves relative to the current position (the Absolute option is ignored).
    private static readonly ConditionalWeakTable<InputInjector, StrongBox<Point>> s_positions = new();

    public static async Task TapAsync(InputInjector injector, UIElement element, double x = 10, double y = 10)
    {
        MoveTo(injector, element.TransformToVisual(null).TransformPoint(new Point(x, y)));
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        await Session.WaitForIdleAsync();
    }

    public static async Task MoveAsync(InputInjector injector, UIElement? element, double x, double y)
    {
        MoveTo(injector, element is null ? new Point(x, y) : element.TransformToVisual(null).TransformPoint(new Point(x, y)));
        await Session.WaitForIdleAsync();
    }

    public static async Task LeftDownAsync(InputInjector injector)
    {
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        await Session.WaitForIdleAsync();
    }

    public static async Task LeftUpAsync(InputInjector injector)
    {
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
        await Session.WaitForIdleAsync();
    }

    private static void MoveTo(InputInjector injector, Point position)
    {
        var current = s_positions.GetValue(injector, static _ => new StrongBox<Point>(default));
        var deltaX = (int)Math.Round(position.X - current.Value.X);
        var deltaY = (int)Math.Round(position.Y - current.Value.Y);
        current.Value = new Point(current.Value.X + deltaX, current.Value.Y + deltaY);
        injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = deltaX,
            DeltaY = deltaY,
            MouseOptions = InjectedInputMouseOptions.Move,
        }]);
    }

    /// <summary>
    /// Raises the key events of a key press on an element (tunneling preview event, then the bubbling event).
    /// </summary>
    /// <remarks>
    /// The headless host has no keyboard input source (keyboard injection raises nothing), so the key events are
    /// raised through the Uno Platform input pipeline entry points (test only reflection).
    /// </remarks>
    public static KeyRoutedEventArgs RaiseKey(UIElement target, VirtualKey key, bool keyUp = false)
    {
        var args = (KeyRoutedEventArgs)Activator.CreateInstance(
            typeof(KeyRoutedEventArgs),
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            args: [target, key, VirtualKeyModifiers.None, null, null],
            culture: null)!;
        RaiseTunneling(target, keyUp ? UIElement.PreviewKeyUpEvent : UIElement.PreviewKeyDownEvent, args);
        RaiseBubbling(target, keyUp ? UIElement.KeyUpEvent : UIElement.KeyDownEvent, args);
        return args;
    }

    /// <summary>
    /// Presses and releases a key on an element, holding the given modifier keys.
    /// </summary>
    public static async Task PressKeyAsync(UIElement target, VirtualKey key, params VirtualKey[] modifiers)
    {
        foreach (var modifier in modifiers)
        {
            RaiseKey(target, modifier);
        }

        RaiseKey(target, key);
        RaiseKey(target, key, keyUp: true);

        for (var i = modifiers.Length - 1; i >= 0; i--)
        {
            RaiseKey(target, modifiers[i], keyUp: true);
        }

        await Session.WaitForIdleAsync();
    }

    /// <summary>
    /// Raises the character received (text input) event on an element.
    /// </summary>
    public static CharacterReceivedRoutedEventArgs RaiseCharacter(UIElement target, char character)
    {
        var args = (CharacterReceivedRoutedEventArgs)Activator.CreateInstance(
            typeof(CharacterReceivedRoutedEventArgs),
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            args: [target, character, default(Windows.UI.Core.CorePhysicalKeyStatus)],
            culture: null)!;
        RaiseBubbling(target, UIElement.CharacterReceivedEvent, args);
        return args;
    }

    private static void RaiseTunneling(UIElement target, RoutedEvent routedEvent, RoutedEventArgs args)
    {
        var method = typeof(UIElement).GetMethod("SafeRaiseTunnelingEvent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(target, [routedEvent, args]);
    }

    private static void RaiseBubbling(UIElement target, RoutedEvent routedEvent, RoutedEventArgs args)
    {
        var method = typeof(UIElement).GetMethod("SafeRaiseEvent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var context = Activator.CreateInstance(method.GetParameters()[2].ParameterType);
        method.Invoke(target, [routedEvent, args, context]);
    }

    public static async Task WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 3000)
    {
        var start = Environment.TickCount64;
        while (!condition())
        {
            if (Environment.TickCount64 - start > timeoutMilliseconds)
            {
                return;
            }

            await Task.Delay(20);
            await Session.WaitForIdleAsync();
        }
    }
}
