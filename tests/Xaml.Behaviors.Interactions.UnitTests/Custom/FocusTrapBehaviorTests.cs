using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class FocusTrapBehaviorTests
{
    [AvaloniaFact]
    public void Tab_Inside_Cycle_Group_Uses_Framework_Navigation()
    {
        var trapBefore = CreateButton("TrapBefore");
        var cycleFirst = CreateButton("CycleFirst");
        var cycleSecond = CreateButton("CycleSecond");
        var outside = CreateButton("Outside");

        var cycleContainer = new StackPanel
        {
            Children =
            {
                cycleFirst,
                cycleSecond
            }
        };

        KeyboardNavigation.SetTabNavigation(cycleContainer, KeyboardNavigationMode.Cycle);

        var trap = new StackPanel
        {
            Children =
            {
                trapBefore,
                cycleContainer
            }
        };

        Interaction.GetBehaviors(trap).Add(new FocusTrapBehavior());

        var window = new Window
        {
            Width = 320,
            Height = 160,
            Content = new StackPanel
            {
                Children =
                {
                    trap,
                    outside
                }
            }
        };

        window.Show();
        window.CaptureRenderedFrame();

        Assert.True(cycleSecond.Focus());

        cycleSecond.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Tab,
            PhysicalKey = PhysicalKey.Tab,
            KeyModifiers = KeyModifiers.None,
            KeyDeviceType = KeyDeviceType.Keyboard,
            Source = cycleSecond
        });

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("CycleFirst", (window.FocusManager?.GetFocusedElement() as Button)?.Content);
    }

    private static Button CreateButton(string content)
    {
        return new Button
        {
            Content = content,
            Name = content,
            Width = 120,
            Height = 32,
            Focusable = true
        };
    }
}
