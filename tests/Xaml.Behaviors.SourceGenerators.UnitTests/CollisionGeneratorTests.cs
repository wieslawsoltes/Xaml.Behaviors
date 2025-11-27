using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class CollisionGeneratorTests
{
    [Fact]
    public void Actions_With_Same_Name_Should_Get_Unique_ClassNames()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class First
    {
        [GenerateTypedAction]
        public void Duplicate() { }
    }

    public partial class Second
    {
        [GenerateTypedAction]
        public void Duplicate() { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.True(sources.Any(), "Sources: " + string.Join("\n----\n", sources));
        var generated = sources.Where(s => s.Contains("class DuplicateAction") && s.Contains("namespace TestNamespace")).ToList();
        Assert.Equal(2, generated.Count);
        Assert.NotEqual(ExtractClassName(generated[0]), ExtractClassName(generated[1]));
    }

    [Fact]
    public void Triggers_With_Same_Name_Should_Get_Unique_ClassNames()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedTriggerAttribute : Attribute { }
}

namespace TestNamespace
{
    public partial class First
    {
        [GenerateTypedTrigger]
        public event EventHandler Duplicate
        {
            add { }
            remove { }
        }
    }

    public partial class Second
    {
        [GenerateTypedTrigger]
        public event EventHandler Duplicate
        {
            add { }
            remove { }
        }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.True(sources.Length > 6, "Sources: " + string.Join("\n----\n", sources));
        var generated = sources.Where(s => s.Contains("class DuplicateTrigger") && s.Contains("namespace TestNamespace")).ToList();
        Assert.Equal(2, generated.Count);
        Assert.NotEqual(ExtractClassName(generated[0]), ExtractClassName(generated[1]));
    }

    [Fact]
    public void Trigger_Defined_By_Attribute_And_Assembly_Is_Deduplicated()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TriggerSource), nameof(TestNamespace.TriggerSource.ProcessingFinished))]

namespace TestNamespace
{
    public partial class TriggerSource
    {
        [GenerateTypedTrigger]
        public event EventHandler ProcessingFinished
        {
            add { }
            remove { }
        }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.Where(s => s.Contains("class ProcessingFinishedTrigger") && s.Contains("namespace TestNamespace")).ToList();
        Assert.Single(generated);
    }

    [Fact]
    public void DataTriggers_With_Same_Simple_Name_Should_Get_Unique_ClassNames()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace A { public class Collision { } }
namespace B { public class Collision { } }

[assembly: GenerateTypedDataTrigger(typeof(A.Collision))]
[assembly: GenerateTypedDataTrigger(typeof(B.Collision))]
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.True(sources.Length > 6, "Sources: " + string.Join("\n----\n", sources));
        var generated = sources.Where(s => s.Contains("class CollisionDataTrigger")).ToList();
        Assert.Equal(2, generated.Count);
        Assert.NotEqual(ExtractClassName(generated[0]), ExtractClassName(generated[1]));
    }

    [Fact]
    public void Actions_With_Same_Name_Different_Namespace_Should_Not_Collide()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace FirstNamespace
{
    public partial class First
    {
        [GenerateTypedAction]
        public void Duplicate() { }
    }
}

namespace SecondNamespace
{
    public partial class Second
    {
        [GenerateTypedAction]
        public void Duplicate() { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.Where(s => s.Contains("class DuplicateAction")).ToList();
        Assert.Equal(2, generated.Count);
        Assert.Single(generated.Select(ExtractClassName).Distinct());
    }

    private static string? ExtractClassName(string source)
    {
        var match = Regex.Match(source, @"class\s+(?<name>\w+)");
        return match.Success ? match.Groups["name"].Value : null;
    }
}
