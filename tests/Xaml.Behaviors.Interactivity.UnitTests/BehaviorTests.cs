using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class BehaviorTests
{
    [AvaloniaFact]
    public void Attach_Sets_AssociatedObject()
    {
        var behavior = new TestBehavior();
        var button = new Button();

        behavior.Attach(button);

        Assert.Equal(button, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachedCalled);
    }

    [AvaloniaFact]
    public void Attach_Same_Object_No_Op()
    {
        var behavior = new TestBehavior();
        var button = new Button();

        behavior.Attach(button);
        behavior.Attach(button);

        Assert.Equal(1, behavior.AttachedCalled);
    }

    [AvaloniaFact]
    public void Attach_Null_Does_Nothing()
    {
        var behavior = new TestBehavior();

        behavior.Attach(null);

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(0, behavior.AttachedCalled);
    }

    [AvaloniaFact]
    public void Attach_Different_Object_Throws()
    {
        var behavior = new TestBehavior();
        behavior.Attach(new Button());

        Assert.Throws<InvalidOperationException>(() => behavior.Attach(new Button()));
    }

    [AvaloniaFact]
    public void Detach_Clears_AssociatedObject()
    {
        var behavior = new TestBehavior();
        var button = new Button();

        behavior.Attach(button);
        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(1, behavior.DetachingCalled);
    }

    [AvaloniaFact]
    public void Behavior_Event_Methods_Invoke_Overrides()
    {
        var behavior = new TestBehavior();
        var handler = (IBehaviorEventsHandler)behavior;

        handler.AttachedToVisualTreeEventHandler();
        handler.DetachedFromVisualTreeEventHandler();
        handler.AttachedToLogicalTreeEventHandler();
        handler.DetachedFromLogicalTreeEventHandler();
        handler.LoadedEventHandler();
        handler.UnloadedEventHandler();
        handler.InitializedEventHandler();
        handler.DataContextChangedEventHandler();
        handler.ResourcesChangedEventHandler();
        handler.ActualThemeVariantChangedEventHandler();

        Assert.Equal(1, behavior.VisualAttachCalled);
        Assert.Equal(1, behavior.VisualDetachCalled);
        Assert.Equal(1, behavior.LogicalAttachCalled);
        Assert.Equal(1, behavior.LogicalDetachCalled);
        Assert.Equal(1, behavior.LoadedCalled);
        Assert.Equal(1, behavior.UnloadedCalled);
        Assert.Equal(1, behavior.InitializedCalled);
        Assert.Equal(1, behavior.DataContextChangedCalled);
        Assert.Equal(1, behavior.ResourcesChangedCalled);
        Assert.Equal(1, behavior.ThemeChangedCalled);
    }
}

