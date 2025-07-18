using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class TriggerTests
{
    [AvaloniaFact]
    public void Actions_Default_NotNull()
    {
        var trigger = new StubTrigger();
        Assert.NotNull(trigger.Actions);
        Assert.Empty(trigger.Actions);
    }

    [AvaloniaFact]
    public void Actions_MultipleCalls_ReturnSameInstance()
    {
        var trigger = new StubTrigger();
        var first = trigger.Actions;
        var second = trigger.Actions;
        Assert.Same(first, second);
    }

    [AvaloniaFact]
    public void Attach_SetsAssociatedObject()
    {
        var trigger = new StubTrigger();
        var button = new Button();
        trigger.Attach(button);

        Assert.Equal(button, trigger.AssociatedObject);
        Assert.Equal(1, trigger.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_SameObjectTwice_DoesNotReattach()
    {
        var trigger = new StubTrigger();
        var button = new Button();
        trigger.Attach(button);
        trigger.Attach(button);

        Assert.Equal(1, trigger.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_DifferentObject_Throws()
    {
        var trigger = new StubTrigger();
        trigger.Attach(new Button());

        TestUtilities.AssertThrowsInvalidOperationException(() => trigger.Attach(new Button()));
    }

    [AvaloniaFact]
    public void Attach_Null_Throws()
    {
        var trigger = new StubTrigger();
        Assert.Throws<ArgumentNullException>(() => trigger.Attach(null));
    }

    [AvaloniaFact]
    public void Detach_ClearsAssociatedObject()
    {
        var trigger = new StubTrigger();
        var button = new Button();
        trigger.Attach(button);
        trigger.Detach();

        Assert.Null(trigger.AssociatedObject);
        Assert.Equal(1, trigger.DetachCount);
    }
}
