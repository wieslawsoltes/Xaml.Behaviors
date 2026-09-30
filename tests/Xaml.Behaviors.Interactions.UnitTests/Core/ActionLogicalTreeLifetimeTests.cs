using System;
using System.ComponentModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactions.Core;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

public class ActionLogicalTreeLifetimeTests
{
    [AvaloniaFact]
    public void TriggerAction_DetachesAndReattachesAcrossAssociatedControlLogicalTreeLifetime()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);
#if UNO
        Assert.Same(target, action.Host);
        Assert.Same(target, trigger.Actions!.Host);
#else
        Assert.Same(trigger, action.Parent);
        Assert.Same(target, trigger.Parent);
#endif

        window.Content = null;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(action.Host);
        Assert.Null(trigger.Actions!.Host);
#else
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);
#endif

        window.Content = target;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, command.SubscriptionCount);
#if UNO
        Assert.Same(target, action.Host);
        Assert.Same(target, trigger.Actions!.Host);
#else
        Assert.Same(trigger, action.Parent);
        Assert.Same(target, trigger.Parent);
#endif

        window.Close();
    }

    [AvaloniaFact]
    public void TriggerAction_DetachesWhenWindowCloses()
    {
        var command = new ObservableCommand();
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);

        window.Close();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(action.Host);
        Assert.Null(trigger.Actions!.Host);
#else
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);
#endif
    }

    [AvaloniaFact]
    public void BehaviorCollectionRemove_DetachesTriggerActionLogicalTree()
    {
        var command = new ObservableCommand();
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);

        Interaction.GetBehaviors(target).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(action.Host);
        Assert.Null(trigger.Actions!.Host);
#else
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);
#endif

        window.Close();
    }

    [AvaloniaFact]
    public void TriggerActionsReplacement_DetachesOldActionLogicalTreeAndAttachesNewAction()
    {
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var firstAction = CreateCommandAction(firstCommand);
        var secondAction = CreateCommandAction(secondCommand);

        trigger.Actions!.Add(firstAction);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, firstCommand.SubscriptionCount);
#if UNO
        Assert.Same(target, firstAction.Host);
#else
        Assert.Same(trigger, firstAction.Parent);
#endif

        trigger.Actions = new ActionCollection { secondAction };

        Assert.Equal(0, firstCommand.SubscriptionCount);
#if UNO
        Assert.Null(firstAction.Host);
#else
        Assert.Null(firstAction.Parent);
#endif
        Assert.Equal(1, secondCommand.SubscriptionCount);
#if UNO
        Assert.Same(target, secondAction.Host);
#else
        Assert.Same(trigger, secondAction.Parent);
#endif

        window.Close();
    }

    [AvaloniaTheory]
    [InlineData(NestedActionContainerKind.AsyncActionGroup)]
    [InlineData(NestedActionContainerKind.DebounceAction)]
    [InlineData(NestedActionContainerKind.ThrottleAction)]
    [InlineData(NestedActionContainerKind.ConditionalActionTrueBranch)]
    [InlineData(NestedActionContainerKind.ConditionalActionFalseBranch)]
    [InlineData(NestedActionContainerKind.SwitchCaseDefaultActions)]
    [InlineData(NestedActionContainerKind.SwitchCaseCaseActions)]
    public void NestedActionContainers_DetachChildActionLogicalTreeWhenTriggerIsRemoved(NestedActionContainerKind kind)
    {
        var command = new ObservableCommand();
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var container = CreateNestedActionContainer(kind);
        var childAction = CreateCommandAction(command);

        AddChildAction(container, kind, childAction);
        trigger.Actions!.Add(container);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);
#if UNO
        Assert.Same(target, childAction.Host);
#else
        Assert.Same(ResolveExpectedChildParent(container, kind), childAction.Parent);
#endif

        Interaction.GetBehaviors(target).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(container.Host);
        Assert.Null(childAction.Host);
#else
        Assert.Null(container.Parent);
        Assert.Null(childAction.Parent);
