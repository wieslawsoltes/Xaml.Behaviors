using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

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
}
