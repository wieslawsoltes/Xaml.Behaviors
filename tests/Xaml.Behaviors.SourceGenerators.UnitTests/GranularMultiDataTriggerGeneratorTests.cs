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
    public partial class TestMultiDataTrigger
    {
        [TriggerProperty]
        private string _value1;

        [TriggerProperty]
        private int _value2;
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
}
