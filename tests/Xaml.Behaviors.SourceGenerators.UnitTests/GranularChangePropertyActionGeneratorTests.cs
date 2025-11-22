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
        Assert.Contains("public static readonly StyledProperty<string", generated);
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

        Assert.Contains(diagnostics, d => d.Id == "XBG005");
    }

    [Fact]
    public void Should_Report_Error_For_Generic_Property_Action()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass<T>
    {
        [GenerateTypedChangePropertyAction]
        public T TestProperty { get; set; }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Report_Error_For_Static_Property_Action()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedChangePropertyAction]
        public static string TestProperty { get; set; }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG010");
    }

    [Fact]
    public void Should_Report_Error_For_Inaccessible_Setter()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedChangePropertyAction]
        public string TestProperty { get; }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG015");
    }

    [Fact]
    public void Should_Report_Error_For_InitOnly_Setter()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedChangePropertyAction]
        public string TestProperty { get; init; }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG017");
    }

    [Fact]
    public void Should_Report_Error_For_InitOnly_Setter_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedChangePropertyAction(typeof(TestNamespace.TestClass), ""TestProperty"")]

namespace TestNamespace
{
    public class TestClass
    {
        public string TestProperty { get; init; }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG017");
    }

    [Fact]
    public void Should_Report_Error_When_Containing_Type_Not_Accessible()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    internal class Container
    {
        private class Hidden
        {
            [GenerateTypedChangePropertyAction]
            public string TestProperty { get; set; }
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void Should_Report_Error_For_Nested_Target()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Outer
    {
        public partial class Inner
        {
            [GenerateTypedChangePropertyAction]
            public string TestProperty { get; set; }
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }
}
