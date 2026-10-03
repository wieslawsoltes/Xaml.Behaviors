// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Interactivity.UnitTests;

public class InteractivityTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void SetBehaviors_Attaches_And_Detaches()
    {
        var border = new Border();
        var behavior = new RecordingBehavior();
        var behaviors = new BehaviorCollection { behavior };

        Interaction.SetBehaviors(border, behaviors);

        Assert.Same(border, behavior.AssociatedObject);
        Assert.Same(border, behaviors.AssociatedObject);
        Assert.Contains("Attached", behavior.Events);

        Interaction.SetBehaviors(border, null);

        Assert.Null(behavior.AssociatedObject);
        Assert.Equal("Detaching", behavior.Events.Last());
    }

    [UnoHeadlessFact]
    public async Task Lifecycle_Is_Raised_From_Loaded_And_Unloaded()
    {
        var border = new Border();
        var behavior = new RecordingBehavior();
        Interaction.GetBehaviors(border).Add(behavior);

        await Session.ShowAsync(border);

        Assert.Equal(
            ["Attached", "Initialized", "AttachedToLogicalTree", "AttachedToVisualTree", "Loaded"],
            behavior.Events.Where(static e => !e.StartsWith("PropertyChanged", StringComparison.Ordinal) && e != "DataContextChanged"));

        behavior.Events.Clear();
        Session.Window.Content = new Grid();
        await Session.WaitForIdleAsync();

        Assert.Equal(
            ["Unloaded", "DetachedFromVisualTree", "DetachedFromLogicalTree", "Detaching"],
            behavior.Events.Where(static e => e != "DataContextChanged"));
        Assert.Null(behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public async Task Getter_Created_Collection_Reattaches_When_Loaded_Again()
    {
        var border = new Border();
        var behavior = new RecordingBehavior();
        Interaction.GetBehaviors(border).Add(behavior);

        await Session.ShowAsync(border);
        Session.Window.Content = new Grid();
        await Session.WaitForIdleAsync();
        Assert.Null(behavior.AssociatedObject);

        await Session.ShowAsync(border);

        Assert.Same(border, behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public async Task Behavior_Added_To_Loaded_Element_Catches_Up_Lifecycle()
    {
        var border = new Border();
        await Session.ShowAsync(border);

        var behavior = new RecordingBehavior();
        Interaction.GetBehaviors(border).Add(behavior);

        Assert.Equal(
            ["Attached", "Initialized", "AttachedToLogicalTree", "AttachedToVisualTree", "Loaded"],
            behavior.Events.Where(static e => !e.StartsWith("PropertyChanged", StringComparison.Ordinal) && e != "DataContextChanged"));
    }

    [UnoHeadlessFact]
    public void Generated_Properties_Have_Defaults_And_Route_Changes()
    {
        var behavior = new RecordingBehavior();

        Assert.True(behavior.IsEnabled);
        Assert.True((bool)behavior.GetValue(Behavior.IsEnabledProperty));

        behavior.Text = "a";
        behavior.IsEnabled = false;

        Assert.Equal("a", behavior.GetValue(RecordingBehavior.TextProperty));
        Assert.Contains("PropertyChanged:Text", behavior.Events);
        Assert.Contains("PropertyChanged:IsEnabled", behavior.Events);
    }

    [UnoHeadlessFact]
    public async Task Behavior_Inherits_DataContext_And_Binds()
    {
        var border = new Border { DataContext = new TestViewModel { Name = "bound" } };
        var behavior = new RecordingBehavior();
        BindingOperations.SetBinding(behavior, RecordingBehavior.TextProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Name)) });
        Interaction.GetBehaviors(border).Add(behavior);

        await Session.ShowAsync(border);

        Assert.Equal("bound", behavior.Text);
    }

    [UnoHeadlessFact]
    public void Typed_Behavior_Rejects_Other_Types()
    {
        var behavior = new ButtonOnlyBehavior();

        Assert.Throws<InvalidOperationException>(() => behavior.Attach(new Border()));
    }

    [UnoHeadlessFact]
    public void Behavior_Cannot_Attach_Twice()
    {
        var behavior = new RecordingBehavior();
        behavior.Attach(new Border());

        Assert.Throws<InvalidOperationException>(() => behavior.Attach(new Border()));
    }

    [UnoHeadlessFact]
    public void BehaviorCollection_Rejects_Non_Behaviors()
    {
        var behaviors = new BehaviorCollection();

        Assert.Throws<InvalidOperationException>(() => behaviors.Add(new Border()));
    }

    [UnoHeadlessFact]
    public void Trigger_Actions_Are_Stored_In_Property_And_Executed()
    {
        var border = new Border();
        var trigger = new ManualTrigger();
        var action = new RecordingAction();
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);

        Assert.Same(trigger.Actions, trigger.GetValue(Trigger.ActionsProperty));

        var results = trigger.Fire("p").ToList();

        Assert.Equal(["executed"], results);
        Assert.Equal([(border, "p")], action.Calls.Select(static c => (c.Sender as Border, c.Parameter as string)).ToList()!);
    }

    [UnoHeadlessFact]
    public void ActionCollection_Rejects_Non_Actions()
    {
        var actions = new ActionCollection();

        Assert.Throws<InvalidOperationException>(() => actions.Add(new Border()));
    }

    [UnoHeadlessFact]
    public void Action_Host_Follows_Trigger_Attachment()
    {
        var border = new Border();
        var trigger = new ManualTrigger();
        var action = new RecordingAction();
        trigger.Actions.Add(action);

        trigger.Attach(border);
        Assert.Same(border, action.Host);

        trigger.Detach();
        Assert.Null(action.Host);
    }

    [UnoHeadlessFact]
    public async Task InvokeCommandAction_Executes_And_Controls_IsEnabled()
    {
        var command = new TestCommand();
        var button = new Button();
        var trigger = new ManualTrigger();
        var action = new TestInvokeCommandAction
        {
            Command = command,
            CommandParameter = "param",
            UseCommandCanExecuteForIsEnabled = true,
        };
        trigger.Actions.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        trigger.Fire("ignored").ToList();
        Assert.Equal(["param"], command.Executed);
        Assert.True(action.CanExecuteCommand);
        Assert.True(button.IsEnabled);

        command.Enabled = false;
        await Session.WaitForIdleAsync();

        Assert.False(action.CanExecuteCommand);
        Assert.False(button.IsEnabled);

        command.Enabled = true;
        await Session.WaitForIdleAsync();

        Assert.True(button.IsEnabled);
    }

    [UnoHeadlessFact]
    public async Task EventTrigger_Uses_Registered_Click_Handler()
    {
        var button = new Button();
        var trigger = new ClickTrigger { EventName = "Click" };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        new ButtonAutomationPeer(button).Invoke();
        await Session.WaitForIdleAsync();

        Assert.Single(action.Calls);
        Assert.Same(button, action.Calls[0].Sender);
    }

    [UnoHeadlessFact]
    public async Task EventTrigger_Default_Event_Fires_When_Loaded()
    {
        var border = new Border();
        var trigger = new ClickTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);

        await Session.ShowAsync(border);

        Assert.Single(action.Calls);
    }

    [UnoHeadlessFact]
    public void PropertyHelper_Updates_Dependency_Clr_And_Attached_Properties()
    {
        var border = new Border();

        Assert.True(PropertyHelper.UpdatePropertyValue(border, "Width", "42"));
        Assert.Equal(42d, border.Width);

        Assert.True(PropertyHelper.UpdatePropertyValue(border, "Grid.Row", 3));
        Assert.Equal(3, Grid.GetRow(border));

        Assert.True(PropertyHelper.TryGetPropertyValue(border, "Width", out var width, out var preserve));
        Assert.Equal(42d, width);
        Assert.True(preserve);
    }

    [UnoHeadlessFact]
    public void PropertyHelper_Temporary_Value_Is_Reverted()
    {
        var border = new Border { Width = 10 };

        Assert.True(PropertyHelper.TrySetTemporaryAvaloniaPropertyValue(border, "Width", 20d, out var reversion));
        Assert.Equal(20d, border.Width);

        reversion!.Dispose();

        Assert.Equal(10d, border.Width);
    }

    [UnoHeadlessTheory]
    [InlineData(ComparisonConditionType.Equal, 5, "5", true)]
    [InlineData(ComparisonConditionType.NotEqual, 5, "5", false)]
    [InlineData(ComparisonConditionType.LessThan, 4, "5", true)]
    [InlineData(ComparisonConditionType.GreaterThanOrEqual, 5, "5", true)]
    public void ComparisonConditionTypeHelper_Compares(ComparisonConditionType type, int left, string right, bool expected)
    {
        Assert.Equal(expected, ComparisonConditionTypeHelper.Compare(left, type, right));
    }

    [UnoHeadlessFact]
    public void Condition_Rejects_Property_And_Binding_Together()
    {
        var condition = new Condition { Property = FrameworkElement.WidthProperty };

        Assert.Throws<InvalidOperationException>(() => condition.Binding = "value");
    }

    [UnoHeadlessFact]
    public void Condition_BindingValue_Is_The_Bound_Value()
    {
#if WINUI
        // Only framework elements have a data context on native WinUI (every DependencyObject has one on Uno Platform).
        var condition = new Condition();
        BindingOperations.SetBinding(condition, Condition.BindingProperty, new Binding { Source = new TestViewModel { Name = "ctx" }, Path = new PropertyPath(nameof(TestViewModel.Name)) });
#else
        var condition = new Condition { DataContext = new TestViewModel { Name = "ctx" } };
        BindingOperations.SetBinding(condition, Condition.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Name)) });
#endif

        Assert.Equal("ctx", condition.BindingValue);
        Assert.True(condition.IsSet(Condition.BindingValueProperty));
    }

    [UnoHeadlessFact]
    public void Condition_Evaluates_A_Binding_Object_Assigned_In_Code()
    {
        var condition = new Condition
        {
            Binding = new Binding { Source = new TestViewModel { Name = "source" }, Path = new PropertyPath(nameof(TestViewModel.Name)) },
        };

        Assert.Equal("source", condition.BindingValue);

        condition.Binding = "value";
        Assert.Equal("value", condition.BindingValue);

        condition.Binding = null;
        Assert.Null(condition.BindingValue);
    }
}
