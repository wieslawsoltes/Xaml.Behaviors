using Avalonia.Headless.XUnit;
using Xaml.Behaviors.Generated;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class InvokeCommandActionGeneratorTests
{
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
