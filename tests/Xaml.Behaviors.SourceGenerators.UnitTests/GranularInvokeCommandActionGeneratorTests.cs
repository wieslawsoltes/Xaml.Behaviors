using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularInvokeCommandActionGeneratorTests
{
    [Fact]
    public void Should_Generate_InvokeCommandAction_For_Class_Attribute()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class TestInvokeCommandAction
    {
        [ActionCommand]
        private ICommand _command;

        [ActionParameter]
        private object _commandParameter;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestInvokeCommandAction"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<global::System.Windows.Input.ICommand> CommandProperty", generated);
        Assert.Contains("public static readonly StyledProperty<object> CommandParameterProperty", generated);
        Assert.Contains("if (command.CanExecute(this._commandParameter))", generated);
    }

    [Fact]
    public void Should_Generate_InvokeCommandAction_Without_Parameter()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class TestInvokeCommandAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestInvokeCommandAction"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<global::System.Windows.Input.ICommand> CommandProperty", generated);
        Assert.DoesNotContain("CommandParameterProperty", generated);
        Assert.Contains("if (command.CanExecute(parameter))", generated);
    }
}
