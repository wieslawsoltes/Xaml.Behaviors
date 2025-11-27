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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger") && s.Contains("namespace TestNamespace"));
        Assert.NotNull(generated);
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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestClassTestEventTrigger"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
    }

    [Fact]
    public void Assembly_Attribute_Should_Generate_Triggers_For_Wildcard_Pattern()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), ""*Event"")]

namespace TestNamespace
{
    public class TestClass
    {
        public event EventHandler FirstEvent;
        public event EventHandler SecondEvent;
        public event EventHandler Other;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("class TestClassFirstEventTrigger"));
        Assert.Contains(sources, s => s.Contains("class TestClassSecondEventTrigger"));
        Assert.DoesNotContain(sources, s => s.Contains("class TestClassOtherTrigger"));
    }

    [Fact]
    public void Assembly_Attribute_Should_Generate_Triggers_For_Regex_Pattern()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), "".*Finished$"")]

namespace TestNamespace
{
    public class TestClass
    {
        public event EventHandler ProcessingFinished;
        public event EventHandler SetupFinished;
        public event EventHandler Started;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("class TestClassProcessingFinishedTrigger"));
        Assert.Contains(sources, s => s.Contains("class TestClassSetupFinishedTrigger"));
        Assert.DoesNotContain(sources, s => s.Contains("class TestClassStartedTrigger"));
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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger") && s.Contains("namespace TestNamespace"));
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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger") && s.Contains("namespace TestNamespace"));
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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger") && s.Contains("namespace TestNamespace"));
        Assert.True(generated is not null, string.Join("\n----\n", sources));
        var generatedText = generated!;
        Assert.Contains("private void OnEvent(", generatedText);
        Assert.Contains("Interaction.ExecuteActions(AssociatedObject, this.Actions, args", generatedText);
    }

    [Fact]
    public void Action_Delegate_Should_Keep_Weak_Source_Unsubscribe()
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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestEventTrigger") && s.Contains("namespace TestNamespace"));
        Assert.True(generated is not null, "Sources: " + string.Join("\n----\n", sources));
        var generatedText = generated!;
        Assert.Contains("_source.TryGetTarget", generatedText);
        Assert.Contains("typedSource.TestEvent -= OnEvent;", generatedText);
    }

    [Fact]
    public void Should_Report_Error_For_Generic_Trigger()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass<T>
    {
        [GenerateTypedTrigger]
        public event EventHandler TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Report_Error_For_Static_Trigger()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public static event EventHandler TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG010");
    }

    [Fact]
    public void Should_Report_Error_For_NonVoid_Delegate()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public delegate int NonVoidDelegate();

    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event NonVoidDelegate TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG002");
    }

    [Fact]
    public void Should_Report_Error_For_Out_Parameter_Delegate()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public delegate void OutParamDelegate(out int value);

    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event OutParamDelegate TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG003");
    }

    [Fact]
    public void Should_Report_Error_For_ByRef_Parameter_Delegate()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public delegate void RefParamDelegate(ref int value);

    public partial class TestClass
    {
        [GenerateTypedTrigger]
        public event RefParamDelegate TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG003");
    }

    [Fact]
    public void Assembly_Attribute_Should_Report_Error_For_NonVoid_Delegate()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), ""TestEvent"")]

namespace TestNamespace
{
    public delegate int NonVoidDelegate();

    public class TestClass
    {
        public event NonVoidDelegate TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG002");
    }

    [Fact]
    public void Assembly_Attribute_Should_Report_Error_For_Out_Parameter_Delegate()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(TestNamespace.TestClass), ""TestEvent"")]

namespace TestNamespace
{
    public delegate void OutParamDelegate(out int value);

    public class TestClass
    {
        public event OutParamDelegate TestEvent;
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG003");
    }

    [Fact]
    public void Should_Report_Error_For_Inaccessible_Trigger()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedTrigger]
        private event EventHandler TestEvent
        {
            add {}
            remove {}
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void Should_Report_Error_When_Containing_Type_Not_Accessible()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    internal class Container
    {
        private class Hidden
        {
            [GenerateTypedTrigger]
            public event EventHandler TestEvent
            {
                add {}
                remove {}
            }
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void Should_Report_Error_For_Nested_Target()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Outer
    {
        public partial class Inner
        {
            [GenerateTypedTrigger]
            public event EventHandler TestEvent
            {
                add {}
                remove {}
            }
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }
}
