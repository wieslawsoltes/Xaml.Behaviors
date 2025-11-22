using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class InvokeCommandActionGeneratorTests
{
    [AvaloniaFact]
    public void TypedInvokeCommandAction_Should_Execute_Command()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TypedInvokeCommandAction");
        var command = new TestCommand();
        
        action.Command = command;
        action.CommandParameter = "Param";
        
        action.Execute(control, null);
        
        Assert.True(command.Executed);
        Assert.Equal("Param", command.ExecutedParameter);
    }
}
