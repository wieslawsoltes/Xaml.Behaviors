using System.Linq;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class PropertyTriggerGeneratorTests
{
    [Fact]
    public void Should_Generate_PropertyTrigger_For_StyledProperty()
    {
        var source = @"
using Avalonia;
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class TestControl : Control
    {
        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<TestControl, int>(nameof(Count));

        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
    }
}

[assembly: GeneratePropertyTrigger(typeof(TestNamespace.TestControl), ""CountProperty"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("CountPropertyTrigger"));
        Assert.Contains(sources, s => s.Contains("ComparisonConditionProperty"));
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Non_Avalonia_Property_Field()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class BadControl
    {
        public static readonly string NotAProperty = ""nope"";
    }
}

[assembly: GeneratePropertyTrigger(typeof(TestNamespace.BadControl), ""NotAProperty"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG019");
    }
}
