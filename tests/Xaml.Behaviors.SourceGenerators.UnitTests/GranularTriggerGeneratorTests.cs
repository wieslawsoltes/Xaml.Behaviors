using System;
using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularTriggerGeneratorTests
{
    [Fact]
    public void Should_Generate_Trigger_For_Event_Attribute()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event EventHandler TestEvent
        {
            add {}
            remove {}
        }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
        Assert.Contains("typedSource.TestEvent += _proxy.OnEvent;", generated);
        Assert.Contains("typedSource.TestEvent -= _proxy.OnEvent;", generated);
    }

    [Fact]
    public void Should_Generate_Trigger_For_Assembly_Attribute()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), ""TestEvent"")]

namespace TestNamespace
{
    public class TestClass
    {
        public event EventHandler TestEvent;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
    }

    [Fact]
    public void Should_Generate_Trigger_With_Custom_EventArgs()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class CustomEventArgs : EventArgs { }

    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event EventHandler<CustomEventArgs> TestEvent
        {
            add {}
            remove {}
        }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("private void OnEvent(object? sender, global::TestNamespace.CustomEventArgs e)", generated);
    }

    [Fact]
    public void Should_Report_Error_If_Event_Not_Found_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), ""MissingEvent"")]

namespace TestNamespace
{
    public class TestClass
    {
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        // The generator generates a file with error comment instead of reporting diagnostic
        var errorFile = sources.FirstOrDefault(s => s.Contains("/* Error: Event MissingEvent not found on TestClass */"));
        Assert.NotNull(errorFile);
    }
}
