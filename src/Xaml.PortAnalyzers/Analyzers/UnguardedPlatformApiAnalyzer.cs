// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Xaml.PortAnalyzers.Configuration;
using Xaml.PortAnalyzers.PortMaps;

namespace Xaml.PortAnalyzers.Analyzers
{
    /// <summary>
    /// XPORT002: reports references to source platform symbols that the port map does not declare as mapped
    /// (or declares as unsupported) in shared code that is compiled for the port target.
    /// XPORT005: reports invalid port map files.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnguardedPlatformApiAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(PortDiagnosticDescriptors.UnguardedPlatformApi, PortDiagnosticDescriptors.InvalidPortMap);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            var load = PortMapLoader.Load(context.Options.AdditionalFiles, context.CancellationToken);
            if (load.Errors.Count > 0)
            {
                var errors = load.Errors;
                context.RegisterAdditionalFileAction(fileContext => ReportPortMapErrors(fileContext, errors));
            }

            if (load.Map is null)
            {
                return;
            }

            var cache = new PortTreeCache(PortConfiguration.Create(context.Options.AnalyzerConfigOptionsProvider));
            var classifier = new PortSymbolClassifier(load.Map, cache.IsShared);
            var analysis = new Analysis(cache, classifier);
            context.RegisterSyntaxNodeAction(analysis.AnalyzeName, SyntaxKind.IdentifierName, SyntaxKind.GenericName);
            context.RegisterSyntaxNodeAction(analysis.AnalyzeUsing, SyntaxKind.UsingDirective);
        }

        private static void ReportPortMapErrors(AdditionalFileAnalysisContext context, IReadOnlyList<PortMapError> errors)
        {
            for (var i = 0; i < errors.Count; i++)
            {
                var error = errors[i];
                if (error.File.Path == context.AdditionalFile.Path)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        PortDiagnosticDescriptors.InvalidPortMap,
                        error.GetLocation(),
                        error.File.Path,
                        error.Message));
                }
            }
        }

        private sealed class Analysis
        {
            private readonly PortTreeCache _cache;
            private readonly PortSymbolClassifier _classifier;

            public Analysis(PortTreeCache cache, PortSymbolClassifier classifier)
            {
                _cache = cache;
                _classifier = classifier;
            }

            public void AnalyzeName(SyntaxNodeAnalysisContext context)
            {
                var name = (SimpleNameSyntax)context.Node;
                if (name is IdentifierNameSyntax { IsVar: true } || IsInUsingOrNamespaceName(name) || name.IsPartOfStructuredTrivia())
                {
                    return;
                }

                var tree = name.SyntaxTree;
                if (!_cache.IsShared(tree) || !_cache.IsActiveForPort(tree, name.SpanStart, context.CancellationToken))
                {
                    return;
                }

                var symbol = GetSymbol(context.SemanticModel.GetSymbolInfo(name, context.CancellationToken));
                if (symbol is null || symbol.Kind == SymbolKind.Namespace)
                {
                    return;
                }

                var classification = _classifier.Classify(symbol);
                if (classification.RequiresGuard)
                {
                    Report(context, name.Identifier.GetLocation(), classification, tree);
                    return;
                }

                if (classification.Status != PortSymbolStatus.Mapped)
                {
                    return;
                }

                // A mapped type may still be spelled with a namespace qualifier that does not exist on the port.
                var qualifier = GetQualifier(name);
                if (qualifier is null
                    || context.SemanticModel.GetSymbolInfo(qualifier, context.CancellationToken).Symbol is not INamespaceSymbol namespaceSymbol)
                {
                    return;
                }

                var namespaceClassification = _classifier.ClassifyNamespace(namespaceSymbol);
                if (namespaceClassification.RequiresGuard)
                {
                    Report(context, qualifier.GetLocation(), namespaceClassification, tree);
                }
            }

            public void AnalyzeUsing(SyntaxNodeAnalysisContext context)
            {
                var directive = (UsingDirectiveSyntax)context.Node;
                var name = directive.Name;
                if (name is null)
                {
                    return;
                }

                var tree = directive.SyntaxTree;
                if (!_cache.IsShared(tree) || !_cache.IsActiveForPort(tree, directive.SpanStart, context.CancellationToken))
                {
                    return;
                }

                var symbol = GetSymbol(context.SemanticModel.GetSymbolInfo(name, context.CancellationToken));
                if (symbol is null)
                {
                    return;
                }

                var classification = symbol is INamespaceSymbol namespaceSymbol
                    ? _classifier.ClassifyNamespace(namespaceSymbol)
                    : _classifier.Classify(symbol);
                if (classification.RequiresGuard)
                {
                    Report(context, name.GetLocation(), classification, tree);
                }
            }

            private void Report(SyntaxNodeAnalysisContext context, Location location, PortSymbolClassification classification, SyntaxTree tree)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    PortDiagnosticDescriptors.UnguardedPlatformApi,
                    location,
                    classification.Name,
                    classification.Reason,
                    _cache.GetSettings(tree).TargetSymbol));
            }

            private static ISymbol? GetSymbol(SymbolInfo info)
            {
                if (info.Symbol is not null)
                {
                    return info.Symbol;
                }

                return info.CandidateSymbols.Length > 0 ? info.CandidateSymbols[0] : null;
            }

            private static ExpressionSyntax? GetQualifier(SimpleNameSyntax name)
            {
                return name.Parent switch
                {
                    QualifiedNameSyntax qualified when qualified.Right == name => qualified.Left,
                    MemberAccessExpressionSyntax memberAccess when memberAccess.Name == name => memberAccess.Expression,
                    _ => null,
                };
            }

            private static bool IsInUsingOrNamespaceName(SyntaxNode node)
            {
                var current = node;
                while (current.Parent is NameSyntax)
                {
                    current = current.Parent;
                }

                return current.Parent is UsingDirectiveSyntax or BaseNamespaceDeclarationSyntax;
            }
        }
    }
}
