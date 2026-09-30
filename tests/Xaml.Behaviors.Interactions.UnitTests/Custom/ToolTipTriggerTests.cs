#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Core;
using Xaml.Interactions.Custom;
using Xaml.Interactions.UnitTests.Core;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Core;
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

public class ToolTipTriggerTests
{
    [AvaloniaFact]
    public void ToolTipOpeningTrigger_ExecutesActionsForDirectEvent()
    {
        object? commandParameter = null;
        var target = new Border();
#if UNO
        // WinUI has no tooltip routed events: the trigger handles ToolTip.Opened of the element's tooltip.
        var toolTip = new ToolTip { Content = "Tip" };
        ToolTipService.SetToolTip(target, toolTip);
#endif
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
#if UNO
        toolTip.IsOpen = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.IsAssignableFrom<RoutedEventArgs>(commandParameter);
#else
        var eventArgs = new CancelRoutedEventArgs(ToolTip.ToolTipOpeningEvent);

        target.RaiseEvent(eventArgs);

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.Same(eventArgs, commandParameter);
#endif
    }

    [AvaloniaFact]
    public void ToolTipClosingTrigger_ExecutesActionsForDirectEvent()
    {
        object? commandParameter = null;
        var target = new Border();
#if UNO
        // WinUI has no tooltip routed events: the trigger handles ToolTip.Closed of the element's tooltip.
        var toolTip = new ToolTip { Content = "Tip" };
        ToolTipService.SetToolTip(target, toolTip);
#endif
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
#if UNO
        toolTip.IsOpen = true;
        Dispatcher.UIThread.RunJobs();
        Assert.Null(commandParameter);

        toolTip.IsOpen = false;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.IsAssignableFrom<RoutedEventArgs>(commandParameter);
#else
        var eventArgs = new RoutedEventArgs(ToolTip.ToolTipClosingEvent);

        target.RaiseEvent(eventArgs);

        Assert.Equal(RoutingStrategies.Direct, trigger.EventRoutingStrategy);
        Assert.Same(eventArgs, commandParameter);
#endif
    }

#if !UNO
    // The Avalonia ToolTip.ToolTipOpening/ToolTipClosing attached routed events have no WinUI counterpart: WinUI
    // elements have no ToolTipOpening or ToolTipClosing event for EventTriggerBehavior.EventName.
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
#endif
}
