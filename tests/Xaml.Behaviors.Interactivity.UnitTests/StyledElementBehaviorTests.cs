using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StyledElementBehaviorTests
{
    [AvaloniaFact]
    public void Attach_WithNull_Throws()
    {
        var behavior = new StubStyledElementBehavior();

        Assert.Throws<ArgumentNullException>(() => behavior.Attach(null));
        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(0, behavior.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_SetsAssociatedObject_AndCallsOnAttached()
    {
        var behavior = new StubStyledElementBehavior();
        var textBlock = new TextBlock();

        behavior.Attach(textBlock);

        Assert.Equal(textBlock, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_SameObjectTwice_CallsOnce()
    {
        var behavior = new StubStyledElementBehavior();
        var textBlock = new TextBlock();

        behavior.Attach(textBlock);
        behavior.Attach(textBlock);

        Assert.Equal(textBlock, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachCount);
    }

    [AvaloniaFact]
    public void Attach_DifferentObjects_Throws()
    {
        var behavior = new StubStyledElementBehavior();
        var first = new TextBlock();
        var second = new TextBlock();

        behavior.Attach(first);
        Assert.Throws<InvalidOperationException>(() => behavior.Attach(second));
        Assert.Equal(first, behavior.AssociatedObject);
        Assert.Equal(1, behavior.AttachCount);
    }

    [AvaloniaFact]
    public void Detach_ClearsAssociatedObject_AndCallsOnDetaching()
    {
        var behavior = new StubStyledElementBehavior();
        var textBlock = new TextBlock();

        behavior.Attach(textBlock);
        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal(1, behavior.DetachCount);
    }

    [AvaloniaFact]
    public void DataContext_IsSynchronized()
    {
        var behavior = new StubStyledElementBehavior();
        var textBlock = new TextBlock { DataContext = "initial" };

        behavior.Attach(textBlock);
        Assert.Equal("initial", behavior.DataContext);

        textBlock.DataContext = "updated";
        Assert.Equal("updated", behavior.DataContext);
    }

    [AvaloniaFact]
    public void BehaviorEvents_InvokeCorrespondingMethods()
    {
        var behavior = new StubStyledElementBehavior();
        var textBlock = new TextBlock();

        behavior.Attach(textBlock);
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

        Assert.Equal(1, behavior.AttachedToVisualTreeCount);
        Assert.Equal(1, behavior.DetachedFromVisualTreeCount);
        Assert.Equal(1, behavior.AttachedToLogicalTreeCount);
        Assert.Equal(1, behavior.DetachedFromLogicalTreeCount);
        Assert.Equal(1, behavior.LoadedCount);
        Assert.Equal(1, behavior.UnloadedCount);
        Assert.Equal(1, behavior.InitializedCount);
        Assert.Equal(1, behavior.DataContextChangedCount);
        Assert.Equal(1, behavior.ResourcesChangedCount);
        Assert.Equal(1, behavior.ActualThemeVariantChangedCount);
    }
}

