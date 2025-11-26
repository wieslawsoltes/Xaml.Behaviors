using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class MultiDataTriggerGeneratorTests
{
    [AvaloniaFact]
    public void TypedMultiDataTrigger_Should_Execute_Actions_When_Conditions_Met()
    {
        var control = new TestControl();
        var trigger = new TypedMultiDataTrigger();
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlTestMethodAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        
        trigger.Actions!.Add(action);
        trigger.Attach(control);
        
        trigger.Value1 = "A";
        trigger.Value2 = "B";
        
        Assert.True(control.MethodCalled);
    }

    [Fact]
    public void Should_Report_Error_For_Readonly_Field()
    {
        var source = @"
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public partial class ReadonlyTrigger : StyledElementTrigger
    {
        [TriggerProperty]
        private readonly bool _flag;

        private bool Evaluate() => _flag;
    }
}
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG032");
    }

    [Fact]
    public void Should_Report_Error_For_Static_Field()
    {
        var source = @"
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public partial class StaticTrigger : StyledElementTrigger
    {
        [TriggerProperty]
        private static bool _flag;

        private bool Evaluate() => _flag;
    }
}
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG010");
    }

    [Fact]
    public void Should_Report_Error_When_Type_Is_Not_Partial()
    {
        var source = @"
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

[GenerateTypedMultiDataTrigger]
public class NonPartialTrigger : StyledElementTrigger
{
    [TriggerProperty]
    private bool _flag;

    private bool Evaluate() => _flag;
}
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG016");
    }
}
