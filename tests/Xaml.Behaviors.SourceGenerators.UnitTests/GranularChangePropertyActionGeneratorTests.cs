using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularChangePropertyActionGeneratorTests
{
    [Fact]
    public void Should_Generate_Action_For_Property_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedChangePropertyAction]
        public string TestProperty { get; set; }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class SetTestPropertyAction"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
        Assert.Contains("public static readonly StyledProperty<string> ValueProperty", generated);
        Assert.Contains("typedTarget.TestProperty = Value;", generated);
    }

    [Fact]
    public void Should_Generate_Action_For_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedChangePropertyAction(typeof(TestNamespace.TestClass), ""TestProperty"")]

namespace TestNamespace
{
    public class TestClass
    {
        public string TestProperty { get; set; }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class SetTestPropertyAction"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
    }

    [Fact]
    public void Should_Report_Error_If_Property_Not_Found_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedChangePropertyAction(typeof(TestNamespace.TestClass), ""MissingProperty"")]

namespace TestNamespace
{
    public class TestClass
    {
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        var errorFile = sources.FirstOrDefault(s => s.Contains("/* Error: Property MissingProperty not found on TestClass */"));
        Assert.NotNull(errorFile);
    }
}
