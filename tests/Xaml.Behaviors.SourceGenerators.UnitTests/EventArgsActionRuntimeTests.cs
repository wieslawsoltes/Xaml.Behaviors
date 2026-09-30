using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Xaml.Behaviors.Uno.Headless;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Headless.XUnit;
#endif
using Xaml.Behaviors.SourceGenerators;
using Xunit;

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

public class EventArgsActionRuntimeTests
{
    public class PointerEventHandler
    {
        public bool Called { get; private set; }
        public PointerPressedEventArgs? LastArgs { get; private set; }

#if UNO
        // WinUI pointer arguments have no click count: project the pointer instead.
        [GenerateEventArgsAction(Project = "KeyModifiers,Pointer")]
#else
        [GenerateEventArgsAction(Project = "KeyModifiers,ClickCount")]
#endif
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

#if UNO
        // WinUI key arguments carry no modifiers.
        [GenerateEventArgsAction(UseDispatcher = true, Project = "Key")]
#else
        [GenerateEventArgsAction(UseDispatcher = true, Project = "Key,KeyModifiers")]
#endif
        public void OnKeyDown(KeyEventArgs args)
        {
            Called = true;
            LastKey = args.Key;
#if !UNO
            LastModifiers = args.KeyModifiers;
#endif
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

#if UNO
        // WinUI pointer arguments are created by the input system: execute the action for a real pointer press.
        var element = new Border { Width = 40, Height = 40, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        PointerPressedEventArgs? args = null;
        var executed = false;
        element.PointerPressed += (_, e) =>
        {
            args = e;
            executed = (bool)action.Execute(null, e);
        };
        UnoHeadlessSession.Current.Show(element);
        UnoHeadlessSession.Current.Mouse.Click(element);

        Assert.NotNull(args);
        Assert.True(executed);
        Assert.True(handler.Called);
        Assert.Same(args, handler.LastArgs);
        Assert.Same(args.Pointer, action.Pointer);
        Assert.Equal(args.KeyModifiers, (KeyModifiers)action.KeyModifiers);
#else
        var args = CreatePointerArgs(KeyModifiers.Control | KeyModifiers.Shift, clickCount: 2);
        var executed = (bool)action.Execute(null, args);

        Assert.True(executed);
        Assert.True(handler.Called);
        Assert.Same(args, handler.LastArgs);
        Assert.Equal(2, (int)action.ClickCount);
        Assert.Equal(KeyModifiers.Control | KeyModifiers.Shift, (KeyModifiers)action.KeyModifiers);
#endif
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

#if UNO
        // WinUI key arguments are created by the input system: execute the action for a real key press.
        var element = new Button();
        var executed = 0;
        element.AddHandler(
            UIElement.KeyDownEvent,
            new Microsoft.UI.Xaml.Input.KeyEventHandler((_, e) =>
            {
                action.Execute(null, e);
                executed++;
                Assert.False(handler.Called);
            }),
            handledEventsToo: true);
        UnoHeadlessSession.Current.Show(element);
        Assert.True(element.Focus(FocusState.Programmatic));
        UnoHeadlessSession.Current.Keyboard.KeyDown(Key.Space);
        Assert.Equal(1, executed);
#else
        var args = new KeyEventArgs
        {
            Key = Key.Space,
            KeyModifiers = KeyModifiers.Meta
        };

        action.Execute(null, args);
        Assert.False(handler.Called);
#endif

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
#if !UNO
        Assert.Equal(KeyModifiers.Meta, handler.LastModifiers);
#endif
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

#if !UNO
    private static PointerPressedEventArgs CreatePointerArgs(KeyModifiers modifiers, int clickCount)
    {
        var source = new TestControl();
        var pointer = new Pointer(0, PointerType.Mouse, isPrimary: true);
        var props = new PointerPointProperties();
        return new PointerPressedEventArgs(source, pointer, source, new Point(10, 20), 0, props, modifiers, clickCount);
    }

#endif

    private static async Task FlushDispatcherAsync()
    {
#if UNO
        // Runs the work queued before this call (the test compat InvokeAsync runs inline on the UI thread).
        await UnoHeadlessSession.Current.WaitForIdleAsync();
#else
        await Dispatcher.UIThread.InvokeAsync(() => { });
#endif
    }
}
