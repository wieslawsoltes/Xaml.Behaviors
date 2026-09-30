// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Xaml.PortAnalyzers.Analyzers;
using Xaml.PortAnalyzers.CodeFixes;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Analyzers;

public class PartialDependencyObjectAnalyzerTests
{
    private const string UnoStub = """
        namespace Microsoft.UI.Xaml
        {
            public partial interface DependencyObject { }
        }
        """;

    [Fact]
    public async Task Reports_NonPartialDirectAvaloniaObjectSubclasses()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                using Avalonia;

                namespace Test;

                public class Plain : AvaloniaObject { }
                public partial class Ok : AvaloniaObject { }
                public class Indirect : Ok { }
                public class Generic<T> : AvaloniaObject { }
                public class Unrelated { }
                public abstract partial class Split : AvaloniaObject { }
                public abstract partial class Split { }
                """)
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        Assert.Equal(["XPORT001:5:'Plain'", "XPORT001:8:'Generic'"], diagnostics.Describe());
        Assert.Equal(
            "Type 'Plain' derives directly from 'AvaloniaObject' and is shared with port target 'UNO'; declare 'Plain' partial so the port's dependency object generator can extend it",
            diagnostics[0].GetMessage());
    }

    [Fact]
    public async Task Reports_TypesImplementingTheWinUIDependencyObjectInterface()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("Uno.cs", UnoStub)
            .WithSource("""
                namespace Test;

                public class Direct : Microsoft.UI.Xaml.DependencyObject { }
                public partial class Fine : Microsoft.UI.Xaml.DependencyObject { }
                """)
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        Assert.Equal(["XPORT001:3:'Direct'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Reports_NonPartialContainingTypes()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                using Avalonia;

                namespace Test;

                public class Outer
                {
                    public partial class Inner : AvaloniaObject { }
                }
                """)
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("XPORT001:7:'Inner'", diagnostic.Describe());
        Assert.Contains("declare 'Outer' partial", diagnostic.GetMessage());
    }

    [Fact]
    public async Task Ignores_FilesThatAreNotShared()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithGlobalOption("build_property.XamlPortSharedSourceExcludes", "Avalonia/**;Private.cs")
            .WithSource("Avalonia/Excluded.cs", "public class A : Avalonia.AvaloniaObject { }")
            .WithSource("Private.cs", "public class B : Avalonia.AvaloniaObject { }")
            .WithSource("NotShared.cs", "public class C : Avalonia.AvaloniaObject { }")
            .WithFileOption("NotShared.cs", "xaml_port.shared", "false")
            .WithSource("Shared.cs", "public class D : Avalonia.AvaloniaObject { }")
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        Assert.Equal(["XPORT001:1:'D'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Ignores_DeclarationsExcludedFromThePortBuild()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                #if UNO
                public partial class Guarded : Microsoft.UI.Xaml.DependencyObject { }
                #else
                public class Guarded : Avalonia.AvaloniaObject { }
                #endif
                #if !UNO
                public class AvaloniaOnly : Avalonia.AvaloniaObject { }
                #endif
                """)
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task UsesConfiguredTargetSymbolInMessage()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithGlobalOption("build_property.XamlPortTargetSymbol", "WINUI")
            .WithSource("public class A : Avalonia.AvaloniaObject { }")
            .GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer());

        Assert.Contains("port target 'WINUI'", Assert.Single(diagnostics).GetMessage());
    }

    [Fact]
    public async Task Ignores_CompilationsWithoutDependencyObjects()
    {
        var test = new PortAnalyzerTest().WithSource("public class A { }");

        Assert.Empty(await test.GetDiagnosticsAsync(new PartialDependencyObjectAnalyzer()));
    }

    [Fact]
    public async Task CodeFix_AddsPartialAfterExistingModifiers()
    {
        var fixedSource = await new PortAnalyzerTest()
            .WithSource("""
                using Avalonia;

                namespace Test;

                /// <summary>Docs.</summary>
                public sealed class Plain : AvaloniaObject
                {
                }
                """)
            .ApplyCodeFixesAsync(new PartialDependencyObjectAnalyzer(), new AddPartialModifierCodeFixProvider());

        Assert.Equal("""
            using Avalonia;

            namespace Test;

            /// <summary>Docs.</summary>
            public sealed partial class Plain : AvaloniaObject
            {
            }
            """, fixedSource);
    }

    [Fact]
    public async Task CodeFix_AddsPartialWithoutModifiersAndKeepsAttributes()
    {
        var fixedSource = await new PortAnalyzerTest()
            .WithSource("""
                using System;

                namespace Test;

                [Obsolete]
                class Plain : Avalonia.AvaloniaObject { }
                """)
            .ApplyCodeFixesAsync(new PartialDependencyObjectAnalyzer(), new AddPartialModifierCodeFixProvider());

        Assert.Equal("""
            using System;

            namespace Test;

            [Obsolete]
            partial class Plain : Avalonia.AvaloniaObject { }
            """, fixedSource);
    }

    [Fact]
    public async Task CodeFix_MakesContainingTypesPartial()
    {
        var fixedSource = await new PortAnalyzerTest()
            .WithSource("""
                namespace Test;

                public static class Outer
                {
                    internal class Middle
                    {
                        public class Inner : Avalonia.AvaloniaObject { }
                    }
                }
                """)
            .ApplyCodeFixesAsync(new PartialDependencyObjectAnalyzer(), new AddPartialModifierCodeFixProvider());

        Assert.Equal("""
            namespace Test;

            public static partial class Outer
            {
                internal partial class Middle
                {
                    public partial class Inner : Avalonia.AvaloniaObject { }
                }
            }
            """, fixedSource);
    }

    [Fact]
    public void CodeFix_ExposesFixAllProvider()
    {
        var provider = new AddPartialModifierCodeFixProvider();

        Assert.NotNull(provider.GetFixAllProvider());
        Assert.Equal(["XPORT001"], provider.FixableDiagnosticIds);
    }
}
