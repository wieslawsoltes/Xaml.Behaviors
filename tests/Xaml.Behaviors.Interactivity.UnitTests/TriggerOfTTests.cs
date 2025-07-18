using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class TriggerOfTTests
{
    private class ButtonTrigger : StyledElementTrigger<Button>
    {
    }

    [AvaloniaFact]
    public void Attach_WrongType_Throws()
    {
        var trigger = new ButtonTrigger();

        Assert.Throws<InvalidOperationException>(() => trigger.Attach(new TextBox()));
    }

    [AvaloniaFact]
    public void Attach_CorrectType_SetsAssociatedObject()
    {
        var trigger = new ButtonTrigger();
        var button = new Button();

        trigger.Attach(button);

        Assert.Equal(button, trigger.AssociatedObject);
    }
}
