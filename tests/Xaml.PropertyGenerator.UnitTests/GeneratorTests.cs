using System;
using System.Linq;
using Xunit;

namespace Xaml.PropertyGenerator.UnitTests;

public class GeneratorTests
{
    private const string AvaloniaSource = """
        using System.Collections.Generic;
        using Avalonia;
        using Xaml.PropertyGenerator;

        namespace Sample;

        public enum Mode { First, Second = 4 }

        public partial class Owner : AvaloniaObject
        {
            [StyledProperty(DefaultValue = true)]
            public partial bool IsActive { get; set; }

            [StyledProperty(DefaultValue = Mode.Second, DefaultBindingMode = PropertyBindingMode.TwoWay)]
            public partial Mode Mode { get; set; }

            [StyledProperty(DefaultValueExpression = "System.TimeSpan.FromMilliseconds(500)")]
            public partial System.TimeSpan Delay { get; set; }

            [StyledProperty(Content = true, ResolveByName = true)]
            public partial object? Target { get; set; }

            [DirectProperty(DefaultValue = true)]
            public partial bool CanExecute { get; private set; }

            [DirectProperty(Lazy = true)]
            public partial List<int> Items { get; }

            [StyledProperty]
            public partial string? Text { get; set; }

            partial void OnTextChanged(string? oldValue, string? newValue) { }
        }

        [AttachedProperty("Tag", typeof(string), IsNullable = true)]
        public partial class Attachments
        {
            static partial void OnTagChanged(AvaloniaObject element, string? oldValue, string? newValue) { }
        }

        public partial class Outer<T>
        {
            public partial class Inner : AvaloniaObject
            {
                [StyledProperty]
                public partial T? Value { get; set; }
            }
        }
        """;

    private const string WinUISource = """
        using System.Collections.Generic;
        using Microsoft.UI.Xaml;
        using Xaml.PropertyGenerator;

        namespace Sample;

        public enum Mode { First, Second = 4 }

        public partial class Owner : FrameworkElement
        {
            public int ChangedCount { get; private set; }

            [StyledProperty(DefaultValue = true)]
            public partial bool IsActive { get; set; }

            [StyledProperty(DefaultValue = Mode.Second)]
            public partial Mode Mode { get; set; }

            [StyledProperty(DefaultValueExpression = "System.TimeSpan.FromMilliseconds(500)")]
            public partial System.TimeSpan Delay { get; set; }

            [StyledProperty(Content = true)]
            public partial object? Target { get; set; }

            [DirectProperty(DefaultValue = true)]
            public partial bool CanExecute { get; private set; }

            [DirectProperty(Lazy = true)]
            public partial List<int> Items { get; }

            [StyledProperty]
            public partial string? Text { get; set; }

            partial void OnTextChanged(string? oldValue, string? newValue) { }

            protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs change) => ChangedCount++;
        }

        [AttachedProperty("Tag", typeof(string), IsNullable = true, HostType = typeof(UIElement))]
        public static partial class Attachments
        {
            static partial void OnTagChanged(UIElement element, string? oldValue, string? newValue) { }
        }
        """;

    [Fact]
    public void Avalonia_Output_Compiles()
    {
        var run = GeneratorTestHelper.Run(AvaloniaSource, TestPlatform.Avalonia);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("global::Avalonia.StyledProperty<bool> IsActiveProperty", run.GeneratedSource);
        Assert.Contains("defaultValue: (bool)(true)", run.GeneratedSource);
        Assert.Contains("defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay", run.GeneratedSource);
        Assert.Contains("global::Avalonia.DirectProperty<global::Sample.Owner, bool> CanExecuteProperty", run.GeneratedSource);
        Assert.Contains("SetAndRaise(CanExecuteProperty, ref field, value)", run.GeneratedSource);
        Assert.Contains("get => field ??= new global::System.Collections.Generic.List<int>();", run.GeneratedSource);
        Assert.Contains("[global::Avalonia.Metadata.Content]", run.GeneratedSource);
        Assert.Contains("[global::Avalonia.Controls.ResolveByName]", run.GeneratedSource);
        Assert.Contains("global::Avalonia.AttachedProperty<string?> TagProperty", run.GeneratedSource);
        Assert.Contains("partial void OnTextChanged(string? oldValue, string? newValue);", run.GeneratedSource);
    }

