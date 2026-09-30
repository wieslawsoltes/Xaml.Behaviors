#if UNO
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Media;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

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

#if UNO
        var behavior0 = (BehaviorCollection?)buttons[0].GetValue(Interaction.BehaviorsProperty);
#else
        var behavior0 = buttons[0].GetValue(Interaction.BehaviorsProperty);
#endif
        Assert.NotNull(behavior0);
        Assert.Single(behavior0);
        Assert.Equal(buttons[0], behavior0!.AssociatedObject);

#if UNO
        var behavior1 = (BehaviorCollection?)buttons[1].GetValue(Interaction.BehaviorsProperty);
#else
        var behavior1 = buttons[1].GetValue(Interaction.BehaviorsProperty);
#endif
        Assert.NotNull(behavior1);
        Assert.Single(behavior1);
        Assert.Equal(buttons[1], behavior1!.AssociatedObject);

#if UNO
        var behavior2 = (BehaviorCollection?)buttons[2].GetValue(Interaction.BehaviorsProperty);
#else
        var behavior2 = buttons[2].GetValue(Interaction.BehaviorsProperty);
#endif
        Assert.NotNull(behavior2);
        Assert.Single(behavior2);
        Assert.Equal(buttons[2], behavior2!.AssociatedObject);

#if UNO
        // WinUI has no shared Brushes instances: compare the colors.
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(buttons[0].Background).Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(buttons[1].Background).Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(buttons[2].Background).Color);
#else
        Assert.Equal(buttons[0].Background, Brushes.Transparent);
        Assert.Equal(buttons[1].Background, Brushes.Transparent);
        Assert.Equal(buttons[2].Background, Brushes.Transparent);
#endif

#if UNO
        new ButtonAutomationPeer(buttons[0]).Invoke();
#else
        buttons[0].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif

        Assert.Equal(window.Resources["RedBrush"], buttons[0].Background);

#if UNO
        new ButtonAutomationPeer(buttons[1]).Invoke();
#else
        buttons[1].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif

        Assert.Equal(window.Resources["RedBrush"], buttons[1].Background);

#if UNO
        new ButtonAutomationPeer(buttons[2]).Invoke();
#else
        buttons[2].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
#endif

        Assert.Equal(window.Resources["RedBrush"], buttons[2].Background);
    }
}
