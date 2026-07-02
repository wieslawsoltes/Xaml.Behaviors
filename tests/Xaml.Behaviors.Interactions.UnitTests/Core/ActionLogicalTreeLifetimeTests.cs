using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

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
        Assert.Same(trigger, action.Parent);
        Assert.Same(target, trigger.Parent);

        window.Content = null;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, command.SubscriptionCount);
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);

        window.Content = target;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1, command.SubscriptionCount);
        Assert.Same(trigger, action.Parent);
        Assert.Same(target, trigger.Parent);

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
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);
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
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);

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
        Assert.Same(trigger, firstAction.Parent);

        trigger.Actions = new ActionCollection { secondAction };

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Null(firstAction.Parent);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.Same(trigger, secondAction.Parent);

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
        Assert.Same(ResolveExpectedChildParent(container, kind), childAction.Parent);

        Interaction.GetBehaviors(target).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
        Assert.Null(container.Parent);
        Assert.Null(childAction.Parent);

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
        Assert.Same(group, firstAction.Parent);

        group.Actions = new ActionCollection { secondAction };

        Assert.Equal(0, firstCommand.SubscriptionCount);
        Assert.Null(firstAction.Parent);
        Assert.Equal(1, secondCommand.SubscriptionCount);
        Assert.Same(group, secondAction.Parent);

        window.Close();
    }

    [AvaloniaFact]
    public void NestedActionBindings_ClearWhenContainerLogicalTreeDetaches()
    {
        var command = new ObservableCommand { CanExecuteResult = false };
        var window = CreateWindow();
        var target = CreateTarget();
        var indicator = new Border();
        var trigger = new ClickEventTrigger();
        var group = new AsyncActionGroup();
        var action = CreateCommandAction(command);

        indicator.Bind(InputElement.IsEnabledProperty, action.GetObservable(InvokeCommandActionBase.CanExecuteCommandProperty));
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
        Assert.Same(trigger, action.Parent);

        popup.IsOpen = false;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, command.SubscriptionCount);
        Assert.Null(action.Parent);
        Assert.Null(trigger.Parent);

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
        Assert.Same(window, trigger.Parent);
        Assert.Same(window, action.Parent);

        Interaction.GetBehaviors(window).Remove(trigger);

        Assert.Equal(0, command.SubscriptionCount);
        Assert.Null(trigger.Parent);
        Assert.Null(action.Parent);
    }

    private static Window CreateWindow()
    {
        return new Window
        {
            Width = 320,
            Height = 200
        };
    }

    private static Border CreateTarget()
    {
        return new Border
        {
            Width = 160,
            Height = 60,
            Focusable = true
        };
    }

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

    private static StyledElement ResolveExpectedChildParent(StyledElementAction container, NestedActionContainerKind kind)
    {
        if (container is SwitchCaseAction switchCaseAction && kind == NestedActionContainerKind.SwitchCaseCaseActions)
        {
            return switchCaseAction.Cases![0];
        }

        return container;
    }

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