    [Fact]
    public void WinUI_Output_Compiles()
    {
        var run = GeneratorTestHelper.Run(WinUISource, TestPlatform.WinUI);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("global::Microsoft.UI.Xaml.DependencyProperty IsActiveProperty", run.GeneratedSource);
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata((bool)(true)", run.GeneratedSource);
        Assert.Contains("[global::Microsoft.UI.Xaml.Markup.ContentProperty(Name = \"Target\")]", run.GeneratedSource);
        Assert.Contains("owner.OnPropertyChanged(e);", run.GeneratedSource);
        Assert.Contains("owner.CanExecute = (bool)e.NewValue!;", run.GeneratedSource);
        Assert.Contains("RegisterAttached(\"Tag\"", run.GeneratedSource);
    }

    [Fact]
    public void WinUI_Reports_Unsupported_Options()
    {
        const string source = """
            using Microsoft.UI.Xaml;
            using Xaml.PropertyGenerator;
            namespace Sample;
            public partial class Owner : FrameworkElement
            {
                [StyledProperty(Inherits = true)]
                public partial bool IsActive { get; set; }
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.WinUI);

        Assert.Empty(run.CompilationErrors);
        Assert.Contains(run.GeneratorDiagnostics, d => d.Id == "XPG0005");
    }

    [Theory]
    [InlineData("public bool IsActive { get; set; }", "XPG0001")]
    [InlineData("public partial bool IsActive { get; set; }", "XPG0002")]
    public void Reports_Declaration_Errors(string member, string id)
    {
        var source = $$"""
            using Avalonia;
            using Xaml.PropertyGenerator;
            namespace Sample;
            public {{(id == "XPG0002" ? string.Empty : "partial ")}}class Owner : AvaloniaObject
            {
                [StyledProperty]
                {{member}}
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.Avalonia);

        Assert.Contains(run.GeneratorDiagnostics, d => d.Id == id);
    }

    [Theory]
    [InlineData("double.PositiveInfinity", "double", "double.PositiveInfinity")]
    [InlineData("double.NegativeInfinity", "double", "double.NegativeInfinity")]
    [InlineData("double.NaN", "double", "double.NaN")]
    [InlineData("float.PositiveInfinity", "float", "float.PositiveInfinity")]
    [InlineData("0.5", "double", "0.5d")]
    public void Special_Floating_Point_Defaults_Are_Emitted(string value, string type, string expected)
    {
        var source = $$"""
            using Avalonia;
            using Xaml.PropertyGenerator;
            namespace Sample;
            public partial class Owner : AvaloniaObject
            {
                [StyledProperty(DefaultValue = {{value}})]
                public partial {{type}} Size { get; set; }
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.Avalonia);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains($"defaultValue: ({type})({expected})", run.GeneratedSource);
    }

    [Fact]
    public void Reports_Lazy_Property_With_Setter()
    {
        const string source = """
            using System.Collections.Generic;
            using Avalonia;
            using Xaml.PropertyGenerator;
            namespace Sample;
            public partial class Owner : AvaloniaObject
            {
                [DirectProperty(Lazy = true)]
                public partial List<int> Items { get; set; }
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.Avalonia);

        Assert.Contains(run.GeneratorDiagnostics, d => d.Id == "XPG0004");
    }

    [Fact]
    public void Attributes_Are_Not_Persisted_In_Metadata()
    {
        var run = GeneratorTestHelper.Run(AvaloniaSource, TestPlatform.Avalonia);

        Assert.Empty(run.CompilationErrors);
        Assert.DoesNotContain("XAML_PROPERTY_GENERATOR_ATTRIBUTES", run.GeneratedSource.Split('\n').Where(static l => l.StartsWith("#define", System.StringComparison.Ordinal)));
    }

    private const string EnumSource = """
        using Microsoft.UI.Xaml;
        using Xaml.PropertyGenerator;

        namespace TestNs;

        public enum Mode { First, Second }

        public partial class Host : BASE
        {
            [StyledProperty(DefaultValue = Mode.Second)]
            public partial Mode Mode { get; set; }

            [StyledProperty]
            public partial Mode? OptionalMode { get; set; }

            [StyledProperty]
            public partial string? Text { get; set; }

            [StyledProperty]
            public partial DependencyProperty? Target { get; set; }

            [StyledProperty]
            public partial Host? Other { get; set; }

            [StyledProperty]
            public partial System.Collections.Generic.List<Host>? Others { get; set; }

            [StyledProperty]
            public partial System.TimeSpan Delay { get; set; }

            [StyledProperty]
            public partial System.Windows.Input.ICommand? Command { get; set; }

            [StyledProperty]
            public partial System.Type? Kind { get; set; }

            [StyledProperty]
            public partial double? Size { get; set; }

            [StyledProperty]
            public partial DependencyObject? Child { get; set; }
        }
        """;

    [Fact]
    public void NativeWinUI_Registers_Properties_Of_Other_Than_Framework_Types_As_Object()
    {
        var run = GeneratorTestHelper.Run(EnumSource.Replace("BASE", "DependencyObject", StringComparison.Ordinal), TestPlatform.NativeWinUI);

        Assert.Empty(run.CompilationErrors);
        Assert.Contains("Register(nameof(Mode), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(OptionalMode), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Text), typeof(string)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Target), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        // Types that are not framework types are opaque to native WinUI: dependency objects stored in such a
        // property would not join the tree of their owner.
        Assert.Contains("Register(nameof(Other), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Others), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Delay), typeof(global::System.TimeSpan)", run.GeneratedSource, StringComparison.Ordinal);
        // System types that are not WinUI classes have no base type in the XAML type information: asking for it
        // terminates the application.
        Assert.Contains("Register(nameof(Command), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Kind), typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Size), typeof(double?)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Child), typeof(global::Microsoft.UI.Xaml.DependencyObject)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("get => (global::TestNs.Mode)GetValue(ModeProperty)", run.GeneratedSource, StringComparison.Ordinal);
    }

    [Fact]
    public void UnoPlatform_Registers_Enum_Properties_With_Their_Type()
    {
        // Uno Platform implements DependencyObject through its own generator: the host is a FrameworkElement.
        var run = GeneratorTestHelper.Run(EnumSource.Replace("BASE", "FrameworkElement", StringComparison.Ordinal), TestPlatform.WinUI);

        Assert.Empty(run.CompilationErrors);
        Assert.DoesNotContain("typeof(object)", run.GeneratedSource, StringComparison.Ordinal);
        Assert.Contains("Register(nameof(Mode), typeof(global::TestNs.Mode)", run.GeneratedSource, StringComparison.Ordinal);
    }
}
