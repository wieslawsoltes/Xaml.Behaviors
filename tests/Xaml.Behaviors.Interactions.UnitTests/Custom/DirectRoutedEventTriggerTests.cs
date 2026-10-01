#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using System;
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

/// <summary>
/// Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/399: with the default
/// <see cref="RoutingStrategies.Direct"/> routing strategy, the routed event triggers of tunneling or bubbling events
/// (key events) handle the events raised by the associated element itself, and only those, on both platforms.
/// </summary>
public class DirectRoutedEventTriggerTests
{
    public static TheoryData<string> KeyTriggers => new()
    {
        "KeyTrigger.KeyDown",
        "KeyTrigger.KeyUp",
        "KeyDownTrigger",
        "KeyUpTrigger",
    };

    [AvaloniaTheory]
    [MemberData(nameof(KeyTriggers))]
    public void Key_Trigger_With_Default_Routing_Fires_For_Keys_Raised_By_The_Element(string name)
    {
        var textBox = new TextBox();
        var trigger = CreateKeyTrigger(name, routes: null);
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(textBox).Add(trigger);
        var window = new Window { Width = 200, Height = 200, Content = new Border { Child = textBox } };
        window.Show();
        Assert.True(textBox.Focus());
        Dispatcher.UIThread.RunJobs();

        PressAndRelease(window, PhysicalKey.F5);

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.Equal(1, action.ExecutionCount);

        window.Close();
    }

    [AvaloniaTheory]
    [MemberData(nameof(KeyTriggers))]
    public void Key_Trigger_With_Default_Routing_Ignores_Keys_Raised_By_A_Child(string name)
    {
        var textBox = new TextBox();
        var border = new Border { Child = textBox };
        var trigger = CreateKeyTrigger(name, routes: null);
        var bubbleTrigger = CreateKeyTrigger(name, RoutingStrategies.Bubble);
        var tunnelTrigger = CreateKeyTrigger(name, RoutingStrategies.Tunnel);
        var action = new CountingAction();
        var bubbleAction = new CountingAction();
        var tunnelAction = new CountingAction();
        trigger.Actions!.Add(action);
        bubbleTrigger.Actions!.Add(bubbleAction);
        tunnelTrigger.Actions!.Add(tunnelAction);
        Interaction.GetBehaviors(border).Add(trigger);
        Interaction.GetBehaviors(border).Add(bubbleTrigger);
        Interaction.GetBehaviors(border).Add(tunnelTrigger);
        var window = new Window { Width = 200, Height = 200, Content = border };
        window.Show();
        Assert.True(textBox.Focus());
        Dispatcher.UIThread.RunJobs();

        PressAndRelease(window, PhysicalKey.F5);

        Assert.Equal(0, action.ExecutionCount);

        // An explicit Bubble or Tunnel routing strategy still handles the events of the descendants.
        Assert.Equal(1, bubbleAction.ExecutionCount);
        Assert.Equal(1, tunnelAction.ExecutionCount);

        window.Close();
    }

    [AvaloniaFact]
    public void KeyTrigger_On_A_TextBox_Fires_On_Enter_Down_And_Up()
    {
        // The KeyTriggerView sample: two KeyTriggers with the default routing on a TextBox.
        var textBox = new TextBox();
        var keyDown = new KeyTrigger { Key = Key.Enter, Event = KeyTrigger.FiredOn.KeyDown };
        var keyUp = new KeyTrigger { Key = Key.Enter, Event = KeyTrigger.FiredOn.KeyUp };
        var keyDownAction = new CountingAction();
        var keyUpAction = new CountingAction();
        keyDown.Actions!.Add(keyDownAction);
        keyUp.Actions!.Add(keyUpAction);
        Interaction.GetBehaviors(textBox).Add(keyDown);
        Interaction.GetBehaviors(textBox).Add(keyUp);
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { textBox } } };
        window.Show();
        Assert.True(textBox.Focus());
        Dispatcher.UIThread.RunJobs();

        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, keyDownAction.ExecutionCount);

#if !UNO
        // Avalonia's KeyPressQwerty only raises the key down event (the Uno counterpart presses and releases the key).
        Assert.Equal(0, keyUpAction.ExecutionCount);
        window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
#endif

        Assert.Equal(1, keyUpAction.ExecutionCount);

        window.Close();
    }

    [AvaloniaFact]
    public void KeyDownTrigger_And_KeyUpTrigger_On_A_TextBox_Fire_For_Any_Key()
    {
        // The KeyInputTriggersView sample: KeyDownTrigger and KeyUpTrigger without a key filter on a TextBox.
        var textBox = new TextBox();
        var keyDown = new KeyDownTrigger();
        var keyUp = new KeyUpTrigger();
        var keyDownAction = new CountingAction();
        var keyUpAction = new CountingAction();
        keyDown.Actions!.Add(keyDownAction);
        keyUp.Actions!.Add(keyUpAction);
        Interaction.GetBehaviors(textBox).Add(keyDown);
        Interaction.GetBehaviors(textBox).Add(keyUp);
        var window = new Window { Width = 200, Height = 200, Content = new StackPanel { Children = { textBox } } };
        window.Show();
        Assert.True(textBox.Focus());
        Dispatcher.UIThread.RunJobs();

        PressAndRelease(window, PhysicalKey.F5);

        Assert.Equal(1, keyDownAction.ExecutionCount);
        Assert.Equal(1, keyUpAction.ExecutionCount);

        window.Close();
    }