#endif

        window.Close();
    }

    [AvaloniaFact]
    public void NestedActionContainerActionsReplacement_DetachesOldChildActionAndAttachesNewChildAction()
    {
        var firstCommand = new ObservableCommand { CanExecuteResult = false };
        var secondCommand = new ObservableCommand { CanExecuteResult = true };
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var group = new AsyncActionGroup();
        var firstAction = CreateCommandAction(firstCommand);
        var secondAction = CreateCommandAction(secondCommand);

        group.Actions!.Add(firstAction);
        trigger.Actions!.Add(group);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, firstCommand.SubscriptionCount);
#if UNO
        Assert.Same(target, firstAction.Host);
#else
        Assert.Same(group, firstAction.Parent);
#endif

        group.Actions = new ActionCollection { secondAction };

        Assert.Equal(0, firstCommand.SubscriptionCount);
#if UNO
        Assert.Null(firstAction.Host);
#else
        Assert.Null(firstAction.Parent);
#endif
        Assert.Equal(1, secondCommand.SubscriptionCount);
#if UNO
        Assert.Same(target, secondAction.Host);
#else
        Assert.Same(group, secondAction.Parent);
#endif

        window.Close();
    }

    [AvaloniaFact]
    public void NestedActionBindings_ClearWhenContainerLogicalTreeDetaches()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var window = CreateWindow();
        var target = CreateTarget();
#if UNO
        // WinUI Border has no IsEnabled (only controls have).
        var indicator = new ContentControl();
#else
        var indicator = new Border();
#endif
        var trigger = new ClickEventTrigger();
        var group = new AsyncActionGroup();
        var action = CreateCommandAction(command);

#if UNO
        indicator.Bind(Microsoft.UI.Xaml.Controls.Control.IsEnabledProperty, action.GetObservable<bool>(InvokeCommandActionBase.CanExecuteCommandProperty));
#else
        indicator.Bind(InputElement.IsEnabledProperty, action.GetObservable(InvokeCommandActionBase.CanExecuteCommandProperty));
#endif
        group.Actions!.Add(action);
        trigger.Actions!.Add(group);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = new StackPanel
        {
            Children =
            {
                target,
                indicator
            }
        };
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);
        Assert.False(indicator.IsEnabled);

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();

        Assert.True(indicator.IsEnabled);

        Interaction.GetBehaviors(target).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);

        command.CanExecuteResult = false;
        command.RaiseCanExecuteChanged();

        Assert.True(action.CanExecuteCommand);
        Assert.True(indicator.IsEnabled);

        window.Close();
    }

    [AvaloniaFact]
    public void UseCommandCanExecuteForIsEnabledBinding_ClearsWhenTriggerLogicalTreeDetaches()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var window = CreateWindow();
        var target = CreateTarget();
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);
        action.UseCommandCanExecuteForIsEnabled = true;

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(target).Add(trigger);

        window.Content = target;
        window.Show();

        Assert.Equal(1, command.SubscriptionCount);
        Assert.False(target.IsEnabled);

        Interaction.GetBehaviors(target).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
        Assert.True(target.IsEnabled);

        command.CanExecuteResult = false;
        command.RaiseCanExecuteChanged();

        Assert.True(target.IsEnabled);

        window.Close();
    }

    [AvaloniaFact]
    public void PopupHostedTriggerAction_DetachesWhenPopupCloses()
    {
        var command = new ObservableCommand();
        var window = CreateWindow();
        var placementTarget = new Button
        {
            Content = "Host",
            Width = 120,
            Height = 32
        };
        var popupTarget = CreateTarget();
        var popup = new Popup
        {
            PlacementTarget = placementTarget,
            Child = popupTarget
        };
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(popupTarget).Add(trigger);

        window.Content = new Grid
        {
            Children =
            {
                placementTarget,
                popup
            }
        };
        window.Show();

        popup.IsOpen = true;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, command.SubscriptionCount);
#if UNO
        Assert.Same(popupTarget, action.Host);
#else
        Assert.Same(trigger, action.Parent);
#endif

        popup.IsOpen = false;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(action.Host);
        Assert.Null(trigger.Actions!.Host);
