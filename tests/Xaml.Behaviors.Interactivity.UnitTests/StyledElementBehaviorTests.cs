using Avalonia.Controls;
using Avalonia.Headless.XUnit;
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

    private sealed class TestStyledElementBehavior : StyledElementBehavior;
}