#if !UNO
    private static readonly RoutedEvent<RoutedEventArgs> TunnelOnlyEvent =
        RoutedEvent.Register<DirectRoutedEventTriggerTests, RoutedEventArgs>("TunnelOnly", RoutingStrategies.Tunnel);

    [AvaloniaFact]
    public void TryGetEmulatedDirectRoutes_Maps_Direct_Only_Subscriptions_To_Routed_Events()
    {
        // Direct alone on a tunneling and bubbling or a bubbling event: the bubble route, filtered by source.
        Assert.True(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.KeyDownEvent, RoutingStrategies.Direct, out var routes));
        Assert.Equal(RoutingStrategies.Bubble, routes);
        Assert.True(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.GotFocusEvent, RoutingStrategies.Direct, out routes));
        Assert.Equal(RoutingStrategies.Bubble, routes);

        // Direct events, and subscriptions that include Tunnel or Bubble, are unchanged.
        Assert.False(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.PointerEnteredEvent, RoutingStrategies.Direct, out routes));
        Assert.Equal(RoutingStrategies.Direct, routes);
        Assert.False(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.KeyDownEvent, RoutingStrategies.Direct | RoutingStrategies.Bubble, out routes));
        Assert.Equal(RoutingStrategies.Direct | RoutingStrategies.Bubble, routes);
        Assert.False(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.KeyDownEvent, RoutingStrategies.Tunnel, out routes));
        Assert.Equal(RoutingStrategies.Tunnel, routes);
        Assert.False(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.KeyDownEvent, RoutingStrategies.Bubble, out routes));
        Assert.Equal(RoutingStrategies.Bubble, routes);
        Assert.False(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(InputElement.KeyDownEvent, (RoutingStrategies)0, out routes));
        Assert.Equal((RoutingStrategies)0, routes);

        // A tunnel-only event is emulated on the tunnel route.
        Assert.True(RoutedEventTriggerBase.TryGetEmulatedDirectRoutes(TunnelOnlyEvent, RoutingStrategies.Direct, out routes));
        Assert.Equal(RoutingStrategies.Tunnel, routes);
    }

    [AvaloniaFact]
    public void RoutedEventTrigger_With_Default_Routing_Handles_Bubbling_Events_Of_The_Element_Only()
    {
        var child = new Button();
        var parent = new Border { Child = child };
        var trigger = new ClickRoutedEventTrigger();
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(parent).Add(trigger);
        var window = new Window { Content = parent };
        window.Show();

        child.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(0, action.ExecutionCount);

        parent.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.Equal(1, action.ExecutionCount);

        window.Close();
    }

    [AvaloniaFact]
    public void Direct_Trigger_Stops_Handling_Events_When_Detached()
    {
        var textBox = new TextBox();
        var trigger = new KeyDownTrigger();
        var action = new CountingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(textBox).Add(trigger);
        var window = new Window { Content = textBox };
        window.Show();

        textBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.F5 });
        Assert.Equal(1, action.ExecutionCount);

        Interaction.GetBehaviors(textBox).Remove(trigger);
        textBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.F5 });

        Assert.Equal(1, action.ExecutionCount);

        window.Close();
    }

    private sealed class ClickRoutedEventTrigger : RoutedEventTrigger
    {
        protected override RoutedEvent RoutedEvent => Button.ClickEvent;
    }
#endif

    private static RoutedEventTriggerBase<KeyEventArgs> CreateKeyTrigger(string name, RoutingStrategies? routes)
    {
        RoutedEventTriggerBase<KeyEventArgs> trigger = name switch
        {
            "KeyTrigger.KeyDown" => new KeyTrigger { Key = Key.F5, Event = KeyTrigger.FiredOn.KeyDown },
            "KeyTrigger.KeyUp" => new KeyTrigger { Key = Key.F5, Event = KeyTrigger.FiredOn.KeyUp },
            "KeyDownTrigger" => new KeyDownTrigger { Key = Key.F5 },
            "KeyUpTrigger" => new KeyUpTrigger { Key = Key.F5 },
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };

        if (routes is { } value)
        {
            trigger.EventRoutingStrategy = value;
        }

        return trigger;
    }

    private static void PressAndRelease(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
#if !UNO
        // Avalonia's KeyPressQwerty only raises the key down event (the Uno counterpart presses and releases the key).
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
#endif
        Dispatcher.UIThread.RunJobs();
    }

#if UNO
    private sealed class CountingAction : Xaml.Interactivity.Action
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
