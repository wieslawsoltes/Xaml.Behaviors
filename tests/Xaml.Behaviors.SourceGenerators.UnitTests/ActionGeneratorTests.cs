using Avalonia.Headless.XUnit;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ActionGeneratorTests
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
}
