using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class BehaviorOfTTests
{
    [AvaloniaFact]
    public void Attach_Correct_Type_Sets_Typed_AssociatedObject()
    {
        var behavior = new TestBehaviorOfT();
        var button = new Button();

        behavior.Attach(button);

        Assert.Equal(button, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachedCalled);
    }

    [AvaloniaFact]
    public void Attach_Wrong_Type_Throws()
    {
        var behavior = new TestBehaviorOfT();
        var textBlock = new TextBlock();

        Assert.Throws<InvalidOperationException>(() => behavior.Attach(textBlock));
    }

    [AvaloniaFact]
    public void Detach_After_Attach_Clears_AssociatedObject()
    {
        var behavior = new TestBehaviorOfT();
        var button = new Button();

        behavior.Attach(button);
        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(1, behavior.DetachingCalled);
    }
}

