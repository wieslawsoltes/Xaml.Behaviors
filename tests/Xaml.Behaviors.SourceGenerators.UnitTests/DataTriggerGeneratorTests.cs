using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class DataTriggerGeneratorTests
{
    [AvaloniaFact]
    public void StringDataTrigger_Should_Execute_Actions_When_Condition_Met()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("StringDataTrigger", "Xaml.Behaviors.Generated");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        trigger.Actions!.Add(action);
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
    public void StringDataTrigger_Should_Evaluate_On_Attach_When_Matching()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("StringDataTrigger", "Xaml.Behaviors.Generated");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");

        trigger.Actions!.Add(action);
        trigger.Binding = "Match";
        trigger.Value = "Match";
        trigger.ComparisonCondition = ComparisonConditionType.Equal;

        trigger.Attach(control);

        Assert.True(control.MethodCalled);
    }
}
