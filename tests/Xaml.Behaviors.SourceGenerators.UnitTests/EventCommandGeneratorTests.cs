using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

public class EventCommandGeneratorTests
{
    [Fact]
    public void Should_Generate_EventCommandTrigger_For_Button_Click()
    {
        var source = @"
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("ClickEventCommandTrigger"));
        Assert.Contains(sources, s => s.Contains("CommandProperty"));
    }

    [Fact]
    public void Should_Honor_Assembly_Attribute_ParameterPath()
    {
#if UNO
        // WinUI: RoutedEventArgs.OriginalSource, Click is declared on ButtonBase.
        var source = @"
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = ""OriginalSource"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonBaseClickEventCommandTrigger")));
        Assert.Contains("nameof(ParameterPath), typeof(string), typeof(ButtonBaseClickEventCommandTrigger), new global::Microsoft.UI.Xaml.PropertyMetadata(\"OriginalSource\"", trigger);
        Assert.Contains("TryResolveParameterPath", trigger);
#else
        var source = @"
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = ""Source"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonClickEventCommandTrigger")));
        Assert.Contains("nameof(ParameterPath), \"Source\"", trigger);
        Assert.Contains("TryResolveParameterPath", trigger);
#endif
    }

    [Fact]
    public void Assembly_Attribute_Allows_Constant_ParameterPath()
    {
#if UNO
        // WinUI: RoutedEventArgs.OriginalSource, Click is declared on ButtonBase.
        var source = @"
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = nameof(Microsoft.UI.Xaml.RoutedEventArgs.OriginalSource))]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonBaseClickEventCommandTrigger")));
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(\"OriginalSource\"", trigger);
#else
        var source = @"
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = nameof(Avalonia.Interactivity.RoutedEventArgs.Source))]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonClickEventCommandTrigger")));
        Assert.Contains("nameof(ParameterPath), \"Source\"", trigger);
#endif
    }

    [Fact]
    public void ParameterPath_Can_Target_Base_EventArgs_Property()
    {
#if UNO
        // WinUI RoutedEventArgs has no Handled property: target its OriginalSource.
        var source = @"
using System;
using Microsoft.UI.Xaml;
using Xaml.Behaviors.SourceGenerators;

public class DerivedArgs : RoutedEventArgs { }

public class Host
{
    [GenerateEventCommand(ParameterPath = nameof(RoutedEventArgs.OriginalSource))]
    public event EventHandler<DerivedArgs>? Fired;
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("FiredEventCommandTrigger", StringComparison.Ordinal)));
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(\"OriginalSource\"", trigger);
        Assert.Contains(".OriginalSource", trigger);
#else
        var source = @"
using System;
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

public class DerivedArgs : RoutedEventArgs { }

public class Host
{
    [GenerateEventCommand(ParameterPath = nameof(RoutedEventArgs.Handled))]
    public event EventHandler<DerivedArgs>? Fired;
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("FiredEventCommandTrigger", StringComparison.Ordinal)));
        Assert.Contains("nameof(ParameterPath), \"Handled\"", trigger);
        Assert.Contains(".Handled", trigger);
#endif
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Missing_Event()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class Foo { }
}

[assembly: GenerateEventCommand(typeof(TestNamespace.Foo), ""Missing"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG004");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Invalid_ParameterPath()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public class Host
{
    [GenerateEventCommand(ParameterPath = ""Missing"")]
    public event System.EventHandler? Fired;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG020");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Inaccessible_ParameterPath()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public class Args : EventArgs
{
    private string Secret { get; } = ""hidden"";
}

public class Host
{
    [GenerateEventCommand(ParameterPath = ""Secret"")]
    public event EventHandler<Args>? Fired;
}

";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG021");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Ref_Parameter()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public delegate void RefHandler(ref EventArgs args);

public class Host
{
    [GenerateEventCommand]
    public event RefHandler? Fired;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG029");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Too_Many_Parameters()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public delegate void ThreeParamHandler(object sender, EventArgs args, int value);

public class Host
{
    [GenerateEventCommand]
    public event ThreeParamHandler? Fired;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG034");
    }

    [Fact]
    public void Should_Generate_ParameterPath_Without_Reflection()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public class Payload
{
    public string? Value { get; set; }
}

public class Args : EventArgs
{
    public Payload? Data { get; set; }
}

public class Host
{
    [GenerateEventCommand(ParameterPath = ""Data.Value"")]
    public event EventHandler<Args>? Fired;
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("FiredEventCommandTrigger")));
        Assert.DoesNotContain("GetType().GetProperty", trigger);
        Assert.Contains("TryResolveParameterPath", trigger);
        Assert.Contains("string.Equals(ParameterPath, \"Data.Value\"", trigger);
    }

    [Fact]
    public void ParameterPath_Indexer_Is_Rejected()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public class Args : EventArgs
{
    public string this[int index] => ""value"";
}

