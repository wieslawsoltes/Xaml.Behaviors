// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Xaml.PortAnalyzers.Configuration;

namespace Xaml.PortAnalyzers.Analyzers
{
    /// <summary>
    /// XPORT001: reports shared classes deriving directly from a dependency object that are not partial.
    /// </summary>
    /// <remarks>
    /// On Uno Platform <c>DependencyObject</c> is an interface whose implementation is emitted by a source generator
    /// as another partial declaration of every type deriving directly from it (and of its containing types).
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PartialDependencyObjectAnalyzer : DiagnosticAnalyzer
    {
        private const string AvaloniaObjectMetadataName = "Avalonia.AvaloniaObject";
        private const string WinUIDependencyObjectMetadataName = "Microsoft.UI.Xaml.DependencyObject";

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(PortDiagnosticDescriptors.PartialDependencyObject);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            var avaloniaObject = context.Compilation.GetTypeByMetadataName(AvaloniaObjectMetadataName);
            var dependencyObject = context.Compilation.GetTypeByMetadataName(WinUIDependencyObjectMetadataName);
            if (avaloniaObject is null && dependencyObject is null)
            {
                return;
            }

            var cache = new PortTreeCache(PortConfiguration.Create(context.Options.AnalyzerConfigOptionsProvider));
            var analysis = new Analysis(cache, avaloniaObject, dependencyObject);
            context.RegisterSymbolAction(analysis.AnalyzeNamedType, SymbolKind.NamedType);
        }

        private sealed class Analysis
        {
            private readonly PortTreeCache _cache;
            private readonly INamedTypeSymbol? _avaloniaObject;
            private readonly INamedTypeSymbol? _dependencyObject;

            public Analysis(PortTreeCache cache, INamedTypeSymbol? avaloniaObject, INamedTypeSymbol? dependencyObject)
            {
                _cache = cache;
                _avaloniaObject = avaloniaObject;
                _dependencyObject = dependencyObject;
            }

            public void AnalyzeNamedType(SymbolAnalysisContext context)
            {
                var type = (INamedTypeSymbol)context.Symbol;
                if (type.TypeKind != TypeKind.Class)
                {
                    return;
                }

                var baseSymbol = GetDirectDependencyObjectBase(type);
                if (baseSymbol is null)
                {
                    return;
                }

                var references = type.DeclaringSyntaxReferences;
                for (var i = 0; i < references.Length; i++)
                {
                    var reference = references[i];
                    var tree = reference.SyntaxTree;
                    if (!_cache.IsShared(tree))
                    {
                        continue;
                    }

                    if (reference.GetSyntax(context.CancellationToken) is not ClassDeclarationSyntax declaration)
                    {
                        continue;
                    }

                    if (!_cache.IsActiveForPort(tree, declaration.Identifier.SpanStart, context.CancellationToken))
                    {
                        continue;
                    }

                    var nonPartial = FindFirstNonPartialDeclaration(declaration);
                    if (nonPartial is null)
                    {
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(
                        PortDiagnosticDescriptors.PartialDependencyObject,
                        declaration.Identifier.GetLocation(),
                        type.Name,
                        baseSymbol.Name,
                        _cache.GetSettings(tree).TargetSymbol,
                        nonPartial.Identifier.ValueText));
                }
            }

            private INamedTypeSymbol? GetDirectDependencyObjectBase(INamedTypeSymbol type)
            {
                var baseType = type.BaseType?.OriginalDefinition;
                if (baseType is not null
                    && (SymbolEqualityComparer.Default.Equals(baseType, _avaloniaObject) || SymbolEqualityComparer.Default.Equals(baseType, _dependencyObject)))
                {
                    return baseType;
                }

                if (_dependencyObject is not null && _dependencyObject.TypeKind == TypeKind.Interface)
                {
                    var interfaces = type.Interfaces;
                    for (var i = 0; i < interfaces.Length; i++)
                    {
                        if (SymbolEqualityComparer.Default.Equals(interfaces[i].OriginalDefinition, _dependencyObject))
                        {
                            return _dependencyObject;
                        }
                    }
                }

                return null;
            }

            private static TypeDeclarationSyntax? FindFirstNonPartialDeclaration(ClassDeclarationSyntax declaration)
            {
                // Report the innermost declaration first, then containing types.
                for (SyntaxNode? node = declaration; node is not null; node = node.Parent)
                {
                    if (node is TypeDeclarationSyntax typeDeclaration && !IsPartial(typeDeclaration))
                    {
                        return typeDeclaration;
                    }
                }

                return null;
            }

            private static bool IsPartial(TypeDeclarationSyntax declaration)
            {
                var modifiers = declaration.Modifiers;
                for (var i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].IsKind(SyntaxKind.PartialKeyword))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
