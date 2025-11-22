using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularDataTriggerGeneratorTests
{
    [Fact]
    public void Should_Generate_DataTrigger_For_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedDataTrigger(typeof(string))]
[assembly: GenerateTypedDataTrigger(typeof(int))]
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        
        var stringTrigger = sources.FirstOrDefault(s => s.Contains("class StringDataTrigger"));
        Assert.True(stringTrigger is not null, "Sources: " + string.Join("\n----\n", sources));
        Assert.Contains("public static readonly StyledProperty<string", stringTrigger);
        
        var intTrigger = sources.FirstOrDefault(s => s.Contains("class Int32DataTrigger"));
        Assert.NotNull(intTrigger);
        Assert.Contains("public static readonly StyledProperty<int> BindingProperty", intTrigger);
    }
}
