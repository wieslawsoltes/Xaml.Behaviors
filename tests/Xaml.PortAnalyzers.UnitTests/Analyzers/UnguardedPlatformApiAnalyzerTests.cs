// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Xaml.PortAnalyzers.Analyzers;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Analyzers;

public class UnguardedPlatformApiAnalyzerTests
{
    private const string PortMap = """
        {
          "sourceNamespacePrefixes": ["Avalonia"],
          "mapped": {
            "types": ["Avalonia.AvaloniaObject", "Avalonia.Controls.Control", "Avalonia.AvaloniaProperty", "Avalonia.Input.Key"],
            "members": ["Avalonia.AvaloniaObject.GetValue", "Avalonia.AvaloniaObject.SetValue", "Avalonia.AvaloniaProperty.Register"]
          },
          "unsupported": {
            "types": ["Avalonia.Controls.Window"],
            "namespaces": ["Avalonia.LogicalTree"]
          }
        }
        """;

    private static PortAnalyzerTest Create(string source) => new PortAnalyzerTest().WithPortMap(PortMap).WithSource(source);

    [Fact]
    public async Task IsSilent_WithoutPortMap()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("public class A { public Avalonia.Controls.Window? W; }")
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Reports_UnmappedTypesAndMembers()
    {
        var diagnostics = await Create("""
            #if !UNO
            using Avalonia;
            using Avalonia.Controls;
            #endif

            namespace Test;

            public partial class Sample : AvaloniaObject
            {
                public static readonly StyledProperty<bool> FlagProperty = AvaloniaProperty.Register<Sample, bool>("Flag");

                public bool Flag
                {
                    get => (bool)GetValue(FlagProperty);
                    set => SetValue(FlagProperty, value);
                }

                public double Width(Control control) => control.Bounds.Width;
            }
            """).GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:10:'StyledProperty'", "XPORT002:18:'Bounds'", "XPORT002:18:'Width'"], diagnostics.Describe());
        Assert.Equal(
            "'Avalonia.StyledProperty' is not mapped on port target 'UNO'; guard it with '#if !UNO' (or the '#else' branch of '#if UNO') or add it to the port map",
            diagnostics[0].GetMessage());
        Assert.StartsWith("'Avalonia.Visual.Bounds' is not mapped", diagnostics[1].GetMessage());
        Assert.StartsWith("'Avalonia.Rect.Width' is not mapped", diagnostics[2].GetMessage());
    }

    [Fact]
    public async Task Ignores_CodeExcludedFromThePortBuild()
    {
        var diagnostics = await Create("""
            namespace Test;

            public class Sample
            {
            #if UNO
                public object? Value;
            #else
                public Avalonia.StyledProperty<bool>? Value;
            #endif

            #if !UNO
                public Avalonia.Controls.Window? Window;
            #endif

            #if NET5_0_OR_GREATER || !UNO
                public Avalonia.Controls.Window? Reported;
            #endif

            #if (!UNO && !BROWSER) || UNKNOWN
                public Avalonia.Controls.Window? Nested;
            #endif
            }
            """).GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        // 'NET5_0_OR_GREATER || !UNO' is excluded from the port build because NET5_0_OR_GREATER is not defined.
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task EvaluatesConditionsWithCurrentSymbols()
    {
        var diagnostics = await Create("""
            public class Sample
            {
            #if NET5_0_OR_GREATER || !UNO
                public Avalonia.Controls.Window? Reported;
            #endif
            }
            """)
            .WithPreprocessorSymbols("NET5_0_OR_GREATER")
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:4:'Window'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Reports_UnsupportedTypesMembersAndNamespaces()
    {
        var diagnostics = await Create("""
            using Avalonia.LogicalTree;

            public class Sample
            {
                public string? Title(Avalonia.Controls.Window window) => window.Title;

                public ILogical? Logical;
            }
            """).GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(
            ["XPORT002:1:'Avalonia.LogicalTree'", "XPORT002:5:'Window'", "XPORT002:5:'Title'", "XPORT002:7:'ILogical'"],
            diagnostics.Describe());
        Assert.StartsWith("'Avalonia.LogicalTree' is unsupported on port target 'UNO'", diagnostics[0].GetMessage());
        Assert.StartsWith("'Avalonia.Controls.Window' is unsupported", diagnostics[1].GetMessage());
        Assert.StartsWith("'Avalonia.Controls.Window.Title' is unsupported", diagnostics[2].GetMessage());
        Assert.StartsWith("'Avalonia.LogicalTree.ILogical' is unsupported", diagnostics[3].GetMessage());
    }

    [Fact]
    public async Task Reports_UsingsOfNamespacesThatAreNotMapped()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap("""{ "mapped": { "namespaces": ["Avalonia.Input"] } }""")
            .WithSource("""
                using Avalonia.Controls;
                using Avalonia.Input;
                using Avalonia.Input.Platform;
                using System.Collections.Generic;
                using static Avalonia.Controls.Control;
                using WindowAlias = Avalonia.Controls.Window;

                public class Sample
                {
                    public Key Key = Key.A;
                }
                """)
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(
            ["XPORT002:1:'Avalonia.Controls'", "XPORT002:5:'Avalonia.Controls.Control'", "XPORT002:6:'Avalonia.Controls.Window'"],
            diagnostics.Describe());
    }

    [Fact]
    public async Task Reports_RenamedNamespacesOnlyWhereTheNameIsSpelled()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap("""{ "renamedNamespaces": { "Avalonia.Controls": "Microsoft.UI.Xaml.Controls" } }""")
            .WithSource("""
                using Avalonia.Controls;

                public class Sample
                {
                    public Button? First;
                    public Avalonia.Controls.Button? Second;
                    public string? Content(Button button) => button.Content?.ToString();
                }
                """)
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:1:'Avalonia.Controls'", "XPORT002:6:'Avalonia.Controls'"], diagnostics.Describe());
        Assert.StartsWith("'Avalonia.Controls' is renamed to 'Microsoft.UI.Xaml.Controls' on port target 'UNO'", diagnostics[0].GetMessage());
    }

