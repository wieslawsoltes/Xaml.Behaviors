// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Behaviors.Generated;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Behaviors.SourceGenerators.UnitTests.WinUI;

/// <summary>
/// Runtime tests of the WinUI code emitted by <c>Xaml.Behaviors.SourceGenerators</c> on Uno Platform (headless).
/// </summary>
public class GeneratedRuntimeTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void Typed_Action_Invokes_Method_With_Property_Arguments()
    {
        var viewModel = new CounterViewModel();
        var action = new IncrementAction { TargetObject = viewModel, Step = 3 };

        var result = action.Execute(null, null);

        Assert.Equal(true, result);
        Assert.Equal(3, viewModel.Count);
        Assert.Equal(3, action.GetValue(IncrementAction.StepProperty));
    }

    [UnoHeadlessFact]
    public void Typed_Action_Uses_Sender_When_TargetObject_Is_Not_Set()
    {
        var viewModel = new CounterViewModel();
        var action = new IncrementAction { Step = 2 };

        Assert.Equal(true, action.Execute(viewModel, null));
        Assert.Equal(false, action.Execute(new object(), null));
        Assert.Equal(2, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task Typed_Action_With_Dispatcher_Posts_To_UI_Thread()
    {
        var viewModel = new CounterViewModel { Count = 5 };
        var action = new ResetAction { TargetObject = viewModel };

        Assert.Equal(true, action.Execute(null, null));
        Assert.Equal(5, viewModel.Count);

        await Session.WaitForIdleAsync();

        Assert.Equal(0, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task Awaitable_Action_Tracks_IsExecuting()
    {
        var viewModel = new CounterViewModel();
        var action = new LoadAsyncAction { TargetObject = viewModel, Item = "page" };

        Assert.Equal(true, action.Execute(null, null));
        Assert.True(action.IsExecuting);

        for (var i = 0; i < 20 && action.IsExecuting; i++)
        {
            await Task.Delay(10);
            await Session.WaitForIdleAsync();
        }

        Assert.False(action.IsExecuting);
        Assert.Null(action.LastError);
        Assert.Equal(["page"], viewModel.Log);
    }

    [UnoHeadlessFact]
    public async Task Typed_Trigger_On_Button_Click_Executes_Actions()
    {
        var viewModel = new CounterViewModel();
        var button = new Button();
        var trigger = new ButtonBaseClickTrigger();
        trigger.Actions!.Add(new IncrementAction { TargetObject = viewModel, Step = 1 });
        var recorder = new RecordingAction();
        trigger.Actions.Add(recorder);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        new ButtonAutomationPeer(button).Invoke();
        new ButtonAutomationPeer(button).Invoke();

        Assert.Equal(2, viewModel.Count);
        Assert.Equal(2, recorder.Parameters.Count);
        Assert.IsType<RoutedEventArgs>(recorder.Parameters[0], exactMatch: false);

        Interaction.GetBehaviors(button).Remove(trigger);
        new ButtonAutomationPeer(button).Invoke();

        Assert.Equal(2, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task Typed_Trigger_Follows_SourceObject()
    {
        var viewModel = new CounterViewModel();
        var host = new Border();
        var first = new Button();
        var second = new Button();
        var trigger = new ButtonBaseClickTrigger { SourceObject = first };
        trigger.Actions!.Add(new IncrementAction { TargetObject = viewModel, Step = 1 });
        Interaction.GetBehaviors(host).Add(trigger);
        await Session.ShowAsync(host);

        new ButtonAutomationPeer(first).Invoke();
        trigger.SourceObject = second;
        new ButtonAutomationPeer(first).Invoke();
        new ButtonAutomationPeer(second).Invoke();

        Assert.Equal(2, viewModel.Count);
    }

    [UnoHeadlessFact]
    public void ChangePropertyAction_Sets_And_Reverts_Framework_Property()
    {
        var textBlock = new TextBlock { Text = "before" };
        var action = new TextBlockSetTextAction { Value = "after" };

        Assert.Equal(true, action.Execute(textBlock, null));
        Assert.Equal("after", textBlock.Text);

        textBlock.Text = "before";
        Assert.Equal(true, action.ExecuteReversibly(textBlock, null));
        Assert.Equal("after", textBlock.Text);
        Assert.Equal(true, action.Revert(textBlock, null));
        Assert.Equal("before", textBlock.Text);
    }

    [UnoHeadlessFact]
    public async Task ChangePropertyAction_With_Dispatcher_Sets_Value_On_UI_Thread()
    {
        var viewModel = new CounterViewModel();
        var action = new SetCountAction { TargetObject = viewModel, Value = 7 };

        Assert.Equal(true, action.Execute(null, null));
        await Session.WaitForIdleAsync();
        Assert.Equal(7, viewModel.Count);

        viewModel.Count = 1;
        Assert.Equal(true, action.ExecuteReversibly(null, null));
        Assert.Equal(7, viewModel.Count);
        Assert.Equal(true, action.Revert(null, null));
        Assert.Equal(1, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task ChangePropertyAction_With_Dispatcher_Invokes_On_UI_Thread_From_Background_Thread()
    {
        var viewModel = new CounterViewModel { Count = 1 };
        var action = new SetCountAction { TargetObject = viewModel, Value = 9 };

        var applied = await Task.Run(() => action.ExecuteReversibly(null, null));

        Assert.Equal(true, applied);
        Assert.Equal(9, viewModel.Count);

        var reverted = await Task.Run(() => action.Revert(null, null));

        Assert.Equal(true, reverted);
        Assert.Equal(1, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task DataTrigger_Executes_Actions_When_Condition_Matches()
    {
        var border = new Border();
        var recorder = new RecordingAction();
        var trigger = new Int32DataTrigger { Binding = 1, Value = 5, ComparisonCondition = ComparisonConditionType.GreaterThanOrEqual };
        trigger.Actions!.Add(recorder);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        Assert.Empty(recorder.Parameters);

        trigger.Binding = 5;
        Assert.Single(recorder.Parameters);

        trigger.Binding = 10;
        Assert.Equal(2, recorder.Parameters.Count);

        trigger.ComparisonCondition = ComparisonConditionType.Equal;
        Assert.Equal(2, recorder.Parameters.Count);
    }

    [UnoHeadlessFact]
    public async Task MultiDataTrigger_Evaluates_Generated_Properties()
    {
        var border = new Border();
        var recorder = new RecordingAction();
        var trigger = new BothPositiveTrigger();
        trigger.Actions!.Add(recorder);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        trigger.First = 1;
        Assert.Empty(recorder.Parameters);

        trigger.Second = 2;
        Assert.Single(recorder.Parameters);
    }

    [UnoHeadlessFact]
    public async Task PropertyTrigger_Observes_Framework_DependencyProperty()
    {
        var textBox = new TextBox();
        var recorder = new RecordingAction();
        var trigger = new TextBoxTextPropertyTrigger { Value = "go" };
        trigger.Actions!.Add(recorder);
        Interaction.GetBehaviors(textBox).Add(trigger);
        await Session.ShowAsync(textBox);

        textBox.Text = "wait";
        Assert.Empty(recorder.Parameters);

        textBox.Text = "go";
        Assert.Single(recorder.Parameters);

        Interaction.GetBehaviors(textBox).Remove(trigger);
        textBox.Text = "stop";
        textBox.Text = "go";
        Assert.Single(recorder.Parameters);
    }

    [UnoHeadlessFact]
    public async Task PropertyTrigger_Resolves_SourceName_Through_Name_Scope()
    {
        var target = new TextBox { Name = "target" };
        var panel = new StackPanel();
        panel.Children.Add(target);
        var recorder = new RecordingAction();
        var trigger = new TextBoxTextPropertyTrigger { Value = "named", SourceName = "target" };
        trigger.Actions!.Add(recorder);
        await Session.ShowAsync(panel);
        Interaction.GetBehaviors(panel).Add(trigger);

        target.Text = "named";

        Assert.Single(recorder.Parameters);
    }

    [UnoHeadlessFact]
    public async Task PropertyTrigger_Observes_NotifyPropertyChanged_Source()
    {
        var viewModel = new CounterViewModel();
        var border = new Border();
        var recorder = new RecordingAction();
        var trigger = new CountPropertyTrigger { SourceObject = viewModel, Value = 2 };
        trigger.Actions!.Add(recorder);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        viewModel.Count = 1;
        Assert.Empty(recorder.Parameters);

        viewModel.Count = 2;
        Assert.Single(recorder.Parameters);

        trigger.SourceObject = null;
        viewModel.Count = 3;
        viewModel.Count = 2;
        Assert.Single(recorder.Parameters);
    }

    [UnoHeadlessFact]
    public void InvokeCommandAction_Executes_Command_With_Parameter()
    {
        var command = new TestCommand();
        var action = new RunCommandAction { Command = command, Parameter = "p" };

        Assert.Equal(true, action.Execute(null, null));
        Assert.Equal(["p"], command.Executed);

        action.Parameter = null;
        action.Command = null;
        Assert.Equal(false, action.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task EventCommand_Executes_Command_On_Button_Click()
    {
        var command = new TestCommand();
        var button = new Button();
        var trigger = new ButtonBaseClickEventCommandTrigger { Command = command };
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        new ButtonAutomationPeer(button).Invoke();

        var parameter = Assert.Single(command.Executed);
        Assert.Same(button, parameter);

        trigger.Parameter = "explicit";
        new ButtonAutomationPeer(button).Invoke();

        Assert.Equal("explicit", command.Executed[1]);
    }
}
