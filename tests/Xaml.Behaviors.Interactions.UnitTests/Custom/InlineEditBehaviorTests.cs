using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class InlineEditBehaviorTests
{
    [AvaloniaFact]
    public void DoubleTapped_DisplayControl_Begins_Edit()
    {
        var displayControl = new Border { Width = 100, Height = 30, Background = Brushes.Transparent };
        var editControl = new TextBox { Width = 100, Height = 30 };
        var host = new StackPanel
        {
            Children =
            {
                displayControl,
                editControl
            }
        };
        Interaction.GetBehaviors(host).Add(new InlineEditBehavior
        {
            DisplayControl = displayControl,
            EditControl = editControl
        });
        var window = new Window { Content = host };

        window.Show();
        window.Click(displayControl);
        window.Click(displayControl);

        Assert.False(displayControl.IsVisible);
        Assert.True(editControl.IsVisible);
    }

    [AvaloniaFact]
    public void DoubleTapped_AssociatedObject_Begins_Edit()
    {
        var activationTarget = new Border { Width = 100, Height = 30, Background = Brushes.Transparent };
        var displayControl = new Border { Width = 100, Height = 30, Background = Brushes.Transparent };
        var editControl = new TextBox { Width = 100, Height = 30 };
        var host = new StackPanel
        {
            Children =
            {
                activationTarget,
                displayControl,
                editControl
            }
        };
        Interaction.GetBehaviors(host).Add(new InlineEditBehavior
        {
            DisplayControl = displayControl,
            EditControl = editControl,
            EditOnAssociatedObjectDoubleTapped = true
        });
        var window = new Window { Content = host };

        window.Show();
        window.Click(activationTarget);
        window.Click(activationTarget);

        Assert.False(displayControl.IsVisible);
        Assert.True(editControl.IsVisible);
    }

    [AvaloniaFact]
    public void DoubleTapped_EditControl_DoesNotRestartEdit()
    {
        var activationTarget = new Border { Width = 100, Height = 30, Background = Brushes.Transparent };
        var displayControl = new Border { Width = 100, Height = 30, Background = Brushes.Transparent };
        var editControl = new TextBox { Width = 100, Height = 30, Text = "one two" };
        var host = new StackPanel
        {
            Children =
            {
                activationTarget,
                displayControl,
                editControl
            }
        };
        Interaction.GetBehaviors(host).Add(new InlineEditBehavior
        {
            DisplayControl = displayControl,
            EditControl = editControl,
            EditOnAssociatedObjectDoubleTapped = true
        });
        var window = new Window { Content = host };

        window.Show();
        window.Click(activationTarget);
        window.Click(activationTarget);

        editControl.AddHandler(
            InputElement.DoubleTappedEvent,
            (_, _) =>
            {
                editControl.SelectionStart = 1;
                editControl.SelectionEnd = 3;
            },
            RoutingStrategies.Bubble);

        window.Click(editControl);
        window.Click(editControl);

        Assert.Equal(1, editControl.SelectionStart);
        Assert.Equal(3, editControl.SelectionEnd);
    }
}
