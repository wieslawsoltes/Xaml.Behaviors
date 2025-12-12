using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class InvokeCommandActionGeneratorTests
{
    [AvaloniaFact]
    public void TypedInvokeCommandAction_Should_Execute_Command()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TypedInvokeCommandAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        var command = new TestCommand();
        
        action.Command = command;
        action.CommandParameter = "Param";
        
        action.Execute(control, null);
        
        Assert.True(command.Executed);
        Assert.Equal("Param", command.ExecutedParameter);
    }

    [Fact]
    public void InvokeCommandAction_Inaccessible_Field_Type_Reports_Diagnostic()
    {
        var source = @"
using System;
using System.Windows.Input;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

[GenerateTypedInvokeCommandAction]
public partial class Host : StyledElementAction
{
    private class PrivateCommand : ICommand
    {
#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) { }
    }

    [ActionCommand] private PrivateCommand _command = new();
    [ActionParameter] private object? _parameter;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void InvokeCommandAction_Internal_Field_Type_On_Public_Type_Reports_Diagnostic()
    {
        var source = @"
using System;
using System.Windows.Input;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

internal class InternalCommand : ICommand
{
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) { }
}

[GenerateTypedInvokeCommandAction]
public partial class Host : StyledElementAction
{
    [ActionCommand] private InternalCommand _command = new();
    [ActionParameter] private object? _parameter;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }
}
