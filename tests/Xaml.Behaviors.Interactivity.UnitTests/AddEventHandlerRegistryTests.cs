using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

public class AddEventHandlerRegistryTests
{
    private class TestControl
    {
        public event EventHandler<EventArgs>? CustomEvent;
        public void Raise() => CustomEvent?.Invoke(this, EventArgs.Empty);
    }

    [AvaloniaFact]
    public void TryRegisterEventHandler_ButtonClick_ReturnsDisposable()
    {
        var button = new Button();
        var called = false;

        var disposable = AddEventHandlerRegistry.TryRegisterEventHandler(button, nameof(Button.Click), (_, _) => called = true);

        Assert.NotNull(disposable);

#if UNO
        new ButtonAutomationPeer(button).Invoke();
#else
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif
        Assert.True(called);

        called = false;
        disposable!.Dispose();
#if UNO
        new ButtonAutomationPeer(button).Invoke();
#else
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif
        Assert.False(called);
    }

    [AvaloniaFact]
    public void TryRegisterEventHandler_NoMatch_ReturnsNull()
    {
        var button = new Button();
        var disposable = AddEventHandlerRegistry.TryRegisterEventHandler(button, "NoEvent", (_, _) => { });
        Assert.Null(disposable);
    }

    [AvaloniaFact]
    public void Register_Unregister_CustomHandler()
    {
        var control = new TestControl();
        bool called = false;

        var handler = new FuncAddEventHandler<TestControl, EventArgs>(
            nameof(TestControl.CustomEvent),
            (o, h) => o.CustomEvent += h,
            (o, h) => o.CustomEvent -= h);

        AddEventHandlerRegistry.Register(handler);
        try
        {
            var disposable = AddEventHandlerRegistry.TryRegisterEventHandler(control, nameof(TestControl.CustomEvent), (_, _) => called = true);
            Assert.NotNull(disposable);

            control.Raise();
            Assert.True(called);

            disposable!.Dispose();
            called = false;
            control.Raise();
            Assert.False(called);
        }
        finally
        {
            AddEventHandlerRegistry.Unregister(handler);
        }

        var disposable2 = AddEventHandlerRegistry.TryRegisterEventHandler(control, nameof(TestControl.CustomEvent), (_, _) => { });
        Assert.Null(disposable2);
    }
}