    [Fact]
    public async Task Reports_QualifiersOfMappedTypesThatAreNotMapped()
    {
        var diagnostics = await Create("""
            public class Sample
            {
                public Avalonia.Controls.Control? Control;
                public global::Avalonia.AvaloniaObject? Object;
            }
            """).GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:3:'Avalonia.Controls'", "XPORT002:4:'global::Avalonia'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Treats_SharedSourceSymbolsAsMapped()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap("{}")
            .WithGlobalOption("build_property.XamlPortSharedSourceExcludes", "AvaloniaOnly.cs")
            .WithSource("Shared.cs", """
                namespace Avalonia.MyLibrary
                {
                    public partial class Helper
                    {
                        public static int Shared() => 1;
                    }
                }
                """)
            .WithSource("AvaloniaOnly.cs", """
                namespace Avalonia.MyLibrary
                {
                    public partial class Helper
                    {
                        public static int AvaloniaOnly() => 2;
                    }

                    public class PlatformHelper
                    {
                        public static int Value => 3;
                    }
                }
                """)
            .WithSource("Consumer.cs", """
                public class Consumer
                {
                    public int Sum() => Avalonia.MyLibrary.Helper.Shared() + Avalonia.MyLibrary.Helper.AvaloniaOnly() + Avalonia.MyLibrary.PlatformHelper.Value;
                }
                """)
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        // Qualifiers of mapped types are reported too because the port map does not map 'Avalonia.MyLibrary'.
        Assert.Equal(
            [
                "XPORT002:3:'Avalonia.MyLibrary'",
                "XPORT002:3:'Avalonia.MyLibrary'",
                "XPORT002:3:'AvaloniaOnly'",
                "XPORT002:3:'PlatformHelper'",
                "XPORT002:3:'Value'",
            ],
            diagnostics.Describe());
    }

    [Fact]
    public async Task Supports_MemberWildcardsEnumsAndArityNames()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap("""
                {
                  "mapped": {
                    "types": ["Avalonia.Controls.Control", "Avalonia.Input.Key", "Avalonia.StyledProperty`1", "Avalonia.Media.IBrush"],
                    "members": ["Avalonia.Controls.Control.*", "Avalonia.AvaloniaProperty.Name"]
                  }
                }
                """)
            .WithSource("""
                #if !UNO
                using Avalonia.Controls;
                using Avalonia.Input;
                using Avalonia.Media;
                #endif

                public class Sample
                {
                    public object? Tag(Control control) => control.Tag;
                    public Key Key => Key.Enter;
                    public string Name(Avalonia.StyledProperty<int> property) => property.Name;
                    public IBrush[]? Brushes;
                }
                """)
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:11:'Avalonia'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Ignores_NonSourcePlatformSymbolsDocCommentsAndExcludedFiles()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap(PortMap)
            .WithGlobalOption("build_property.XamlPortSharedSourceExcludes", "Excluded/**")
            .WithSource("Excluded/A.cs", "public class A { public Avalonia.Controls.Window? W; }")
            .WithSource("B.cs", """
                using System;
                using System.Collections.Generic;

                /// <summary>See <see cref="Avalonia.Controls.Window"/>.</summary>
                public class B
                {
                    public List<string> Items { get; } = new();
                    public int Count() { var items = Items; return items.Count; }
                    public Func<int, int> Twice => static x => x * 2;
                }
                """)
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Reports_ExtensionMethodsByTheirDeclaringType()
    {
        var diagnostics = await Create("""
            #if !UNO
            using Avalonia;
            using Avalonia.Controls;
            #endif

            public class Sample
            {
                public object Observe(Control control) => control.GetObservable(Control.TagProperty);
            }
            """).GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(["XPORT002:8:'GetObservable'", "XPORT002:8:'TagProperty'"], diagnostics.Describe());
        Assert.StartsWith("'Avalonia.AvaloniaObjectExtensions.GetObservable'", diagnostics[0].GetMessage());
    }

    [Fact]
    public async Task UsesConfiguredTargetSymbol()
    {
        var diagnostics = await Create("""
            public class Sample
            {
            #if !WINUI
                public Avalonia.Controls.Window? Guarded;
            #endif
            #if !UNO
                public Avalonia.Controls.Window? NotGuardedForWinUI;
            #endif
            }
            """)
            .WithGlobalOption("build_property.XamlPortTargetSymbol", "WINUI")
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("XPORT002:7:'Window'", diagnostic.Describe());
        Assert.Contains("'#if !WINUI'", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Reports_InvalidPortMaps()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithPortMap(PortMap)
            .WithPortMap("{\n  \"mapped\": { \"types\": 1 }\n}", "broken.xamlport.json")
            .WithSource("public class A { public Avalonia.Controls.Window? W; }")
            .GetDiagnosticsAsync(new UnguardedPlatformApiAnalyzer());

        Assert.Equal(2, diagnostics.Length);
        var invalid = Assert.Single(diagnostics, d => d.Id == "XPORT005");
        Assert.Equal(PortAnalyzerTest.PathOf("broken.xamlport.json"), invalid.Location.GetLineSpan().Path);
        Assert.Equal(1, invalid.Location.GetLineSpan().StartLinePosition.Line);
        Assert.Contains("'mapped.types' must be an array of strings", invalid.GetMessage());
        Assert.Single(diagnostics, d => d.Id == "XPORT002");
    }
}
