using System.Linq;
using Xunit;

namespace Xaml.PropertyGenerator.UnitTests;

/// <summary>
/// WinUI/Uno Platform annotates the property type of <c>DependencyProperty.Register</c> with
/// <c>[DynamicallyAccessedMembers]</c>; the generated registrations must not produce trimming warnings.
/// </summary>
public class GeneratorTrimmingTests
{
    private const string DynamicallyAccessedMembers = "[global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(";

    private const string WinUIGenericSource = """
        using Microsoft.UI.Xaml;
        using Xaml.PropertyGenerator;

        namespace Sample;

        public partial class Rule<TKey, T> : FrameworkElement
        {
            [StyledProperty]
            public partial T? Minimum { get; set; }

            [StyledProperty]
            public partial T? Maximum { get; set; }

            [StyledProperty]
            public partial string? Message { get; set; }
        }

        public partial class Outer<T>
        {
            public partial class Inner : FrameworkElement
            {
                [DirectProperty]
                public partial T? Value { get; set; }
            }
        }

        public sealed class IntRule : Rule<string, int>
        {
        }
        """;

    private const string WinUITypeSource = """
        using System;
        using Microsoft.UI.Xaml;
        using Xaml.PropertyGenerator;

        namespace Sample;

        public partial class Owner : FrameworkElement
        {
            [StyledProperty]
            public partial Type? DataType { get; set; }

            [StyledProperty]
            public partial string? Text { get; set; }

            partial void OnDataTypeChanged(Type? oldValue, Type? newValue) { }
        }

        [AttachedProperty("Kind", typeof(Type), IsNullable = true)]
        public static partial class Attachments
        {
        }
        """;

    [Fact]
    public void Trim_Analyzer_Reports_Unannotated_Registrations()
    {
        // Guards the test harness: the hand-written equivalents of the generated registrations produce the warnings.
        const string source = """
            using System;
            using Microsoft.UI.Xaml;

            namespace Sample;

            public partial class Rule<T> : FrameworkElement
            {
                public static readonly DependencyProperty MinimumProperty =
                    DependencyProperty.Register("Minimum", typeof(T), typeof(Rule<T>), new PropertyMetadata(default(T)));

                public static readonly DependencyProperty DataTypeProperty =
                    DependencyProperty.Register("DataType", typeof(Type), typeof(Rule<T>), new PropertyMetadata(null));
            }
            """;

        var warnings = GeneratorTestHelper.GetTrimWarnings(source, TestPlatform.WinUI, runGenerator: false);

        Assert.Contains(warnings, static d => d.Id == "IL2087");
        Assert.Contains(warnings, static d => d.Id == "IL2111");
    }

    [Fact]
    public void WinUI_Annotates_Type_Parameters_Used_As_Property_Types()
    {
        var run = GeneratorTestHelper.Run(WinUIGenericSource, TestPlatform.WinUI);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("partial class Rule<TKey, " + DynamicallyAccessedMembers, run.GeneratedSource);
        Assert.Contains("partial class Outer<" + DynamicallyAccessedMembers, run.GeneratedSource);
        Assert.Contains("partial class Inner\n", run.GeneratedSource);
        Assert.Equal(2, CountOccurrences(run.GeneratedSource, DynamicallyAccessedMembers));
    }

    [Fact]
    public void WinUI_Generic_Property_Registrations_Have_No_Trim_Warnings()
    {
        var warnings = GeneratorTestHelper.GetTrimWarnings(WinUIGenericSource, TestPlatform.WinUI);

        Assert.Empty(warnings);
    }

    [Fact]
    public void WinUI_Keeps_Existing_Type_Parameter_Annotations()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using Microsoft.UI.Xaml;
            using Xaml.PropertyGenerator;

            namespace Sample;

            public partial class Rule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : FrameworkElement
            {
                [StyledProperty]
                public partial T? Minimum { get; set; }
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.WinUI);

        Assert.Empty(run.CompilationErrors);
        Assert.DoesNotContain(DynamicallyAccessedMembers, run.GeneratedSource);
        Assert.Empty(GeneratorTestHelper.GetTrimWarnings(source, TestPlatform.WinUI));
    }

    [Fact]
    public void WinUI_Registers_Type_Properties_Through_A_Suppressed_Helper()
    {
        var run = GeneratorTestHelper.Run(WinUITypeSource, TestPlatform.WinUI);

        Assert.Empty(run.GeneratorDiagnostics);
        Assert.Empty(run.CompilationErrors);
        Assert.Contains("DataTypeProperty = RegisterDataTypeProperty();", run.GeneratedSource);
        Assert.Contains("KindProperty = RegisterKindProperty();", run.GeneratedSource);
        Assert.Contains("UnconditionalSuppressMessage(\"Trimming\", \"IL2111\"", run.GeneratedSource);
        Assert.Equal(2, CountOccurrences(run.GeneratedSource, "UnconditionalSuppressMessage"));
        Assert.Contains("public static readonly global::Microsoft.UI.Xaml.DependencyProperty TextProperty =\n", run.GeneratedSource);
    }

    [Fact]
    public void WinUI_Type_Property_Registrations_Have_No_Trim_Warnings()
    {
        var warnings = GeneratorTestHelper.GetTrimWarnings(WinUITypeSource, TestPlatform.WinUI);

        Assert.Empty(warnings);
    }

    [Fact]
    public void Avalonia_Output_Has_No_Trimming_Annotations()
    {
        const string source = """
            using System;
            using Avalonia;
            using Xaml.PropertyGenerator;

            namespace Sample;

            public partial class Rule<T> : AvaloniaObject
            {
                [StyledProperty]
                public partial T? Minimum { get; set; }

                [StyledProperty]
                public partial Type? DataType { get; set; }
            }
            """;

        var run = GeneratorTestHelper.Run(source, TestPlatform.Avalonia);

        Assert.Empty(run.CompilationErrors);
        Assert.Contains("partial class Rule<T>\n", run.GeneratedSource);
        Assert.Contains("public static readonly global::Avalonia.StyledProperty<global::System.Type?> DataTypeProperty =\n", run.GeneratedSource);
        Assert.DoesNotContain("DynamicallyAccessedMembers", run.GeneratedSource);
        Assert.DoesNotContain("UnconditionalSuppressMessage", run.GeneratedSource);
    }

    private static int CountOccurrences(string text, string value)
        => text.Split(value).Length - 1;
}
