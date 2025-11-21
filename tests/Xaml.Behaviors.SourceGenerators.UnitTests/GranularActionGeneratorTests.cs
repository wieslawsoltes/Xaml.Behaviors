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
        var generated = sources.FirstOrDefault(s => s.Contains("class TestMethodAction"));
        Assert.NotNull(generated);
        Assert.Contains("namespace TestNamespace", generated);
        Assert.Contains("typedTarget.TestMethod()", generated);
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
        Assert.Contains("public static readonly StyledProperty<string> P1Property", generated);
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
        Assert.DoesNotContain("public static readonly StyledProperty<object> SenderProperty", generated);
        Assert.Contains("var p1 = sender is object s ? s : default;", generated);
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
        Assert.Contains("public static readonly StyledProperty<string> MethodParameterTargetObjectProperty", generated);
        Assert.Contains("typedTarget.TestMethod(MethodParameterTargetObject)", generated);
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
}
