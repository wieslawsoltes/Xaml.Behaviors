using Avalonia.Headless.XUnit;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class MultiDataTriggerGeneratorTests
{
    [AvaloniaFact]
    public void TypedMultiDataTrigger_Should_Execute_Actions_When_Conditions_Met()
    {
        var control = new TestControl();
        var trigger = new TypedMultiDataTrigger();
        var action = new TestMethodAction();
        
        trigger.Actions!.Add(action);
        trigger.Attach(control);
        
        trigger.Value1 = "A";
        trigger.Value2 = "B";
        
        Assert.True(control.MethodCalled);
    }
}
