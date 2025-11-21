using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TestCommand : System.Windows.Input.ICommand
{
    public bool CanExecuteValue { get; set; } = true;
    public bool Executed { get; private set; }
    public object? ExecutedParameter { get; private set; }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => CanExecuteValue;

    public void Execute(object? parameter)
    {
        Executed = true;
        ExecutedParameter = parameter;
    }
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class SourceGeneratorTests
{
    [AvaloniaFact]
    public void TestMethodAction_Should_Call_Method()
    {
        var control = new TestControl();
        var action = new TestMethodAction();
        
        action.Execute(control, null);
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void TestMethodWithParameterAction_Should_Call_Method_With_Parameter()
    {
        var control = new TestControl();
        var action = new TestMethodWithParameterAction();
        action.Parameter = "Test";
        
        action.Execute(control, null);
        
        Assert.True(control.MethodCalled);
        Assert.Equal("Test", control.MethodParameter);
    }

    [AvaloniaFact]
    public void TestEventTrigger_Should_Execute_Actions_On_Event()
    {
        var control = new TestControl();
        var trigger = new TestEventTrigger();
        var action = new TestMethodAction();
        
        trigger.Actions.Add(action);
        trigger.Attach(control);
        
        control.RaiseTestEvent();
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void SetTagAction_Should_Set_Property()
    {
        var control = new TestControl();
        var action = new SetTagAction();
        action.Value = "TagValue";
        
        action.Execute(control, null);
        
        Assert.Equal("TagValue", control.Tag);
    }

    [AvaloniaFact]
    public void StringDataTrigger_Should_Execute_Actions_When_Condition_Met()
    {
        var control = new TestControl();
        var trigger = new StringDataTrigger();
        var action = new TestMethodAction();
        
        trigger.Actions.Add(action);
        trigger.Binding = "Match";
        trigger.Value = "Match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;
        
        trigger.Attach(control);
        
        // Trigger logic runs on property change.
        trigger.Binding = "NoMatch";
        trigger.Binding = "Match";
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void TypedMultiDataTrigger_Should_Execute_Actions_When_Conditions_Met()
    {
        var control = new TestControl();
        var trigger = new TypedMultiDataTrigger();
        var action = new TestMethodAction();
        
        trigger.Actions.Add(action);
        trigger.Attach(control);
        
        trigger.Value1 = "A";
        trigger.Value2 = "B";
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void TypedInvokeCommandAction_Should_Execute_Command()
    {
        var control = new TestControl();
        var action = new TypedInvokeCommandAction();
        var command = new TestCommand();
        
        action.Command = command;
        action.CommandParameter = "Param";
        
        action.Execute(control, null);
        
        Assert.True(command.Executed);
        Assert.Equal("Param", command.ExecutedParameter);
    }
}
