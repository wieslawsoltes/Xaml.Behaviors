using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
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

        var containers = window.TargetListBox.GetRealizedContainers().Cast<ListBoxItem>().ToList();

        foreach (var container in containers)
        {
            var behaviors = container.GetValue(Interaction.BehaviorsProperty);
            Assert.NotNull(behaviors);
            Assert.Single(behaviors);
            var trigger = Assert.IsType<EventTriggerBehavior>(behaviors![0]);
            var actions = trigger.Actions;
            Assert.NotNull(actions);
            Assert.Single(actions!);
            Assert.IsType<ChangePropertyAction>(actions![0]);
        }

        Assert.Equal(containers[0].Background, Brushes.Transparent);
        Assert.Equal(containers[1].Background, Brushes.Transparent);
        Assert.Equal(containers[2].Background, Brushes.Transparent);

        containers[0].Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Assert.Equal(window.Resources["RedBrush"], containers[0].Background);

        containers[1].Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Assert.Equal(window.Resources["RedBrush"], containers[1].Background);

        containers[2].Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Assert.Equal(window.Resources["RedBrush"], containers[2].Background);
    }
}
