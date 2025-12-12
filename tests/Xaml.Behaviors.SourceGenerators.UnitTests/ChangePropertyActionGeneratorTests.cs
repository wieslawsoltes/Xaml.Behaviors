using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class ChangePropertyActionGeneratorTests
{
    [AvaloniaFact]
    public void SetTagAction_Should_Set_Property()
    {
        var control = new TestControl();
        dynamic action = GeneratedTypeHelper.CreateInstance("TestControlSetTagAction", "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests");
        action.Value = "TagValue";
        
        action.Execute(control, null);
        
        Assert.Equal("TagValue", control.Tag);
    }

    [Fact]
    public void ChangePropertyAction_Should_Handle_Global_Namespace()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public class GlobalViewModel
{
    [GenerateTypedChangePropertyAction]
    public string Name { get; set; } = string.Empty;
}
";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public void ChangePropertyActions_With_Different_Dispatcher_Flags_Are_Distinct()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public class Host
{
    public string Name { get; set; } = string.Empty;
}

[assembly: GenerateTypedChangePropertyAction(typeof(Host), ""Name"")]
[assembly: GenerateTypedChangePropertyAction(typeof(Host), ""Name"", UseDispatcher = true)]
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);

        var classNames = sources
            .SelectMany(s => Regex.Matches(s, @"partial class (\w+)").Select(m => m.Groups[1].Value))
            .Where(n => n.Contains("NameAction", StringComparison.Ordinal))
            .Distinct()
            .ToList();

        Assert.Equal(2, classNames.Count);
    }
}
