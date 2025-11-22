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
        Assert.True(generated is not null, string.Join("\n----\n", sources));
        var generatedText = generated!;
        Assert.Contains("private void OnEvent", generatedText);
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

        Assert.Contains(diagnostics, d => d.Id == "XBG004");
    }

    [Fact]
        public void Should_Generate_Trigger_For_Action_Delegate()
        {
            var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event Action TestEvent;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger"));
        Assert.True(generated is not null, string.Join("\n----\n", sources));
        var generatedText = generated!;
        Assert.Contains("private void OnEvent()", generatedText);
        Assert.Contains("Interaction.ExecuteActions(AssociatedObject, this.Actions, null);", generatedText);
    }

    [Fact]
    public void Should_Generate_Trigger_For_Custom_Delegate_With_EventArgs()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public class CustomArgs : EventArgs { }
    public delegate void CustomDelegate(CustomArgs args);

    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event CustomDelegate TestEvent;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger"));
        Assert.True(generated is not null, string.Join("\n----\n", sources));
        var generatedText = generated!;
        Assert.Contains("private void OnEvent(", generatedText);
        Assert.Contains("Interaction.ExecuteActions(AssociatedObject, this.Actions, args", generatedText);
    }
}
