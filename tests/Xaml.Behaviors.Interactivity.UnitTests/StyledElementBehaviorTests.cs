using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class StyledElementBehaviorTests
{
    [AvaloniaFact]
    public void Detach_ClearsLogicalParentAndTemplatedParent()
    {
        var behavior = new TestStyledElementBehavior();
        var button = new Button();
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

        window.Close();
    }

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

    private sealed class TestStyledElementBehavior : StyledElementBehavior;
}
