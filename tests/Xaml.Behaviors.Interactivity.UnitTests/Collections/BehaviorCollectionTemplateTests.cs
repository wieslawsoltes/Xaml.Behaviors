using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class BehaviorCollectionTemplateTests
{
    [AvaloniaFact]
    public void BehaviorCollectionTemplate_001()
    {
        var window = new BehaviorCollectionTemplate001();

        window.Show();

        var buttons = new[]
        {
            window.TargetButton1,
            window.TargetButton2,
            window.TargetButton3
        };

        var behavior0 = buttons[0].GetValue(Interaction.BehaviorsProperty);
        Assert.NotNull(behavior0);
        Assert.Single(behavior0);
        Assert.Equal(buttons[0], behavior0!.AssociatedObject);

        var behavior1 = buttons[1].GetValue(Interaction.BehaviorsProperty);
        Assert.NotNull(behavior1);
        Assert.Single(behavior1);
        Assert.Equal(buttons[1], behavior1!.AssociatedObject);

        var behavior2 = buttons[2].GetValue(Interaction.BehaviorsProperty);
        Assert.NotNull(behavior2);
        Assert.Single(behavior2);
        Assert.Equal(buttons[2], behavior2!.AssociatedObject);

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
