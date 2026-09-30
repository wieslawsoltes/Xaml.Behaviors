// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xaml.PortAnalyzers.CodeFixes
{
    /// <summary>
    /// Fixes XPORT004 by casting the returned <c>GetValue(...)</c> call to the declared property type.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddGetValueCastCodeFixProvider))]
    [Shared]
    public sealed class AddGetValueCastCodeFixProvider : CodeFixProvider
    {
        private const string EquivalenceKey = "XamlPort.AddGetValueCast";

        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(DiagnosticIds.GetValueCast);

        /// <inheritdoc />
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: false) is not ExpressionSyntax expression)
                {
                    continue;
                }

                var property = expression.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
                if (property is null)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Cast GetValue result to '" + property.Type.WithoutTrivia().ToString() + "'",
                        cancellationToken => AddCastAsync(context.Document, expression, property.Type, cancellationToken),
                        EquivalenceKey),
                    diagnostic);
            }
        }

        /// <summary>
        /// Wraps an expression in a cast to the given type, keeping the expression's trivia.
        /// </summary>
        internal static CastExpressionSyntax CreateCast(ExpressionSyntax expression, TypeSyntax type)
        {
            return SyntaxFactory.CastExpression(type.WithoutTrivia(), expression.WithoutTrivia())
                .WithTriviaFrom(expression);
        }

        private static async Task<Document> AddCastAsync(Document document, ExpressionSyntax expression, TypeSyntax type, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            return document.WithSyntaxRoot(root.ReplaceNode(expression, CreateCast(expression, type)));
        }
    }
}
