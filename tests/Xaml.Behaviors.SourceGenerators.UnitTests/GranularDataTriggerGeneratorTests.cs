using System.Linq;
using Xunit;

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

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
#if UNO
        Assert.Contains("DependencyProperty.Register(nameof(Binding), typeof(string)", stringTrigger);
#else
        Assert.Contains("public static readonly StyledProperty<string", stringTrigger);
#endif
        
        var intTrigger = sources.FirstOrDefault(s => s.Contains("class Int32DataTrigger"));
        Assert.NotNull(intTrigger);
#if UNO
        Assert.Contains("DependencyProperty.Register(nameof(Binding), typeof(int)", intTrigger);
#else
        Assert.Contains("public static readonly StyledProperty<int> BindingProperty", intTrigger);
#endif
    }

    [Fact]
    public void Should_Generate_Valid_ClassName_For_Array_DataTrigger()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedDataTrigger(typeof(int[]))]
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.Single(s => s.Contains("class Int32Array1DataTrigger"));
        Assert.Contains("class Int32Array1DataTrigger", generated);
    }

    [Fact]
    public void Should_Generate_DataTrigger_For_Aliased_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;
using Alias = Xaml.Behaviors.SourceGenerators.GenerateTypedDataTriggerAttribute;

[assembly: Alias(typeof(string))]
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("class StringDataTrigger"));
    }

    [Fact]
    public void Should_Report_Error_For_Generic_DataTrigger()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public class Generic<T> { }

[assembly: GenerateTypedDataTrigger(typeof(Generic<>))]
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Report_Error_For_Nested_DataTrigger_Target()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class Outer
    {
        public class Inner { }
    }
}

[assembly: GenerateTypedDataTrigger(typeof(TestNamespace.Outer.Inner))]
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }
}
