using System.Linq;
using Xunit;

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

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
    public partial class TestInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
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
#if UNO
        Assert.Contains("DependencyProperty.Register(nameof(Command), typeof(global::System.Windows.Input.ICommand)", generated);
        Assert.Contains("DependencyProperty.Register(nameof(CommandParameter), typeof(object)", generated);
#else
        Assert.Contains("public static readonly StyledProperty<global::System.Windows.Input.ICommand> CommandProperty", generated);
        Assert.Contains("public static readonly StyledProperty<object", generated);
#endif
        Assert.Contains("if (command.CanExecute(this._commandParameter))", generated);
    }

    [Fact]
    public void Should_Dispatch_Command_When_Requested()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction(UseDispatcher = true)]
    public partial class TestInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
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
#if UNO
        Assert.Contains("PostOnUIThread(() =>", generated);
#else
        Assert.Contains("Dispatcher.UIThread.Post(() =>", generated);
#endif
        Assert.Contains("command.Execute(this._commandParameter);", generated);
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
    public partial class TestInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestInvokeCommandAction"));
        Assert.NotNull(generated);
#if UNO
        Assert.Contains("DependencyProperty.Register(nameof(Command), typeof(global::System.Windows.Input.ICommand)", generated);
#else
        Assert.Contains("public static readonly StyledProperty<global::System.Windows.Input.ICommand> CommandProperty", generated);
#endif
        Assert.DoesNotContain("CommandParameterProperty", generated);
        Assert.Contains("if (command.CanExecute(parameter))", generated);
    }

    [Fact]
    public void Should_Report_Error_When_Target_Not_StyledElementAction()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class InvalidInvokeCommandAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

#if UNO
        // WinUI reports the invalid base type with XBG036.
        Assert.Contains(diagnostics, d => d.Id == "XBG036");
#else
        Assert.Contains(diagnostics, d => d.Id == "XBG012");
#endif
    }

    [Fact]
    public void Should_Report_Error_When_Class_Not_Partial()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public class InvalidInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG016");
    }

    [Fact]
    public void Should_Report_Error_For_Generic_InvokeCommandAction()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class GenericInvokeCommandAction<T> : Avalonia.Xaml.Interactivity.StyledElementAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Use_Target_Accessibility()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    internal partial class InternalInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
    {
        [ActionCommand]
        private ICommand _command;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class InternalInvokeCommandAction"));
        Assert.NotNull(generated);
        Assert.Contains("internal partial class InternalInvokeCommandAction", generated);
    }

    [Fact]
    public void Should_Report_Error_For_Readonly_Command_Field()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class ReadonlyCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
    {
        [ActionCommand]
        private readonly ICommand _command;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG032");
    }

    [Fact]
    public void Should_Report_Error_For_Nested_Type()
    {
        var source = @"
using System.Windows.Input;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Outer
    {
        [GenerateTypedInvokeCommandAction]
        public partial class Inner : Avalonia.Xaml.Interactivity.StyledElementAction
        {
            [ActionCommand]
            private ICommand _command;
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG018");
    }

    [Fact]
    public void Should_Report_Error_When_Command_Field_Missing()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedInvokeCommandAction]
    public partial class InvalidInvokeCommandAction : Avalonia.Xaml.Interactivity.StyledElementAction
    {
    }
}";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG030");
    }
}
