using System;
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

        Assert.Contains(diagnostics, d => d.Id == "XBG033");
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

    [Fact]
    public void Should_Report_Diagnostic_For_Inaccessible_Method()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction]
        private void OnSecret(EventArgs args) { }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Ref_Parameter()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction]
        public void Handle(ref EventArgs args) { }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG009");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Ambiguous_Overloads()
    {
        var source = @"
using Avalonia.Interactivity;
using Avalonia.Input;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventArgsAction(typeof(Handler), ""OnRouted"")]

public partial class Handler
{
    public void OnRouted(RoutedEventArgs args) { }

    public void OnRouted(PointerEventArgs args) { }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG007");
    }

    [Fact]
        public void Duplicate_Named_EventArgsActions_With_Different_Options_Should_Not_Collide()
        {
            var source = @"
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

public partial class Handler
{
    [GenerateEventArgsAction(Name = ""Shared"")]
    [GenerateEventArgsAction(Name = ""Shared"", UseDispatcher = true, Project = ""Handled"")]
    public void OnRouted(RoutedEventArgs args) { }
}
";

            var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

            Assert.Empty(diagnostics);
            var generated = sources.Where(s => s.Contains("Shared", StringComparison.Ordinal) && s.Contains("StyledElementAction", StringComparison.Ordinal)).ToList();
            Assert.True(generated.Count >= 2, "Expected two generated EventArgs actions for differing options.");
        }

    [Fact]
    public void Should_Report_Diagnostic_For_Generic_Containing_Type()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler<T>
    {
        [GenerateEventArgsAction]
        public void OnGeneric(EventArgs args) { }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Generate_Multiple_EventArgs_Actions_From_Multiple_Attributes()
    {
        var source = @"
using Avalonia.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Handler
    {
        [GenerateEventArgsAction(Name = ""FirstRoutedAction"")]
        [GenerateEventArgsAction(Name = ""SecondRoutedAction"", Project = ""Handled"")]
        public void OnRouted(RoutedEventArgs args) { }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("FirstRoutedAction", StringComparison.Ordinal));
        Assert.Contains(sources, s => s.Contains("SecondRoutedAction", StringComparison.Ordinal));
        Assert.Contains(sources, s => s.Contains("HandledProperty", StringComparison.Ordinal));
    }
}
