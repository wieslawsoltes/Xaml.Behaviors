using System.Linq;
using Avalonia.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class EventArgsActionGeneratorTests
{
    [Fact]
    public void Should_Generate_EventArgs_Action()
    {
        var source = @"
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction]
        public void OnRouted(RoutedEventArgs args) { }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("OnRoutedEventArgsAction"));
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Invalid_Method_Signature()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction]
        public void OnInvalid() { }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG007");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Missing_Projection()
    {
        var source = @"
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction(Project = ""MissingProperty"")]
        public void OnRouted(RoutedEventArgs args) { }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG027");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Inaccessible_Projection()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public class SecretArgs : EventArgs
{
    internal int Secret { get; } = 42;
}

public partial class Handler
{
    [GenerateEventArgsAction(Project = ""Secret"")]
    public void OnSecret(SecretArgs args) { }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG028");
    }
}
