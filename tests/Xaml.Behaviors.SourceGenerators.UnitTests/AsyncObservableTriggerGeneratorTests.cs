using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class AsyncObservableTriggerGeneratorTests
{
    [Fact]
    public void Should_Generate_Async_Trigger()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateAsyncTrigger]
        public Task<int>? LoadTask { get; set; }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("LoadTaskAsyncTrigger"));
        Assert.Contains(sources, s => s.Contains("IsExecutingProperty"));
    }

    [Fact]
    public void Should_Generate_Observable_Trigger()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateObservableTrigger]
        public IObservable<int>? Stream { get; set; }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("StreamObservableTrigger"));
        Assert.Contains(sources, s => s.Contains("LastValueProperty"));
    }

    [Fact]
    public void AsyncTrigger_Defaults_To_UseDispatcher_For_Member()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

public partial class Vm
{
    [GenerateAsyncTrigger]
    public Task? LoadTask { get; set; }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("LoadTaskAsyncTrigger") && s.Contains("Dispatcher.UIThread.Post"));
    }

    [Fact]
    public void Assembly_AsyncTrigger_Defaults_To_UseDispatcher()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        public Task? LoadTask { get; set; }
    }
}

[assembly: GenerateAsyncTrigger(typeof(TestNamespace.Vm), ""LoadTask"")]
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("LoadTaskAsyncTrigger") && s.Contains("Dispatcher.UIThread.Post"));
    }

    [Fact]
    public void AsyncTrigger_Invalid_Property_Type_Reports_Diagnostic()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateAsyncTrigger]
        public int NotATask { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG025");
    }

    [Fact]
    public void ObservableTrigger_Invalid_Property_Type_Reports_Diagnostic()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateObservableTrigger]
        public string NotObservable { get; set; } = string.Empty;
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG026");
    }

    [Fact]
    public void AsyncTrigger_Inaccessible_Property_Reports_Diagnostic()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateAsyncTrigger]
        private Task? HiddenTask { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void ObservableTrigger_Inaccessible_Property_Reports_Diagnostic()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateObservableTrigger]
        private IObservable<int>? HiddenStream { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void AsyncTrigger_Missing_Property_Pattern_Reports_Diagnostic()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm { }
}

[assembly: GenerateAsyncTrigger(typeof(TestNamespace.Vm), ""MissingTask"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG023");
    }

    [Fact]
    public void ObservableTrigger_Missing_Property_Pattern_Reports_Diagnostic()
    {
        var source = @"
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm { }
}

[assembly: GenerateObservableTrigger(typeof(TestNamespace.Vm), ""MissingStream"")]
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG024");
    }

    [Fact]
    public void AsyncTrigger_Inaccessible_Property_Type_Reports_Diagnostic()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    internal class Secret { }

    public partial class Vm
    {
        [GenerateAsyncTrigger]
        public Task<Secret>? Hidden { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void ObservableTrigger_Inaccessible_Property_Type_Reports_Diagnostic()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    internal class Secret { }

    public partial class Vm
    {
        [GenerateObservableTrigger]
        public IObservable<Secret>? Hidden { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG014");
    }

    [Fact]
    public void AsyncTrigger_Generic_ContainingType_Reports_Diagnostic()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm<T>
    {
        [GenerateAsyncTrigger]
        public Task? LoadTask { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void ObservableTrigger_Generic_ContainingType_Reports_Diagnostic()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm<T>
    {
        [GenerateObservableTrigger]
        public IObservable<int>? Stream { get; set; }
    }
}
";

        var (diagnostics, _) = GeneratorTestHelper.RunGenerator(source);

        Assert.Contains(diagnostics, d => d.Id == "XBG008");
    }

    [Fact]
    public void Should_Generate_Multiple_Async_Triggers_From_Multiple_Attributes()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateAsyncTrigger(Name = ""FirstLoadTask"")]
        [GenerateAsyncTrigger(Name = ""SecondLoadTask"", UseDispatcher = false)]
        public Task<int>? LoadTask { get; set; }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("FirstLoadTask", StringComparison.Ordinal));
        Assert.Contains(sources, s => s.Contains("SecondLoadTask", StringComparison.Ordinal));
    }

    [Fact]
    public void Should_Generate_Multiple_Observable_Triggers_From_Multiple_Attributes()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

namespace TestNamespace
{
    public partial class Vm
    {
        [GenerateObservableTrigger(Name = ""FirstStreamTrigger"")]
        [GenerateObservableTrigger(Name = ""SecondStreamTrigger"", FireOnAttach = false)]
        public IObservable<int>? Stream { get; set; }
    }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains(sources, s => s.Contains("FirstStreamTrigger", StringComparison.Ordinal));
        Assert.Contains(sources, s => s.Contains("SecondStreamTrigger", StringComparison.Ordinal));
    }

    [Fact]
    public void AsyncTrigger_Different_Options_With_Same_Name_Are_Unique()
    {
        var source = @"
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

public partial class Vm
{
    [GenerateAsyncTrigger(Name = ""LoadTrigger"", UseDispatcher = true)]
    [GenerateAsyncTrigger(Name = ""LoadTrigger"", UseDispatcher = false, FireOnAttach = false)]
    public Task? LoadTask { get; set; }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);

        var classNames = sources
            .SelectMany(s => Regex.Matches(s, @"class\s+(?<name>\w+)\s*:\s*Avalonia\.Xaml\.Interactivity\.StyledElementTrigger")
                .Select(m => m.Groups["name"].Value))
            .Distinct()
            .ToList();

        Assert.True(classNames.Count >= 2, "Expected two generated async triggers with differing options.");
    }

    [Fact]
    public void ObservableTrigger_Different_Options_With_Same_Name_Are_Unique()
    {
        var source = @"
using System;
using Xaml.Behaviors.SourceGenerators;

public partial class Vm
{
    [GenerateObservableTrigger(Name = ""StreamTrigger"", UseDispatcher = true)]
    [GenerateObservableTrigger(Name = ""StreamTrigger"", UseDispatcher = false, FireOnAttach = false)]
    public IObservable<int>? Stream { get; set; }
}
";

        var (diagnostics, sources) = GeneratorTestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);

        var classNames = sources
            .SelectMany(s => Regex.Matches(s, @"class\s+(?<name>\w+)\s*:\s*Avalonia\.Xaml\.Interactivity\.StyledElementTrigger")
                .Select(m => m.Groups["name"].Value))
            .Distinct()
            .ToList();

        Assert.True(classNames.Count >= 2, "Expected two generated observable triggers with differing options.");
    }
}
