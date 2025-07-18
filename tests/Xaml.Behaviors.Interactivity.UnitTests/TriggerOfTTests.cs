using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class TriggerOfTTests
{
    [AvaloniaFact]
    public void Attach_CorrectType_AssociatedObjectSet()
    {
        var trigger = new StubTrigger<Button>();
        var button = new Button();
        trigger.Attach(button);

        Assert.Equal(button, trigger.AssociatedObject);
        Assert.Equal(1, trigger.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_DerivedType_AssociatedObjectSet()
    {
        var trigger = new StubTrigger<Button>();
        var toggle = new ToggleButton();
        trigger.Attach(toggle);

        Assert.Equal(toggle, trigger.AssociatedObject);
        Assert.Equal(1, trigger.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_WrongType_Throws()
    {
        var trigger = new StubTrigger<Button>();
        var textBlock = new TextBlock();

        TestUtilities.AssertThrowsInvalidOperationException(() => trigger.Attach(textBlock));
    }

    [AvaloniaFact]
    public void Detach_ClearsAssociatedObject()
    {
        var trigger = new StubTrigger<Button>();
        var button = new Button();
        trigger.Attach(button);
        trigger.Detach();

        Assert.Null(trigger.AssociatedObject);
        Assert.Equal(1, trigger.DetachCount);
    }
}
