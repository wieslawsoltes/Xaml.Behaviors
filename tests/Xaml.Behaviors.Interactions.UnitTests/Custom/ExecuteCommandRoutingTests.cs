#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
using Xaml.Interactions.UnitTests.Core;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

/// <summary>
/// Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/400: the <c>ExecuteCommandOn*</c>
/// behaviors run their command whatever the routing strategies of their event, on both platforms. With the default
/// <see cref="RoutingStrategies.Bubble"/> routing they handle the direct pointer events (entered, exited, capture
/// lost), and an explicit <see cref="RoutingStrategies.Direct"/> on a bubbling event handles the events raised by the
/// element itself, and only those.
/// </summary>
public class ExecuteCommandRoutingTests
{
    [AvaloniaFact]
    public void PointerEntered_And_PointerExited_Behaviors_With_Default_Routing_Run_The_Command()
    {
        var border = new Border { Width = 100, Height = 40, Background = Brushes.Red };
        var enteredCount = 0;
        var exitedCount = 0;
        var entered = new ExecuteCommandOnPointerEnteredBehavior { Command = new Command(_ => enteredCount++) };
        var exited = new ExecuteCommandOnPointerExitedBehavior { Command = new Command(_ => exitedCount++) };
        Interaction.GetBehaviors(border).Add(entered);
        Interaction.GetBehaviors(border).Add(exited);
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { border } } };
        window.Show();

        window.MouseMove(border, new Point(50, 20));
        window.MouseMove(border, new Point(55, 22));

        Assert.Equal(RoutingStrategies.Bubble, entered.EventRoutingStrategy);
        Assert.Equal(1, enteredCount);
        Assert.Equal(0, exitedCount);

        window.MouseMove(border, new Point(50, 150));

        Assert.Equal(RoutingStrategies.Bubble, exited.EventRoutingStrategy);
        Assert.Equal(1, enteredCount);
        Assert.Equal(1, exitedCount);

        window.Close();
    }

    [AvaloniaFact]
    public void PointerCaptureLost_Behavior_With_Default_Routing_Runs_The_Command()
    {
        var border = new Border { Width = 100, Height = 40, Background = Brushes.Red };
        var count = 0;
        var behavior = new ExecuteCommandOnPointerCaptureLostBehavior { Command = new Command(_ => count++) };
        var capture = new PointerPressedTrigger();
        capture.Actions!.Add(new CapturePointerAction { TargetControl = border });
        Interaction.GetBehaviors(border).Add(capture);
        Interaction.GetBehaviors(border).Add(behavior);
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { border } } };
        window.Show();

        // The border captures the pointer on press; the capture is released (and lost) on release.
        window.MouseDown(border, new Point(50, 20), MouseButton.Left);
        window.MouseUp(border, new Point(50, 20), MouseButton.Left);

        Assert.Equal(RoutingStrategies.Bubble, behavior.EventRoutingStrategy);
        Assert.Equal(1, count);

        window.Close();
    }

    [AvaloniaFact]
    public void Behavior_With_Direct_Routing_On_A_Bubbling_Event_Runs_The_Command_For_The_Element_Only()
    {
        var (window, host, child) = CreateHostWithChild();
        var count = 0;
        var behavior = new ExecuteCommandOnPointerPressedBehavior
        {
            Command = new Command(_ => count++),
            EventRoutingStrategy = RoutingStrategies.Direct,
            MarkAsHandled = false,
        };
        Interaction.GetBehaviors(host).Add(behavior);
        window.Show();

        // Pressed on the child: the event bubbles to the host, but it was not raised by the host.
        window.MouseDown(child, new Point(5, 5), MouseButton.Left);
        window.MouseUp(child, new Point(5, 5), MouseButton.Left);

        Assert.Equal(0, count);

        // Pressed on the padding of the host: raised by the host itself.
        window.MouseDown(host, new Point(5, 5), MouseButton.Left);
        window.MouseUp(host, new Point(5, 5), MouseButton.Left);

        Assert.Equal(1, count);

        window.Close();
    }

    public static TheoryData<RoutingStrategies> RoutedStrategies => new()
    {
        RoutingStrategies.Bubble,
        RoutingStrategies.Tunnel,
        RoutingStrategies.Direct | RoutingStrategies.Bubble,
    };

    [AvaloniaTheory]
    [MemberData(nameof(RoutedStrategies))]
    public void Behavior_With_Bubble_Or_Tunnel_Routing_Runs_The_Command_For_The_Events_Of_A_Child(RoutingStrategies routes)
    {
        var (window, host, child) = CreateHostWithChild();
        var count = 0;
        var behavior = new ExecuteCommandOnPointerPressedBehavior
        {
            Command = new Command(_ => count++),
            EventRoutingStrategy = routes,
            MarkAsHandled = false,
        };
        Interaction.GetBehaviors(host).Add(behavior);
        window.Show();

        window.MouseDown(child, new Point(5, 5), MouseButton.Left);
        window.MouseUp(child, new Point(5, 5), MouseButton.Left);

        Assert.Equal(1, count);

        window.Close();
    }

    private static (Window Window, Border Host, Border Child) CreateHostWithChild()
    {
        var child = new Border { Width = 60, Height = 20, Background = Brushes.Blue };
        var host = new Border
        {
            Padding = new Thickness(20),
            Background = Brushes.Red,
            Child = child,
#if UNO
            HorizontalAlignment = HorizontalAlignment.Left,
#else
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
#endif
        };
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { host } } };
        return (window, host, child);
    }
}
