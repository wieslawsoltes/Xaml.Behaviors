// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xaml.PortAnalyzers.Preprocessor;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Preprocessor;

public class PortActivityMapTests
{
    /// <summary>
    /// Parses the source for the current (Avalonia) build and returns, for every marker method that exists in the
    /// tree, whether it would be compiled for the port target.
    /// </summary>
    private static Dictionary<string, bool> Analyze(string source, params string[] symbols)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: symbols));
        var map = PortActivityMap.Create(tree, "UNO", CancellationToken.None);
        return tree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .ToDictionary(m => m.Identifier.ValueText, m => map.IsActiveForPort(m.SpanStart), StringComparer.Ordinal);
    }

    [Fact]
    public void Create_TreatsCodeWithoutDirectivesAsActive()
    {
        var result = Analyze("class C { void A() {} }");

        Assert.True(result["A"]);
    }

    [Fact]
    public void Create_HandlesIfNotTargetAndElse()
    {
        var result = Analyze("""
            class C
            {
            #if !UNO
                void AvaloniaOnly() {}
            #endif
                void Shared() {}
            #if UNO
                void UnoOnly() {}
            #else
                void ElseBranch() {}
            #endif
                void After() {}
            }
            """);

        Assert.False(result["AvaloniaOnly"]);
        Assert.True(result["Shared"]);
        Assert.False(result.ContainsKey("UnoOnly"));
        Assert.False(result["ElseBranch"]);
        Assert.True(result["After"]);
    }

    [Fact]
    public void Create_HandlesElifChains()
    {
        var result = Analyze("""
            class C
            {
            #if WINDOWS
                void Windows() {}
            #elif UNO
                void Uno() {}
            #elif !UNO
                void NotUno() {}
            #else
                void Other() {}
            #endif
            }
            """);

        Assert.False(result.ContainsKey("Windows"));
        Assert.False(result.ContainsKey("Uno"));
        Assert.False(result["NotUno"]);
        Assert.False(result.ContainsKey("Other"));
    }

    [Fact]
    public void Create_EvaluatesOtherSymbolsAsCurrentlyDefined()
    {
        var result = Analyze("""
            class C
            {
            #if NET8_0_OR_GREATER && !UNO
                void NetAvalonia() {}
            #endif
            #if NET8_0_OR_GREATER || UNO
                void NetOrUno() {}
            #endif
            #if (DEBUG || NET8_0_OR_GREATER) && UNO == false
                void Parenthesized() {}
            #endif
            #if UNO != true
                void NotEqual() {}
            #endif
            #if true
                void True() {}
            #endif
            }
            """, "NET8_0_OR_GREATER");

        Assert.False(result["NetAvalonia"]);
        Assert.True(result["NetOrUno"]);
        Assert.False(result["Parenthesized"]);
        Assert.False(result["NotEqual"]);
        Assert.True(result["True"]);
    }

    [Fact]
    public void Create_HandlesNestingInsideInactiveRegions()
    {
        var result = Analyze("""
            class C
            {
            #if !UNO
                void Outer() {}
            #if DEBUG
                void NestedDebug() {}
            #else
                void NestedElse() {}
            #endif
                void OuterAfter() {}
            #endif
            #if UNO
            #if DEBUG
                void UnoDebug() {}
            #endif
            #else
                void AfterNested() {}
            #endif
                void End() {}
            }
            """);

        Assert.False(result["Outer"]);
        Assert.False(result["NestedElse"]);
        Assert.False(result["OuterAfter"]);
        Assert.False(result["AfterNested"]);
        Assert.True(result["End"]);
    }

    [Fact]
    public void Create_AppliesDefineAndUndef()
    {
        var result = Analyze("""
            #define SHARED_FEATURE
            #undef UNO
            class C
            {
            #if SHARED_FEATURE && !UNO
                void Both() {}
            #endif
            }
            """);

        Assert.True(result["Both"]);
    }

    [Fact]
    public void Create_IgnoresUnbalancedDirectives()
    {
        var tree = CSharpSyntaxTree.ParseText("""
            class C
            {
            #endif
            #else
            #elif X
                void A() {}
            }
            """);

        var map = PortActivityMap.Create(tree, "UNO", CancellationToken.None);

        Assert.True(map.IsActiveForPort(tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single().SpanStart));
    }

    [Theory]
    [InlineData("A", true)]
    [InlineData("!A", false)]
    [InlineData("A && B", false)]
    [InlineData("A || B", true)]
    [InlineData("(A || B) && !B", true)]
    [InlineData("A == true", true)]
    [InlineData("A != B", true)]
    [InlineData("false", false)]
    [InlineData("true", true)]
    public void Evaluate_SupportsAllOperators(string condition, bool expected)
    {
        var tree = CSharpSyntaxTree.ParseText("#if " + condition + "\n#endif\n");
        var directive = (IfDirectiveTriviaSyntax)tree.GetRoot().GetFirstDirective()!;

        var result = PreprocessorConditionEvaluator.Evaluate(directive.Condition, new HashSet<string> { "A" });

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Evaluate_ReturnsFalseForNull()
    {
        Assert.False(PreprocessorConditionEvaluator.Evaluate(null, new HashSet<string>()));
    }

    [Fact]
    public void CollectIdentifiers_ReturnsAllSymbols()
    {
        var tree = CSharpSyntaxTree.ParseText("#if !(A && B) || C == true\n#endif\n");
        var directive = (IfDirectiveTriviaSyntax)tree.GetRoot().GetFirstDirective()!;
        var identifiers = new List<IdentifierNameSyntax>();

        PreprocessorConditionEvaluator.CollectIdentifiers(directive.Condition, identifiers);

        Assert.Equal(["A", "B", "C"], identifiers.Select(i => i.Identifier.ValueText));
    }
}
