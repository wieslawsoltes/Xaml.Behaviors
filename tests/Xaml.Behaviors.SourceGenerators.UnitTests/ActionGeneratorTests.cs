using System;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ActionGeneratorTests
{
    [AvaloniaFact]
    public void TestMethodAction_Should_Call_Method()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        action.Execute(control, null);
        
        Assert.True(control.MethodCalled);
    }

    [AvaloniaFact]
    public void TestMethodWithParameterAction_Should_Call_Method_With_Parameter()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodWithParameterAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.Parameter = "Test";
        
        action.Execute(control, null);
        
        Assert.True(control.MethodCalled);
        Assert.Equal("Test", control.MethodParameter);
    }

    [Fact]
    public void Assembly_Alias_Attribute_Is_Recognized()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;
using G = Xaml.Behaviors.SourceGenerators.GenerateTypedActionAttribute;

namespace TestNamespace;

public partial class AliasHost
{
    public void Run() { }
}

[assembly: G(typeof(TestNamespace.AliasHost), ""Run"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("AliasHostRunAction", StringComparison.Ordinal));
    }
}
