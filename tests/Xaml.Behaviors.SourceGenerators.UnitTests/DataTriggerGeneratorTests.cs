using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class DataTriggerGeneratorTests
{
    [AvaloniaFact]
    public void StringDataTrigger_Should_Execute_Actions_When_Condition_Met()
    {
        var control = new TestControl();
        var trigger = new StringDataTrigger();
        var action = new TestMethodAction();
        
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
}
