#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Events;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Events;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Events;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Events;
#endif

/// <summary>
/// Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/400: the triggers of the Events package
/// fire whatever the routing strategies of their event, on both platforms. An explicit
/// <see cref="RoutingStrategies.Direct"/> on a bubbling event handles the events raised by the element itself, and only
/// those, and routing strategies without <see cref="RoutingStrategies.Direct"/> handle a direct event.
/// </summary>
public class EventTriggerRoutingTests
{
    [AvaloniaFact]
    public void Trigger_With_Direct_Routing_On_A_Bubbling_Event_Fires_For_The_Element_Only()
    {
        var (window, host, child) = CreateHostWithChild();
        var trigger = new PointerPressedEventTrigger { RoutingStrategies = RoutingStrategies.Direct };
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(host).Add(trigger);
        window.Show();

        // Pressed on the child: the event bubbles to the host, but it was not raised by the host.
        window.MouseDown(child, new Point(5, 5), MouseButton.Left);
        window.MouseUp(child, new Point(5, 5), MouseButton.Left);

        Assert.Equal(0, action.ExecutionCount);

        // Pressed on the padding of the host: raised by the host itself.
        window.MouseDown(host, new Point(5, 5), MouseButton.Left);
        window.MouseUp(host, new Point(5, 5), MouseButton.Left);

        Assert.Equal(1, action.ExecutionCount);

        window.Close();
    }

    [AvaloniaFact]
    public void Trigger_With_Direct_Routing_Stops_Firing_When_Detached()
    {
        var (window, host, _) = CreateHostWithChild();
        var trigger = new PointerPressedEventTrigger { RoutingStrategies = RoutingStrategies.Direct };
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(host).Add(trigger);
        window.Show();

        window.MouseDown(host, new Point(5, 5), MouseButton.Left);
        window.MouseUp(host, new Point(5, 5), MouseButton.Left);
        Assert.Equal(1, action.ExecutionCount);

        Interaction.GetBehaviors(host).Remove(trigger);
        window.MouseDown(host, new Point(5, 5), MouseButton.Left);
        window.MouseUp(host, new Point(5, 5), MouseButton.Left);

        Assert.Equal(1, action.ExecutionCount);

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
    public void Trigger_With_Bubble_Or_Tunnel_Routing_Fires_For_The_Events_Of_A_Child(RoutingStrategies routes)
    {
        var (window, host, child) = CreateHostWithChild();
        var trigger = new PointerPressedEventTrigger { RoutingStrategies = routes };
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(host).Add(trigger);
        window.Show();

        window.MouseDown(child, new Point(5, 5), MouseButton.Left);
        window.MouseUp(child, new Point(5, 5), MouseButton.Left);

        Assert.Equal(1, action.ExecutionCount);

        window.Close();
    }

    [AvaloniaFact]
    public void PointerEntered_And_PointerExited_Triggers_With_Bubble_Routing_Fire()
    {
        var (window, host, _) = CreateHostWithChild();
        var entered = new PointerEnteredEventTrigger { RoutingStrategies = RoutingStrategies.Bubble };
        var exited = new PointerExitedEventTrigger { RoutingStrategies = RoutingStrategies.Bubble };
        var enteredAction = new CountingAction();
        var exitedAction = new CountingAction();
        entered.Actions!.Add(enteredAction);
        exited.Actions!.Add(exitedAction);
        Interaction.GetBehaviors(host).Add(entered);
        Interaction.GetBehaviors(host).Add(exited);
        window.Show();

        window.MouseMove(host, new Point(5, 5));
        window.MouseMove(host, new Point(6, 6));

        Assert.Equal(1, enteredAction.ExecutionCount);
        Assert.Equal(0, exitedAction.ExecutionCount);

        window.MouseMove(host, new Point(5, 150));

        Assert.Equal(1, enteredAction.ExecutionCount);
        Assert.Equal(1, exitedAction.ExecutionCount);

        window.Close();
    }

    private static (Window Window, Border Host, Border Child) CreateHostWithChild()
    {
        var child = new Border { Width = 60, Height = 20, Background = CreateBrush(blue: true) };
        var host = new Border
        {
            Padding = new Thickness(20),
            Background = CreateBrush(blue: false),
            Child = child,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { host } } };
        return (window, host, child);
    }

#if UNO
    private static Brush CreateBrush(bool blue)
    {
        return new SolidColorBrush(blue ? Microsoft.UI.Colors.Blue : Microsoft.UI.Colors.Red);
    }
#else
    private static IBrush CreateBrush(bool blue)
    {
        return blue ? Brushes.Blue : Brushes.Red;
    }
#endif

#if UNO
    private sealed class CountingAction : global::Xaml.Interactivity.Action
#else
    private sealed class CountingAction : Avalonia.Xaml.Interactivity.Action
#endif
    {
        public int ExecutionCount { get; private set; }

        public override object? Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return null;
        }
    }
}
