// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Core;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.UnitTests;

public sealed class TestViewModel : INotifyPropertyChanged
{
    private int _count;
    private string? _name;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Count
    {
        get => _count;
        set { _count = value; OnPropertyChanged(); }
    }

    public string? Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public void Increment() => Count++;

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RecordingCommand : ICommand
{
    public List<object?> Parameters { get; } = [];

    public event System.EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => Parameters.Add(parameter);
}

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Parameters { get; } = [];

    public DependencyObject? ObservedHost => Host;

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

public partial class RecordingStyledAction : StyledElementAction
{
    public List<object?> Parameters { get; } = [];

    public DependencyObject? ObservedHost => Host;

    public override object? Execute(object? sender, object? parameter)
    {
        Parameters.Add(parameter);
        return null;
    }
}

public class InteractionsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static void Click(Button button) => new ButtonAutomationPeer(button).Invoke();

    [UnoHeadlessFact]
    public async Task EventTriggerBehavior_Click_Calls_Method()
    {
        var vm = new TestViewModel();
        var button = new Button();
        var trigger = new EventTriggerBehavior { EventName = "Click" };
        trigger.Actions!.Add(new CallMethodAction { TargetObject = vm, MethodName = nameof(TestViewModel.Increment) });
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        Click(button);
        Click(button);

        Assert.Equal(2, vm.Count);
    }

    [UnoHeadlessFact]
    public async Task ChangePropertyAction_Converts_String_Values()
    {
        var button = new Button();
        var target = new Border();
        var trigger = new EventTriggerBehavior { EventName = "Click" };
        trigger.Actions!.Add(new ChangePropertyAction { TargetObject = target, PropertyName = "Width", Value = "42" });
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(new StackPanel { Children = { button, target } });

        Click(button);

        Assert.Equal(42d, target.Width);
    }

    [UnoHeadlessFact]
    public void ChangePropertyAction_Reverts_Reversible_Execution()
    {
        var target = new Border { Width = 10 };
        var action = new ChangePropertyAction { TargetObject = target, PropertyName = "Width", Value = 20d };

        action.ExecuteReversibly(target, null);
        Assert.Equal(20d, target.Width);

        action.Revert(target, null);
        Assert.Equal(10d, target.Width);
    }

    [UnoHeadlessFact]
    public async Task InvokeCommandAction_Passes_Command_Parameter()
    {
        var command = new RecordingCommand();
        var button = new Button();
        var trigger = new EventTriggerBehavior { EventName = "Click" };
        trigger.Actions!.Add(new InvokeCommandAction { Command = command, CommandParameter = "p" });
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        Click(button);

        Assert.Equal(["p"], command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task InvokeCommandAction_Binds_Command_From_DataContext()
    {
        var command = new RecordingCommand();
        var button = new Button { DataContext = command };
        var trigger = new EventTriggerBehavior { EventName = "Click" };
        var action = new InvokeCommandAction();
        BindingOperations.SetBinding(action, InvokeCommandActionBase.CommandProperty, new Binding());
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        Click(button);

        Assert.Single(command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task DataTriggerBehavior_Executes_When_Bound_Value_Matches()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var trigger = new DataTriggerBehavior { Value = 3, ComparisonCondition = ComparisonConditionType.Equal };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        BindingOperations.SetBinding(trigger, DataTriggerBehavior.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) });
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        action.Parameters.Clear();

        vm.Count = 1;
        await Session.WaitForIdleAsync();
        Assert.Empty(action.Parameters);

        vm.Count = 3;
        await Session.WaitForIdleAsync();
        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task MultiDataTriggerBehavior_Conditions_Inherit_DataContext()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var trigger = new MultiDataTriggerBehavior();
        var countCondition = new Condition { Value = 2, ComparisonCondition = ComparisonConditionType.Equal };
        var nameCondition = new Condition { Value = "ok", ComparisonCondition = ComparisonConditionType.Equal };
        BindingOperations.SetBinding(countCondition, Condition.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) });
        BindingOperations.SetBinding(nameCondition, Condition.BindingProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Name)) });
        trigger.Conditions!.Add(countCondition);
        trigger.Conditions.Add(nameCondition);
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        action.Parameters.Clear();

        vm.Count = 2;
        await Session.WaitForIdleAsync();
        Assert.Empty(action.Parameters);

        vm.Name = "ok";
        await Session.WaitForIdleAsync();
        Assert.NotEmpty(action.Parameters);
        Assert.Equal(2, countCondition.BindingValue);
    }

    [UnoHeadlessFact]
    public async Task DebounceAction_Runs_Nested_Actions_With_Host()
    {
        var border = new Border();
        var trigger = new EventTriggerBehavior();
        var debounce = new DebounceAction { Delay = System.TimeSpan.FromMilliseconds(20) };
        var nested = new RecordingStyledAction();
        debounce.Actions!.Add(nested);
        trigger.Actions!.Add(debounce);
        Interaction.GetBehaviors(border).Add(trigger);

        await Session.ShowAsync(border);
        await Task.Delay(200);
        await Session.WaitForIdleAsync();

        Assert.Single(nested.Parameters);
        Assert.Same(border, nested.ObservedHost);
    }

    [UnoHeadlessFact]
    public async Task TimerTrigger_Executes_Actions_On_Ticks()
    {
        var border = new Border();
        var trigger = new TimerTrigger { MillisecondsPerTick = 10, TotalTicks = 2 };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);

        await Session.ShowAsync(border);
        await Task.Delay(300);
        await Session.WaitForIdleAsync();

        Assert.Equal(2, action.Parameters.Count);
    }
}
