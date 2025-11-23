using System.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class GranularActionGeneratorTests
{
    [Fact]
    public void Should_Generate_Action_For_Method_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public void TestMethod() { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
        Assert.Contains("public partial class TestMethodAction", generated);
        Assert.Contains("typedTarget.TestMethod()", generated);
    }

    [Fact]
    public void Should_Generate_Action_For_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedAction(typeof(TestNamespace.TestClass), ""TestMethod"")]

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod() { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestClassTestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
        Assert.Contains("typedTarget.TestMethod()", generated);
    }

    [Fact]
    public void Assembly_Attribute_Should_Generate_Actions_For_Wildcard_Pattern()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedAction(typeof(TestNamespace.TestClass), ""Do*"")]

namespace TestNamespace
{
    public class TestClass
    {
        public void DoWork() { }
        public void DoMore() { }
        public void Skip() { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("class TestClassDoWorkAction"));
        Assert.Contains(sources, s => s.Contains("class TestClassDoMoreAction"));
        Assert.DoesNotContain(sources, s => s.Contains("class TestClassSkipAction"));
    }

    [Fact]
    public void Should_Generate_Action_With_Parameters()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public void TestMethod(string p1, int p2) { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<string", generated);
        Assert.Contains("public static readonly StyledProperty<int> P2Property", generated);
        Assert.Contains("typedTarget.TestMethod(P1, P2)", generated);
    }

    [Fact]
    public void Should_Generate_Action_With_EventHandler_Signature()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public void TestMethod(object sender, object parameter) { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        // Should NOT generate properties for sender/parameter
        Assert.DoesNotContain("public static readonly StyledProperty<object>", generated);
        Assert.Contains("var p1 = sender is object", generated);
        Assert.Contains("typedTarget.TestMethod(p1, p2)", generated);
    }

    [Fact]
    public void Should_Generate_Async_Action_Task()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public Task TestMethodAsync() => Task.CompletedTask;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAsyncAction"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<bool> IsExecutingProperty", generated);
        Assert.Contains("var t = task;", generated);
        Assert.Contains("TrackTask(t);", generated);
        Assert.Contains("IsExecuting = true;", generated);
    }

    [Fact]
    public void Should_Generate_Async_Action_ValueTask()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public ValueTask TestMethodAsync() => default;
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAsyncAction"));
        Assert.NotNull(generated);
        Assert.Contains("var t = task.AsTask();", generated);
        Assert.Contains("TrackTask(t);", generated);
    }

    [Fact]
    public void Should_Handle_TargetObject_Parameter_Name_Conflict()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public void TestMethod(string targetObject) { }
    }
}";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("public static readonly StyledProperty<string", generated);
        Assert.Contains("typedTarget.TestMethod(MethodParameterTargetObject", generated);
    }

    [Fact]
    public void Should_Handle_Global_Namespace()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

public partial class GlobalClass
{
    [GenerateTypedAction]
    public void TestMethod() { }
}
";
        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("public partial class TestMethodAction", generated);
    }

    [Fact]
    public void Should_Report_Error_If_Method_Not_Found_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedAction(typeof(TestNamespace.TestClass), ""MissingMethod"")]

namespace TestNamespace
{
    public class TestClass
    {
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG006");
    }

    [Fact]
    public void Should_Report_Error_On_Ambiguous_Method_Assembly_Attribute()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedAction(typeof(TestNamespace.TestClass), ""Overloaded"")]

namespace TestNamespace
{
    public class TestClass
    {
        public void Overloaded() { }
        public void Overloaded(int value) { }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG007");
    }

    [Fact]
    public void Should_Report_Error_For_Generic_Action()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass<T>
    {
        [GenerateTypedAction]
        public void TestMethod(T value) { }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Report_Error_For_Static_Action()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public static void TestMethod() { }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG010");
    }

    [Fact]
    public void Should_Report_Error_For_Ref_Parameter()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        public void TestMethod(ref int value) { }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG009");
    }

    [Fact]
    public void Should_Report_Error_For_Inaccessible_Method()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class TestClass
    {
        [GenerateTypedAction]
        private void Hidden() { }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void Should_Report_Error_When_Containing_Type_Not_Accessible()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    internal class Container
    {
        private class Hidden
        {
            [GenerateTypedAction]
            public void TestMethod() { }
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
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Outer
    {
        public partial class Inner
        {
            [GenerateTypedAction]
            public void TestMethod() { }
        }
    }
}";
        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
    }
}
