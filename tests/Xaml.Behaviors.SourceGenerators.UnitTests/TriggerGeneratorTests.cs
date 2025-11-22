using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TriggerGeneratorTests
{
    [AvaloniaFact]
    public void TestEventTrigger_Should_Execute_Actions_On_Event()
    {
        var control = new TestControl();
        dynamic trigger = GeneratedTypeHelper.CreateInstance("TestEventTrigger", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        dynamic action = GeneratedTypeHelper.CreateInstance("TestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        trigger.Actions!.Add(action);
        trigger.Attach(control);
        
        control.RaiseTestEvent();
        
        Assert.True(control.MethodCalled);
    }
}
