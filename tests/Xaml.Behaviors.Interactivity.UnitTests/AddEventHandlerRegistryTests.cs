using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

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

        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.True(called);

        called = false;
        disposable!.Dispose();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
