// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Xaml.PortAnalyzers.Analyzers;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Analyzers;

public class UnknownPreprocessorSymbolAnalyzerTests
{
    [Fact]
    public async Task Reports_TyposInConditions()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                #if UNOO
                #endif
                #if !UNO_
                #endif
                #if UNO
                #elif NETT8_0 || (DEBUG && !TRACE)
                #endif
                public class A { }
                """)
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Equal(["XPORT003:1:'UNOO'", "XPORT003:3:'UNO_'"], diagnostics.Describe());
        Assert.Equal(
            "Preprocessor symbol 'UNOO' is not defined and is not a known platform symbol; check it for typos or add it to 'XamlPortKnownSymbols'",
            diagnostics[0].GetMessage());
    }

    [Fact]
    public async Task Reports_UnknownSymbolsInElifAndInactiveRegions()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                #if UNO
                #if WINDOWSS
                #endif
                #elif ANDROIDD
                #endif
                public class A { }
                """)
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Equal(["XPORT003:2:'WINDOWSS'", "XPORT003:4:'ANDROIDD'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Accepts_KnownDefinedAndLocallyDefinedSymbols()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                #define LOCAL
                #undef REMOVED
                #if LOCAL || REMOVED || MY_FEATURE || NET10_0_OR_GREATER || NETSTANDARD2_0 || HAS_UNO_WINUI || __SKIA__ || true
                #endif
                public class A { }
                """)
            .WithPreprocessorSymbols("MY_FEATURE")
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Accepts_ConfiguredKnownSymbolsAndTargetSymbol()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithGlobalOption("build_property.XamlPortTargetSymbol", "MYPORT")
            .WithGlobalOption("build_property.XamlPortKnownSymbols", "FEATURE_*;EXTRA")
            .WithSource("A.cs", """
                #if MYPORT || FEATURE_X || EXTRA || PER_FILE
                #endif
                public class A { }
                """)
            .WithFileOption("A.cs", "xaml_port.known_symbols", "PER_FILE")
            .WithSource("B.cs", """
                #if PER_FILE || MYPORTT
                #endif
                public class B { }
                """)
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Equal(["XPORT003:1:'PER_FILE'", "XPORT003:1:'MYPORTT'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Applies_ToFilesThatAreNotShared()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithGlobalOption("build_property.XamlPortSharedSourceExcludes", "**")
            .WithSource("#if UNOO\n#endif\npublic class A { }")
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Single(diagnostics);
    }

    [Fact]
    public async Task Ignores_FilesWithoutDirectives()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("#region Test\npublic class A { }\n#endregion")
            .GetDiagnosticsAsync(new UnknownPreprocessorSymbolAnalyzer());

        Assert.Empty(diagnostics);
    }
}
