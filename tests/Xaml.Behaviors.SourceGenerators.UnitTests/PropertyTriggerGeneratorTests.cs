using System;
using System.Linq;
using System.Text.RegularExpressions;
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

    [Fact]
    public void Should_Not_Warn_SourceName_For_Logical_Control()
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
    }
}

[assembly: GeneratePropertyTrigger(typeof(TestNamespace.TestControl), ""CountProperty"", SourceName = ""source"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.DoesNotContain(diagnostics, d => d.Id == "XBG022");
    }

    [Fact]
    public void Should_Warn_SourceName_For_NonLogical_Type()
    {
        var source = @"
using Avalonia;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class PlainAvaloniaObject : Avalonia.AvaloniaObject
    {
        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<PlainAvaloniaObject, int>(nameof(Count));

        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
    }
}

[assembly: GeneratePropertyTrigger(typeof(TestNamespace.PlainAvaloniaObject), ""CountProperty"", SourceName = ""source"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG022");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Generic_Containing_Type()
    {
        var source = @"
using Avalonia;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class GenericControl<T> : Avalonia.AvaloniaObject
    {
        [GeneratePropertyTrigger]
        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<GenericControl<T>, int>(nameof(Count));

        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Generate_Multiple_PropertyTriggers_From_Multiple_Attributes()
    {
        var source = @"
using Avalonia;
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class TestControl : Control
    {
        [GeneratePropertyTrigger(Name = ""FirstCountTrigger"")]
        [GeneratePropertyTrigger(Name = ""SecondCountTrigger"", UseDispatcher = true)]
        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<TestControl, int>(nameof(Count));

        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("FirstCountTrigger", StringComparison.Ordinal));
        Assert.Contains(sources, s => s.Contains("SecondCountTrigger", StringComparison.Ordinal));
    }

    [Fact]
    public void PropertyTriggers_With_Different_Dispatcher_Settings_Are_Distinct()
    {
        var source = @"
using Avalonia;
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class TestControl : Control
    {
        [GeneratePropertyTrigger(Name = ""CountTrigger"")]
        [GeneratePropertyTrigger(Name = ""CountTrigger"", UseDispatcher = true)]
        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<TestControl, int>(nameof(Count));

        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);

        var classNames = sources
            .SelectMany(s => Regex.Matches(s, @"class\s+(?<name>\w+)\s*:\s*Avalonia\.Xaml\.Interactivity\.StyledElementTrigger")
                .Select(m => m.Groups["name"].Value))
            .Where(n => n.Contains("CountTrigger", StringComparison.Ordinal))
            .Distinct()
            .ToList();

        Assert.True(classNames.Count >= 2, "Expected distinct CountTrigger classes for differing UseDispatcher values.");
    }
}
