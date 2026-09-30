#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
using Xaml.Interactions.Custom;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

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

#if UNO
        // WinUI cannot raise key events: the focused element receives a real Tab key press.
        window.KeyPressQwerty(PhysicalKey.Tab, RawInputModifiers.None);
#else
        cycleSecond.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Tab,
            PhysicalKey = PhysicalKey.Tab,
            KeyModifiers = KeyModifiers.None,
            KeyDeviceType = KeyDeviceType.Keyboard,
            Source = cycleSecond
        });
#endif

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
#if UNO
            IsTabStop = true
#else
            Focusable = true
#endif
        };
    }
}
