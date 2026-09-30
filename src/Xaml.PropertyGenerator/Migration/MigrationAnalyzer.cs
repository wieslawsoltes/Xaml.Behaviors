// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xaml.PropertyGenerator.Migration
{
    /// <summary>
    /// Reports hand-written Avalonia property registrations that can be replaced by generated properties
    /// (<c>XPG1001</c>). The accompanying code fix performs the conversion.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MigrationAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>The diagnostic identifier.</summary>
        public const string DiagnosticId = "XPG1001";

        /// <summary>Diagnostic property holding the name of the CLR property to convert.</summary>
        public const string PropertyNameKey = "PropertyName";

        internal static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use a generated property",
            "Property '{0}' can be declared with [{1}] and generated for every supported XAML platform",
            "XamlPropertyGenerator",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/wieslawsoltes/Xaml.Behaviors/blob/master/src/Xaml.PropertyGenerator/README.md#xpg1001");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(static start =>
            {
                if (start.Compilation.GetTypeByMetadataName(AttributeSources.StyledPropertyAttributeName) is null ||
                    start.Compilation.GetTypeByMetadataName(Parser.AvaloniaObjectName) is null)
                {
                    return;
                }

                start.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
            });
        }

        private static void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            var field = (FieldDeclarationSyntax)context.Node;
            var candidate = MigrationModel.TryCreate(field, context.SemanticModel, context.CancellationToken);
            if (candidate is null)
            {
                return;
            }

            var properties = ImmutableDictionary<string, string?>.Empty.Add(PropertyNameKey, candidate.Property.Identifier.ValueText);
            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                field.Declaration.Variables[0].Identifier.GetLocation(),
                properties,
                candidate.Property.Identifier.ValueText,
                candidate.IsDirect ? "DirectProperty" : "StyledProperty"));
        }
    }
}
