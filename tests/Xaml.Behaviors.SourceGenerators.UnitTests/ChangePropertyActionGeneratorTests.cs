using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
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

    [AvaloniaFact]
    public void SetTagAction_Should_Revert_Property()
    {
        var control = new TestControl { Tag = "Original" };
        var action = Assert.IsAssignableFrom<IReversibleAction>(
            GeneratedTypeHelper.CreateInstance(
                "TestControlSetTagAction",
                "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests"));
        ((dynamic)action).Value = "Applied";

        Assert.True((bool)action.ExecuteReversibly(control, null)!);
        Assert.Equal("Applied", control.Tag);
        Assert.True((bool)action.Revert(control, null)!);
        Assert.Equal("Original", control.Tag);
    }

    [AvaloniaFact]
    public void SetTagAction_Should_Preserve_Overlapping_Reversible_Actions()
    {
        var control = new TestControl { Tag = "Original" };
        var first = Assert.IsAssignableFrom<IReversibleAction>(
            GeneratedTypeHelper.CreateInstance(
                "TestControlSetTagAction",
                "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests"));
        var second = Assert.IsAssignableFrom<IReversibleAction>(
            GeneratedTypeHelper.CreateInstance(
                "TestControlSetTagAction",
                "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests"));
        ((dynamic)first).Value = "First";
        ((dynamic)second).Value = "Second";

        Assert.True((bool)first.ExecuteReversibly(control, null)!);
        Assert.True((bool)second.ExecuteReversibly(control, null)!);
        Assert.Equal("Second", control.Tag);

        Assert.True((bool)first.Revert(control, null)!);
        Assert.Equal("Second", control.Tag);
        Assert.True((bool)second.Revert(control, null)!);
        Assert.Equal("Original", control.Tag);
    }

    [AvaloniaFact]
    public void SetTagAction_Should_Restore_Latest_Avalonia_Source_Value()
    {
        var source = new TagSource { Value = "Original" };
        var control = new TestControl();
        using var binding = control.Bind(
            Control.TagProperty,
            source.GetObservable(TagSource.ValueProperty));
        var action = Assert.IsAssignableFrom<IReversibleAction>(
            GeneratedTypeHelper.CreateInstance(
                "TestControlSetTagAction",
                "Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests"));
        ((dynamic)action).Value = "Applied";

        Assert.True((bool)action.ExecuteReversibly(control, null)!);
        source.Value = "Latest";
        Assert.Equal("Applied", control.Tag);
        Assert.True((bool)action.Revert(control, null)!);
        Assert.Equal("Latest", control.Tag);
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
        var actionSources = sources
            .Where(sourceText => sourceText.Contains("NameAction", StringComparison.Ordinal))
            .ToList();
        Assert.Equal(2, actionSources.Count);
        Assert.All(actionSources, sourceText =>
        {
            Assert.Contains("ReversiblePropertyChange<global::Host, string>", sourceText);
            Assert.Contains("new(nameof(global::Host.Name))", sourceText);
        });
    }


    private sealed class TagSource : AvaloniaObject
    {
        public static readonly StyledProperty<object?> ValueProperty =
            AvaloniaProperty.Register<TagSource, object?>(nameof(Value));

        public object? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
