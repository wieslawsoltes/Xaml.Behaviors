using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class ToolTipTriggerTests
{
    [AvaloniaFact]
    public void ToolTipOpeningTrigger_ExecutesActionsForDirectEvent()
    {
        object? commandParameter = null;
        var target = new Border();
        var trigger = new ToolTipOpeningTrigger();
        var action = new InvokeCommandAction
        {
            Command = new Command(parameter => commandParameter = parameter),
            PassEventArgsToCommand = true,
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = target };
        window.Show();
        var eventArgs = new CancelRoutedEventArgs(ToolTip.ToolTipOpeningEvent);

        target.RaiseEvent(eventArgs);

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.Same(eventArgs, commandParameter);
    }

    [AvaloniaFact]
    public void ToolTipClosingTrigger_ExecutesActionsForDirectEvent()
    {
        object? commandParameter = null;
        var target = new Border();
        var trigger = new ToolTipClosingTrigger();
        var action = new InvokeCommandAction
        {
            Command = new Command(parameter => commandParameter = parameter),
            PassEventArgsToCommand = true,
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var window = new Window { Content = target };
        window.Show();
        var eventArgs = new RoutedEventArgs(ToolTip.ToolTipClosingEvent);

        target.RaiseEvent(eventArgs);

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.Same(eventArgs, commandParameter);
    }

    [AvaloniaFact]
    public void EventTriggerBehavior_ExecutesActionsForToolTipOpeningEvent()
    {
        object? commandParameter = null;
        var target = new Border();
        var trigger = new EventTriggerBehavior
        {
            EventName = "ToolTipOpening",
        };
        var action = new InvokeCommandAction
        {
            Command = new Command(parameter => commandParameter = parameter),
            PassEventArgsToCommand = true,
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var eventArgs = new CancelRoutedEventArgs(ToolTip.ToolTipOpeningEvent);

        target.RaiseEvent(eventArgs);

        Assert.Same(eventArgs, commandParameter);
    }

    [AvaloniaFact]
    public void EventTriggerBehavior_ExecutesActionsForToolTipClosingEvent()
    {
        object? commandParameter = null;
        var target = new Border();
        var trigger = new EventTriggerBehavior
        {
            EventName = "ToolTipClosing",
        };
        var action = new InvokeCommandAction
        {
            Command = new Command(parameter => commandParameter = parameter),
            PassEventArgsToCommand = true,
        };
        trigger.Actions ??= [];
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);
        var eventArgs = new RoutedEventArgs(ToolTip.ToolTipClosingEvent);

        target.RaiseEvent(eventArgs);

        Assert.Same(eventArgs, commandParameter);
    }
}
