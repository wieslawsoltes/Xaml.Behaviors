using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

public class StyledElementBehaviorTests
{
    [AvaloniaFact]
    public void Detach_ClearsLogicalParentAndTemplatedParent()
    {
        var behavior = new TestStyledElementBehavior();
        var button = new Button();
#if UNO
        // WinUI has no logical tree and no settable templated parent: the behavior joins and leaves the tree of its
        // associated object.
        var window = new Window
        {
            Content = button
        };

        Interaction.GetBehaviors(button).Add(behavior);
        window.Show();

        Assert.Equal(button, behavior.AssociatedObject);
        Assert.True(((ILogical)behavior).IsAttachedToLogicalTree);

        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
        Assert.False(((ILogical)behavior).IsAttachedToLogicalTree);
#else
        var templatedParent = new ContentControl();
        var window = new Window
        {
            Content = button
        };

        TemplatedParentHelper.SetTemplatedParent(button, templatedParent);
        Interaction.GetBehaviors(button).Add(behavior);
        window.Show();

        Assert.Equal(button, behavior.Parent);
        Assert.Equal(templatedParent, behavior.TemplatedParent);

        behavior.Detach();

        Assert.Null(behavior.Parent);
        Assert.Null(behavior.TemplatedParent);
#endif

        window.Close();
    }

#if !UNO
    // Avalonia behaviors join their logical parent (and templated parent) on visual attach and leave it on visual
    // detach; WinUI has no logical tree and the Uno StyledElementBehavior follows the logical phase only.
    [AvaloniaFact]
    public void DetachVisualTree_ClearsLogicalScopeWhenCallbackThrows()
    {
        var behavior = new ThrowingDetachBehavior();
        var button = new Button();
        var templatedParent = new ContentControl();
        var window = new Window { Content = button };

        TemplatedParentHelper.SetTemplatedParent(button, templatedParent);
        Interaction.GetBehaviors(button).Add(behavior);
        window.Show();

        Assert.Throws<InvalidOperationException>(() =>
            ((IBehaviorEventsHandler)behavior).DetachedFromVisualTreeEventHandler());

        Assert.Null(behavior.Parent);
        Assert.Null(behavior.TemplatedParent);

        behavior.Detach();
        Interaction.SetBehaviors(button, null);
        window.Close();
    }

    // WinUI has no TopLevel: the deferred detach of top level behaviors on close is Avalonia specific.
    [AvaloniaFact]
    public void TopLevelClose_PreservesLogicalParentUntilDeferredDetach()
    {
        var behavior = new TestStyledElementBehavior();
        var window = new Window();
        Interaction.GetBehaviors(window).Add(behavior);

        window.Show();

        Assert.Same(window, behavior.AssociatedObject);
        Assert.Same(window, behavior.Parent);

        window.Close();

        Assert.Same(window, behavior.AssociatedObject);
        Assert.Same(window, behavior.Parent);

        Dispatcher.UIThread.RunJobs();

        Assert.Null(behavior.AssociatedObject);
        Assert.Null(behavior.Parent);
    }
#endif

    private sealed class TestStyledElementBehavior : StyledElementBehavior;

    private sealed class ThrowingDetachBehavior : StyledElementBehavior
    {
        protected override void OnDetachedFromVisualTree()
        {
            throw new InvalidOperationException("Expected test exception.");
        }
    }
}
