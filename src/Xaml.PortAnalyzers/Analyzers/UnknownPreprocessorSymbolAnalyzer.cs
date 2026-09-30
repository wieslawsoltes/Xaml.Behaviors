// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Xaml.PortAnalyzers.Configuration;
using Xaml.PortAnalyzers.Preprocessor;

namespace Xaml.PortAnalyzers.Analyzers
{
    /// <summary>
    /// XPORT003: reports identifiers in <c>#if</c>/<c>#elif</c> conditions that are neither defined nor known
    /// platform symbols, which usually are typos such as <c>#if UNO_</c> or <c>#if !UNOO</c>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnknownPreprocessorSymbolAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(PortDiagnosticDescriptors.UnknownPreprocessorSymbol);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            var configuration = PortConfiguration.Create(context.Options.AnalyzerConfigOptionsProvider);
            context.RegisterSyntaxTreeAction(treeContext => AnalyzeTree(treeContext, configuration));
        }

        private static void AnalyzeTree(SyntaxTreeAnalysisContext context, PortConfiguration configuration)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (!root.ContainsDirectives || root is not CSharpSyntaxNode csharpRoot)
            {
                return;
            }

            var settings = configuration.GetTreeSettings(context.Tree);
            HashSet<string>? defined = null;
            List<IdentifierNameSyntax>? identifiers = null;

            for (var directive = csharpRoot.GetFirstDirective(); directive is not null; directive = directive.GetNextDirective())
            {
                ExpressionSyntax condition;
                switch (directive)
                {
                    case IfDirectiveTriviaSyntax ifDirective:
                        condition = ifDirective.Condition;
                        break;
                    case ElifDirectiveTriviaSyntax elifDirective:
                        condition = elifDirective.Condition;
                        break;
                    default:
                        continue;
                }

                identifiers ??= new List<IdentifierNameSyntax>();
                identifiers.Clear();
                PreprocessorConditionEvaluator.CollectIdentifiers(condition, identifiers);
                for (var i = 0; i < identifiers.Count; i++)
                {
                    var identifier = identifiers[i];
                    var symbol = identifier.Identifier.ValueText;
                    if (symbol.Length == 0 || settings.KnownSymbols.IsMatch(symbol))
                    {
                        continue;
                    }

                    defined ??= CollectDefinedSymbols(context.Tree, csharpRoot);
                    if (defined.Contains(symbol))
                    {
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(
                        PortDiagnosticDescriptors.UnknownPreprocessorSymbol,
                        identifier.GetLocation(),
                        symbol));
                }
            }
        }

        private static HashSet<string> CollectDefinedSymbols(SyntaxTree tree, CSharpSyntaxNode root)
        {
            var defined = new HashSet<string>(tree.Options.PreprocessorSymbolNames, StringComparer.Ordinal);
            for (var directive = root.GetFirstDirective(); directive is not null; directive = directive.GetNextDirective())
            {
                switch (directive)
                {
                    case DefineDirectiveTriviaSyntax define:
                        defined.Add(define.Name.ValueText);
                        break;
                    case UndefDirectiveTriviaSyntax undef:
                        defined.Add(undef.Name.ValueText);
                        break;
                }
            }

            return defined;
        }
    }
}
