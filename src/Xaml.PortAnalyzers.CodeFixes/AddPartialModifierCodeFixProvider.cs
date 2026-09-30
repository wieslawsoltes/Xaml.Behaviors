// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
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
    /// Fixes XPORT001 by adding the <c>partial</c> modifier to the type and to all of its containing types.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddPartialModifierCodeFixProvider))]
    [Shared]
    public sealed class AddPartialModifierCodeFixProvider : CodeFixProvider
    {
        private const string EquivalenceKey = "XamlPort.AddPartialModifier";

        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(DiagnosticIds.PartialDependencyObject);

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
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.Parent is not TypeDeclarationSyntax declaration)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Make type partial",
                        cancellationToken => AddPartialAsync(context.Document, declaration, cancellationToken),
                        EquivalenceKey),
                    diagnostic);
            }
        }

        /// <summary>
        /// Adds <c>partial</c> to a type declaration, keeping its leading trivia, and places it right before the
        /// type keyword as the language requires.
        /// </summary>
        internal static TypeDeclarationSyntax AddPartialModifier(TypeDeclarationSyntax declaration)
        {
            var modifiers = declaration.Modifiers;
            for (var i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].IsKind(SyntaxKind.PartialKeyword))
                {
                    return declaration;
                }
            }

            if (modifiers.Count == 0)
            {
                var keyword = declaration.Keyword;
                var partialToken = SyntaxFactory.Token(keyword.LeadingTrivia, SyntaxKind.PartialKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space));
                return declaration
                    .WithKeyword(keyword.WithLeadingTrivia(SyntaxFactory.TriviaList()))
                    .WithModifiers(SyntaxFactory.TokenList(partialToken));
            }

            return declaration.WithModifiers(modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        }

        private static async Task<Document> AddPartialAsync(Document document, TypeDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document;
            }

            var targets = new List<TypeDeclarationSyntax>();
            for (SyntaxNode? node = declaration; node is not null; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax typeDeclaration)
                {
                    targets.Add(typeDeclaration);
                }
            }

            var newRoot = root.ReplaceNodes(targets, static (_, rewritten) => AddPartialModifier(rewritten));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