public class Host
{
    [GenerateEventCommand(ParameterPath = ""Item"")]
    public event EventHandler<Args>? Fired;
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG020");
    }

    [Fact]
    public void Multiple_EventCommands_With_Different_Options_Are_Distinct()
    {
#if UNO
        // WinUI RoutedEventArgs only has OriginalSource: use the pointer event arguments for a second path.
        var source = @"
using System;
using Microsoft.UI.Xaml.Input;
using Xaml.Behaviors.SourceGenerators;

public class Host
{
    [GenerateEventCommand(Name = ""Shared"", ParameterPath = nameof(PointerRoutedEventArgs.OriginalSource))]
    [GenerateEventCommand(Name = ""Shared"", ParameterPath = nameof(PointerRoutedEventArgs.Pointer), UseDispatcher = true)]
    public event EventHandler<PointerRoutedEventArgs>? Fired;
}
";
#else
        var source = @"
using System;
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

public class Host
{
    [GenerateEventCommand(Name = ""Shared"", ParameterPath = nameof(RoutedEventArgs.Source))]
    [GenerateEventCommand(Name = ""Shared"", ParameterPath = nameof(RoutedEventArgs.RoutedEvent), UseDispatcher = true)]
    public event EventHandler<RoutedEventArgs>? Fired;
}
";
#endif

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);

        var triggerSources = sources
            .Where(s => s.Contains("StyledElementTrigger", StringComparison.Ordinal) && s.Contains("ParameterPathProperty", StringComparison.Ordinal))
            .ToList();

        Assert.True(triggerSources.Count >= 2, "Expected two generated triggers for differing options.");

        var defaults = triggerSources
#if UNO
            .SelectMany(s => Regex.Matches(s, @"nameof\(ParameterPath\), typeof\(string\), typeof\(\w+\), new global::Microsoft\.UI\.Xaml\.PropertyMetadata\((?<val>""[^""]*""|default\(string\?\))").Select(m => m.Groups["val"].Value))
#else
            .SelectMany(s => Regex.Matches(s, @"ParameterPath\),\s*(?<val>""[^""]*""|default\(string\?\))").Select(m => m.Groups["val"].Value))
#endif
            .Distinct(StringComparer.Ordinal)
            .ToList();

#if UNO
        Assert.Contains("\"OriginalSource\"", defaults);
        Assert.Contains("\"Pointer\"", defaults);
#else
        Assert.Contains("\"Source\"", defaults);
        Assert.Contains("\"RoutedEvent\"", defaults);
#endif
    }

    [Fact]
    public void Parameter_Should_Override_ParameterPath_Default()
    {
#if UNO
        // WinUI: RoutedEventArgs.OriginalSource, Click is declared on ButtonBase.
        var source = @"
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = nameof(Microsoft.UI.Xaml.RoutedEventArgs.OriginalSource))]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonBaseClickEventCommandTrigger")));
#else
        var source = @"
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), ""Click"", ParameterPath = nameof(Avalonia.Interactivity.RoutedEventArgs.Source))]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var trigger = Assert.Single(sources.Where(s => s.Contains("ButtonClickEventCommandTrigger")));
#endif
        var parameterCheck = trigger.IndexOf("IsSet(ParameterProperty)", StringComparison.Ordinal);
        var pathCheck = trigger.IndexOf("TryResolveParameterPath", StringComparison.Ordinal);
        Assert.True(parameterCheck >= 0);
        Assert.True(pathCheck >= 0);
        Assert.True(parameterCheck < pathCheck, "Parameter should be considered before resolving ParameterPath.");
    }
}