#else
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);
#endif

        window.Close();
    }

    [AvaloniaFact]
    public void TopLevelHostedTrigger_DetachesActionLogicalTreeWhenBehaviorIsRemoved()
    {
        var command = new ObservableCommand();
        var window = CreateWindow();
        var trigger = new ClickEventTrigger();
        var action = CreateCommandAction(command);

        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(window).Add(trigger);

        Assert.Equal(1, command.SubscriptionCount);
#if UNO
        Assert.Same(window, trigger.Actions!.Host);
        Assert.Same(window, action.Host);
#else
        Assert.Same(window, trigger.Parent);
        Assert.Same(window, action.Parent);
#endif

        Interaction.GetBehaviors(window).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
#if UNO
        Assert.Null(trigger.Actions!.Host);
        Assert.Null(action.Host);
#else
        Assert.Null(trigger.Parent);
        Assert.Null(action.Parent);
#endif
    }

    private static Window CreateWindow()
    {
        return new Window
        {
            Width = 320,
            Height = 200
        };
    }

#if UNO
    // WinUI Border has no IsEnabled and is not focusable: the target is a focusable content control.
    private static ContentControl CreateTarget()
    {
        return new ContentControl
        {
            Width = 160,
            Height = 60,
            IsTabStop = true
        };
    }
#else
    private static Border CreateTarget()
    {
        return new Border
        {
            Width = 160,
            Height = 60,
            Focusable = true
        };
    }
#endif

    private static InvokeCommandAction CreateCommandAction(ObservableCommand command)
    {
        return new InvokeCommandAction
        {
            Command = command,
            CanExecuteCommandParameter = "probe"
        };
    }

    private static StyledElementAction CreateNestedActionContainer(NestedActionContainerKind kind)
    {
        return kind switch
        {
            NestedActionContainerKind.AsyncActionGroup => new AsyncActionGroup(),
            NestedActionContainerKind.DebounceAction => new DebounceAction(),
            NestedActionContainerKind.ThrottleAction => new ThrottleAction(),
            NestedActionContainerKind.ConditionalActionTrueBranch => new ConditionalAction(),
            NestedActionContainerKind.ConditionalActionFalseBranch => new ConditionalAction(),
            NestedActionContainerKind.SwitchCaseDefaultActions => new SwitchCaseAction(),
            NestedActionContainerKind.SwitchCaseCaseActions => new SwitchCaseAction(),
            _ => throw new InvalidEnumArgumentException(nameof(kind), (int)kind, typeof(NestedActionContainerKind))
        };
    }

    private static void AddChildAction(StyledElementAction container, NestedActionContainerKind kind, StyledElementAction child)
    {
        switch (container)
        {
            case AsyncActionGroup group:
                group.Actions!.Add(child);
                break;
            case DebounceAction debounceAction:
                debounceAction.Actions!.Add(child);
                break;
            case ThrottleAction throttleAction:
                throttleAction.Actions!.Add(child);
                break;
            case ConditionalAction conditionalAction when kind == NestedActionContainerKind.ConditionalActionTrueBranch:
                conditionalAction.Actions!.Add(child);
                break;
            case ConditionalAction conditionalAction:
                conditionalAction.ElseActions!.Add(child);
                break;
            case SwitchCaseAction switchCaseAction when kind == NestedActionContainerKind.SwitchCaseDefaultActions:
                switchCaseAction.DefaultActions!.Add(child);
                break;
            case SwitchCaseAction switchCaseAction:
                var caseItem = new Case
                {
                    Value = "case"
                };
                caseItem.Actions!.Add(child);
                switchCaseAction.Cases!.Add(caseItem);
                break;
            default:
                throw new ArgumentException("Unsupported nested action container.", nameof(container));
        }
    }

#if !UNO
    private static StyledElement ResolveExpectedChildParent(StyledElementAction container, NestedActionContainerKind kind)
    {
        if (container is SwitchCaseAction switchCaseAction && kind == NestedActionContainerKind.SwitchCaseCaseActions)
        {
            return switchCaseAction.Cases![0];
        }

        return container;
    }
#endif

    public enum NestedActionContainerKind
    {
        AsyncActionGroup,
        DebounceAction,
        ThrottleAction,
        ConditionalActionTrueBranch,
        ConditionalActionFalseBranch,
        SwitchCaseDefaultActions,
        SwitchCaseCaseActions
    }
}
