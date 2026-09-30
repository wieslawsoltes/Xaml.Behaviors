#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

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
#if UNO
        // The Uno headless application has no theme: the text box needs its WinUI template to receive pointer input.
        host.Resources.MergedDictionaries.Add(new XamlControlsResources());
#endif
        var window = new Window { Content = host };

        window.Show();
        window.Click(activationTarget);
        window.Click(activationTarget);

        editControl.AddHandler(
            InputElement.DoubleTappedEvent,
            (_, _) =>
            {
                editControl.SelectionStart = 1;
#if UNO
                editControl.SelectionLength = 2;
#else
                editControl.SelectionEnd = 3;
#endif
            },
            RoutingStrategies.Bubble);

        window.Click(editControl);
        window.Click(editControl);

#if UNO
        // The WinUI text box applies its own pointer selection after the routed DoubleTapped handlers: check that the
        // edit was not restarted, which would select the whole text.
        Assert.True(editControl.IsVisible);
        Assert.NotEqual(editControl.Text.Length, editControl.SelectionLength);
#else
        Assert.Equal(1, editControl.SelectionStart);
        Assert.Equal(3, editControl.SelectionEnd);
#endif
    }
}
