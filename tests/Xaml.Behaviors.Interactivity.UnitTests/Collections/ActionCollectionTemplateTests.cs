using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Core;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class ActionCollectionTemplateTests
{
    [AvaloniaFact]
    public void ActionCollectionTemplate_001()
    {
        var window = new ActionCollectionTemplate001();

        window.Show();

        var buttons = new[]
        {
            window.TargetButton1,
            window.TargetButton2,
            window.TargetButton3
        };

        foreach (var button in buttons)
        {
            var behaviors = button.GetValue(Interaction.BehaviorsProperty);
            Assert.NotNull(behaviors);
            Assert.Single(behaviors);
            var trigger = Assert.IsType<EventTriggerBehavior>(behaviors![0]);
            var actions = trigger.Actions;
            Assert.NotNull(actions);
            Assert.Single(actions!);
            Assert.IsType<ChangePropertyAction>(actions![0]);
        }

        Assert.Equal(buttons[0].Background, Brushes.Transparent);
        Assert.Equal(buttons[1].Background, Brushes.Transparent);
        Assert.Equal(buttons[2].Background, Brushes.Transparent);

        buttons[0].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(window.Resources["RedBrush"], buttons[0].Background);

        buttons[1].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(window.Resources["RedBrush"], buttons[1].Background);

        buttons[2].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(window.Resources["RedBrush"], buttons[2].Background);
    }
}
