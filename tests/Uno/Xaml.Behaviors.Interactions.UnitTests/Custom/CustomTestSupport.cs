// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Windows.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.TestCompat;
using MouseButton = Xaml.Interactivity.MouseButton;

namespace Xaml.Interactions.UnitTests.Custom;

// Uno Platform counterparts of the shared test helpers and Avalonia test APIs used by the Custom tests. They live in the
// Custom namespace, so they take precedence over (and do not conflict with) Core/Command.cs, Core/ObservableCommand.cs,
// HeadlessWindowExtensions.cs or test compat types once those are shared too.

/// <summary>
/// The Avalonia <c>DispatcherPriority</c> values used by the Custom tests.
/// </summary>
internal enum DispatcherPriority
{
    Background,
}

/// <summary>
/// The Avalonia <c>Dispatcher.UIThread.InvokeAsync(action, priority)</c> overload used by the Custom tests.
/// </summary>
internal static class CustomDispatcherExtensions
{
    /// <summary>Queues <paramref name="action"/> on the UI thread (WinUI has no dispatcher priorities).</summary>
    public static System.Threading.Tasks.Task InvokeAsync(
        this Xaml.Interactivity.UIThreadDispatcher dispatcher,
        System.Action action,
        DispatcherPriority priority)
        => dispatcher.InvokeAsync(action);
}

/// <summary>
/// A delegate command (Uno Platform twin of <c>Core/Command.cs</c>).
/// </summary>
internal class Command(Action<object?> execute, Func<object?, bool>? canExecute = null)
    : ICommand
{
    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => execute.Invoke(parameter);

#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
}

/// <summary>
/// A command that records its <see cref="ICommand.CanExecuteChanged"/> subscriptions (Uno Platform twin of
/// <c>Core/ObservableCommand.cs</c>).
/// </summary>
internal sealed class ObservableCommand : ICommand
{
    private EventHandler? _canExecuteChanged;

    public bool CanExecuteResult { get; set; } = true;

    public Func<object?, bool>? CanExecuteCallback { get; set; }

    public int SubscriptionCount { get; private set; }

    public object? LastCanExecuteParameter { get; private set; }

    public int CanExecuteCallCount { get; private set; }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            _canExecuteChanged += value;
            SubscriptionCount++;
        }
        remove
        {
            _canExecuteChanged -= value;
            SubscriptionCount--;
        }
    }

    public bool CanExecute(object? parameter)
    {
        CanExecuteCallCount++;
        LastCanExecuteParameter = parameter;
        return CanExecuteCallback?.Invoke(parameter) ?? CanExecuteResult;
    }

    public void Execute(object? parameter)
    {
    }

    public void RaiseCanExecuteChanged() => _canExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// The Avalonia <c>KeyboardNavigation.SetTabNavigation</c> attached property setter, mapped to
/// <see cref="UIElement.TabFocusNavigation"/>.
/// </summary>
internal static class KeyboardNavigation
{
    public static void SetTabNavigation(UIElement element, KeyboardNavigationMode mode) => element.TabFocusNavigation = mode;
}

/// <summary>
/// The Avalonia <c>TopLevel.FocusManager</c> of the test window.
/// </summary>
internal static class CustomFocusExtensions
{
    extension(HeadlessTestWindow window)
    {
        /// <summary>Gets the focus manager of the window's XAML root.</summary>
        public TestFocusManager? FocusManager => window.XamlRoot is { } xamlRoot ? new TestFocusManager(xamlRoot) : null;
    }
}

/// <summary>
/// The Avalonia <c>IFocusManager.GetFocusedElement</c> of a XAML root.
/// </summary>
internal sealed class TestFocusManager(XamlRoot xamlRoot)
{
    public object? GetFocusedElement() => Microsoft.UI.Xaml.Input.FocusManager.GetFocusedElement(xamlRoot);
}

/// <summary>
/// The Avalonia <c>Brushes</c> used by the Custom tests.
/// </summary>
internal static class Brushes
{
    public static SolidColorBrush Red => new(Colors.Red);

    public static SolidColorBrush Blue => new(Colors.Blue);

    public static SolidColorBrush Transparent => new(Colors.Transparent);
}

/// <summary>
/// Mouse input relative to an element (Uno Platform twin of <c>HeadlessWindowExtensions.cs</c>) through
/// <see cref="UnoHeadlessSession.Mouse"/>; the <see cref="RawInputModifiers.Control"/> modifier is held with
/// <see cref="UnoHeadlessSession.Keyboard"/>.
/// </summary>
internal static class CustomHeadlessWindowExtensions
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    /// <summary>Clicks the center of <paramref name="relativeTo"/>.</summary>
    public static void Click(
        this UIElement topLevel,
        FrameworkElement relativeTo,
        MouseButton button = MouseButton.Left,
        RawInputModifiers modifiers = RawInputModifiers.None)
    {
        // Like the Avalonia headless platform, input uses the current layout.
        relativeTo.UpdateLayout();
        var point = new Point(relativeTo.ActualWidth / 2, relativeTo.ActualHeight / 2);
        topLevel.MouseDown(relativeTo, point, button, modifiers);
        topLevel.MouseUp(relativeTo, point, button, modifiers);
    }

    /// <summary>Presses a mouse button at a position relative to <paramref name="relativeTo"/>.</summary>
    public static void MouseDown(
        this UIElement topLevel,
        FrameworkElement relativeTo,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers = RawInputModifiers.None)
        => WithModifiers(modifiers, () => relativeTo.MouseDown(point, button, modifiers));

    /// <summary>Moves the mouse to a position relative to <paramref name="relativeTo"/>.</summary>
    public static void MouseMove(
        this UIElement topLevel,
        FrameworkElement relativeTo,
        Point point,
        RawInputModifiers modifiers = RawInputModifiers.None)
        => relativeTo.MouseMove(point, modifiers);

    /// <summary>Releases a mouse button at a position relative to <paramref name="relativeTo"/>.</summary>
    public static void MouseUp(
        this UIElement topLevel,
        FrameworkElement relativeTo,
        Point point,
        MouseButton button,
        RawInputModifiers modifiers = RawInputModifiers.None)
        => WithModifiers(modifiers, () => relativeTo.MouseUp(point, button, modifiers));

    private static void WithModifiers(RawInputModifiers modifiers, System.Action action)
    {
        if (!modifiers.HasFlag(RawInputModifiers.Control))
        {
            action();
            return;
        }

        Session.Keyboard.KeyDown(VirtualKey.Control, VirtualKeyModifiers.Control);
        try
        {
            action();
        }
        finally
        {
            Session.Keyboard.KeyUp(VirtualKey.Control);
            Session.RunJobs();
        }
    }
}
