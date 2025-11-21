using Avalonia.Headless.XUnit;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TriggerGeneratorTests
{
    [AvaloniaFact]
    public void TestEventTrigger_Should_Execute_Actions_On_Event()
    {
        var control = new TestControl();
        var trigger = new TestEventTrigger();
        var action = new TestMethodAction();
        
        trigger.Actions!.Add(action);
        trigger.Attach(control);
        
        control.RaiseTestEvent();
        
        Assert.True(control.MethodCalled);
    }
}
