using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularMultiDataTriggerGeneratorTests
{
    [Fact]
    public void Should_Generate_MultiDataTrigger_For_Class_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public partial class TestMultiDataTrigger : Avalonia.Xaml.Interactivity.StyledElementTrigger
    {
        [TriggerProperty]
        private string _value1;

        [TriggerProperty]
        private int _value2;

        public bool Evaluate() => true;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMultiDataTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<string", generated);
        Assert.Contains("public static readonly StyledProperty<int> Value2Property", generated);
        Assert.Contains("if (change.Property == Value1Property || change.Property == Value2Property)", generated);
    }

    [Fact]
    public void Should_Report_Error_When_Target_Not_StyledElementTrigger()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public partial class InvalidTrigger
    {
        [TriggerProperty]
        private string _value1;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG011");
    }

    [Fact]
    public void Should_Report_Error_When_Evaluate_Missing()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public partial class InvalidTrigger : Avalonia.Xaml.Interactivity.StyledElementTrigger
    {
        [TriggerProperty]
        private string _value1;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG013");
    }

    [Fact]
    public void Should_Report_Error_When_Class_Not_Partial()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    public class InvalidTrigger : Avalonia.Xaml.Interactivity.StyledElementTrigger
    {
        [TriggerProperty]
        private string _value1;

        public bool Evaluate() => true;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG016");
    }

    [Fact]
    public void Should_Use_Target_Accessibility()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    [GenerateTypedMultiDataTrigger]
    internal partial class InternalTrigger : Avalonia.Xaml.Interactivity.StyledElementTrigger
    {
        [TriggerProperty]
        private string _value1;

        public bool Evaluate() => true;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class InternalTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("internal partial class InternalTrigger", generated);
    }

    [Fact]
    public void Should_Report_Error_For_Nested_Type()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Outer
    {
        [GenerateTypedMultiDataTrigger]
        public partial class Inner : Avalonia.Xaml.Interactivity.StyledElementTrigger
        {
            [TriggerProperty]
            private string _value1;

            public bool Evaluate() => true;
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG018");
    }
}
