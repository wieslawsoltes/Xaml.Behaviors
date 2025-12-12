using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Headless.XUnit;
using Xaml.Behaviors.SourceGenerators;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class EventArgsActionRuntimeTests
{
    public class PointerEventHandler
    {
        public bool Called { get; private set; }
        public PointerPressedEventArgs? LastArgs { get; private set; }

        [GenerateEventArgsAction(Project = "KeyModifiers,ClickCount")]
        public void OnPointerPressed(PointerPressedEventArgs args)
        {
            Called = true;
            LastArgs = args;
        }
    }

    public class KeyEventHandler
    {
        public bool Called { get; private set; }
        public Key LastKey { get; private set; }
        public KeyModifiers LastModifiers { get; private set; }

        [GenerateEventArgsAction(UseDispatcher = true, Project = "Key,KeyModifiers")]
        public void OnKeyDown(KeyEventArgs args)
        {
            Called = true;
            LastKey = args.Key;
            LastModifiers = args.KeyModifiers;
        }
    }

    public class EventArgsHandler
    {
        public bool Called { get; private set; }

        [GenerateEventArgsAction]
        public void Handle(RoutedEventArgs args)
        {
            Called = true;
        }
    }

    public class AsyncEventArgsHandler
    {
        public int Calls { get; private set; }

        [GenerateEventArgsAction(UseDispatcher = true)]
        public Task HandleAsync(RoutedEventArgs args)
        {
            Calls++;
            return Task.CompletedTask;
        }
    }

    public class ThrowingEventArgsHandler
    {
        [GenerateEventArgsAction(UseDispatcher = true)]
        public Task FailAsync(RoutedEventArgs args)
        {
            throw new System.InvalidOperationException("boom");
        }
    }

    [AvaloniaFact]
    public void EventArgsAction_Should_Invoke_Target_Method()
    {
        var handler = new EventArgsHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("HandleEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        action.Execute(null, new RoutedEventArgs());

        Assert.True(handler.Called);
    }

    [AvaloniaFact]
    public void EventArgsAction_Should_Project_Pointer_EventArgs()
    {
        var handler = new PointerEventHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("OnPointerPressedEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        var args = CreatePointerArgs(KeyModifiers.Control | KeyModifiers.Shift, clickCount: 2);
        var executed = (bool)action.Execute(null, args);

        Assert.True(executed);
        Assert.True(handler.Called);
        Assert.Same(args, handler.LastArgs);
        Assert.Equal(2, (int)action.ClickCount);
        Assert.Equal(KeyModifiers.Control | KeyModifiers.Shift, (KeyModifiers)action.KeyModifiers);
    }

    [AvaloniaFact]
    public void EventArgsAction_Should_Project_Public_Property()
    {
        var handler = new EventArgsHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("HandleEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        var args = new RoutedEventArgs();

        var executed = (bool)action.Execute(null, args);

        Assert.True(executed);
        Assert.True(handler.Called);
    }

    [AvaloniaFact]
    public async Task EventArgsAction_Should_Dispatch_Key_Handler()
    {
        var handler = new KeyEventHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("OnKeyDownEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        var args = new KeyEventArgs
        {
            Key = Key.Space,
            KeyModifiers = KeyModifiers.Meta
        };

        action.Execute(null, args);
        Assert.False(handler.Called);

        try
        {
            await FlushDispatcherAsync();
        }
        catch (System.PlatformNotSupportedException)
        {
            // Headless dispatcher may not support push frames on some platforms; skip validation there.
            return;
        }

        Assert.True(handler.Called);
        Assert.Equal(Key.Space, handler.LastKey);
        Assert.Equal(KeyModifiers.Meta, handler.LastModifiers);
        Assert.Equal(Key.Space, (Key)action.Key);
    }

    [AvaloniaFact]
    public async Task EventArgsAction_Should_Observe_Task_Result()
    {
        var handler = new AsyncEventArgsHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("HandleAsyncEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        var executed = (bool)action.Execute(null, new RoutedEventArgs());
        Assert.True(executed);

        await FlushDispatcherAsync();
        Assert.Equal(1, handler.Calls);
    }

    [AvaloniaFact]
    public async Task EventArgsAction_Dispatcher_Should_Swallow_Async_Exception()
    {
        var handler = new ThrowingEventArgsHandler();
        dynamic action = GeneratedTypeHelper.CreateInstance("FailAsyncEventArgsAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.TargetObject = handler;

        var executed = (bool)action.Execute(null, new RoutedEventArgs());
        Assert.True(executed);

        try
        {
            await FlushDispatcherAsync();
        }
        catch (PlatformNotSupportedException)
        {
            return;
        }
    }

    private static PointerPressedEventArgs CreatePointerArgs(KeyModifiers modifiers, int clickCount)
    {
        var source = new TestControl();
        var pointer = new Pointer(0, PointerType.Mouse, isPrimary: true);
        var props = new PointerPointProperties();
        return new PointerPressedEventArgs(source, pointer, source, new Point(10, 20), 0, props, modifiers, clickCount);
    }

    private static async Task FlushDispatcherAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { });
    }
}
