// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Xaml.PropertyGenerator.Migration;

namespace Xaml.PropertyGenerator.CodeFixes
{
    /// <summary>
    /// Converts hand-written Avalonia property registrations into generated partial properties (<c>XPG1001</c>).
    /// </summary>
    /// <remarks>
    /// The fix-all provider processes every diagnostic of a document in one pass, so the conversion can be applied
    /// deterministically to a whole solution with
    /// <c>dotnet format analyzers --diagnostics XPG1001 --severity info</c>.
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MigrationCodeFixProvider))]
    [Shared]
    public sealed class MigrationCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Use a generated property";
        private const string AttributeNamespace = "Xaml.PropertyGenerator";

        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MigrationAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public override FixAllProvider GetFixAllProvider()
            => FixAllProvider.Create(static (context, document, diagnostics) => FixDocumentAsync(document, diagnostics, context.CancellationToken)!);

        /// <inheritdoc />
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        Title,
                        ct => FixDocumentAsync(context.Document, ImmutableArray.Create(diagnostic), ct)!,
                        equivalenceKey: MigrationAnalyzer.DiagnosticId),
                    diagnostic);
            }

            return Task.CompletedTask;
        }

        private static async Task<Document?> FixDocumentAsync(Document document, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (root is null || model is null)
            {
                return document;
            }

            var candidates = new List<MigrationModel>();
            foreach (var diagnostic in diagnostics.OrderBy(static d => d.Location.SourceSpan.Start))
            {
                var field = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>();
                if (field is not null && MigrationModel.TryCreate(field, model, cancellationToken) is { } candidate)
                {
                    candidates.Add(candidate);
                }
            }

            if (candidates.Count == 0)
            {
                return document;
            }

            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var types = new HashSet<TypeDeclarationSyntax>();
            foreach (var candidate in candidates)
            {
                editor.RemoveNode(candidate.Registration, SyntaxRemoveOptions.KeepUnbalancedDirectives);
                if (candidate.BackingField is not null && candidate.BackingField.SyntaxTree == root.SyntaxTree)
                {
                    editor.RemoveNode(candidate.BackingField, SyntaxRemoveOptions.KeepUnbalancedDirectives);
                }

                editor.ReplaceNode(candidate.Property, BuildProperty(candidate));

                foreach (var type in candidate.Property.Ancestors().OfType<TypeDeclarationSyntax>())
                {
                    types.Add(type);
                }
            }

            foreach (var type in types)
            {
                if (!type.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    editor.ReplaceNode(type, static (node, _) => AddPartial((TypeDeclarationSyntax)node));
                }
            }

            var newRoot = editor.GetChangedRoot();
            if (!IsAttributeNamespaceInScope(model, candidates[0].Property.SpanStart) && newRoot is CompilationUnitSyntax unit)
            {
                newRoot = AddUsing(unit);
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax BuildProperty(MigrationModel candidate)
        {
            var property = candidate.Property;
            var leading = property.GetLeadingTrivia();
            var indent = leading.LastOrDefault(static t => t.IsKind(SyntaxKind.WhitespaceTrivia)).ToString();
            var eol = property.SyntaxTree.GetText().ToString().Contains("\r\n") ? "\r\n" : "\n";

            var text = new StringBuilder();
            foreach (var list in property.AttributeLists)
            {
                var kept = list.Attributes.Where(a => !candidate.RemovedAttributes.Contains(a)).ToList();
                if (kept.Count == 0)
                {
                    continue;
                }

                text.Append(text.Length == 0 ? string.Empty : indent)
                    .Append('[')
                    .Append(list.Target is null ? string.Empty : list.Target.ToString() + " ")
                    .Append(string.Join(", ", kept.Select(static a => a.ToString())))
                    .Append(']')
                    .Append(eol);
            }

            text.Append(text.Length == 0 ? string.Empty : indent).Append('[').Append(candidate.AttributeName);
            if (candidate.AttributeArguments.Count > 0)
            {
                text.Append('(').Append(string.Join(", ", candidate.AttributeArguments)).Append(')');
            }

            text.Append(']').Append(eol);

            text.Append(indent);
            if (property.Modifiers.Count > 0)
            {
                text.Append(property.Modifiers.ToString()).Append(' ');
            }

            text.Append("partial ").Append(property.Type.ToString()).Append(' ').Append(property.Identifier.ValueText).Append(" { ")
                .Append(candidate.GetterModifiers).Append("get; ");
            if (candidate.SetterModifiers is not null)
            {
                text.Append(candidate.SetterModifiers).Append(candidate.SetterIsInit ? "init; " : "set; ");
            }

            text.Append('}');

            var member = SyntaxFactory.ParseMemberDeclaration(text.ToString())!;
            return member
                .WithLeadingTrivia(leading)
                .WithTrailingTrivia(property.GetTrailingTrivia());
        }

        private static TypeDeclarationSyntax AddPartial(TypeDeclarationSyntax type)
        {
            if (type.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return type;
            }

            if (type.Modifiers.Count == 0)
            {
                var keyword = type.Keyword;
                var partial = SyntaxFactory.Token(keyword.LeadingTrivia, SyntaxKind.PartialKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space));
                return type
                    .WithKeyword(keyword.WithLeadingTrivia(SyntaxTriviaList.Empty))
                    .WithModifiers(SyntaxFactory.TokenList(partial));
            }

            return type.WithModifiers(type.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        }

        private static bool IsAttributeNamespaceInScope(SemanticModel model, int position)
        {
            foreach (var symbol in model.LookupNamespacesAndTypes(position, name: "StyledPropertyAttribute"))
            {
                if (symbol.ContainingNamespace?.ToDisplayString() == AttributeNamespace)
                {
                    return true;
                }
            }

            return false;
        }

        private static CompilationUnitSyntax AddUsing(CompilationUnitSyntax unit)
        {
            var eol = unit.ToFullString().Contains("\r\n") ? SyntaxFactory.CarriageReturnLineFeed : SyntaxFactory.LineFeed;
            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(AttributeNamespace))
                .WithUsingKeyword(SyntaxFactory.Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(SyntaxFactory.Space))
                .WithTrailingTrivia(eol);

            // Insert before the first using that starts a preprocessor region so the directive stays unconditional.
            var usings = unit.Usings;
            var index = usings.Count;
            for (var i = 0; i < usings.Count; i++)
            {
                if (usings[i].GetLeadingTrivia().Any(static t => t.IsDirective))
                {
                    index = i;
                    break;
                }
            }

            if (index == 0 && usings.Count > 0)
            {
                // Keep the file header on the new first line and the directives on the old first using.
                var leading = usings[0].GetLeadingTrivia();
                var split = 0;
                while (split < leading.Count && !leading[split].IsDirective)
                {
                    split++;
                }

                directive = directive.WithLeadingTrivia(leading.Take(split));
                usings = usings.Replace(usings[0], usings[0].WithLeadingTrivia(leading.Skip(split)));
                return unit.WithUsings(usings.Insert(0, directive));
            }

            if (usings.Count == 0)
            {
                var firstToken = unit.GetFirstToken();
                var leading = firstToken.LeadingTrivia;
                var split = 0;
                while (split < leading.Count && !leading[split].IsDirective)
                {
                    split++;
                }

                directive = directive.WithLeadingTrivia(leading.Take(split)).WithTrailingTrivia(eol, eol);
                unit = unit.ReplaceToken(firstToken, firstToken.WithLeadingTrivia(leading.Skip(split)));
                return unit.WithUsings(unit.Usings.Add(directive));
            }

            return unit.WithUsings(usings.Insert(index, directive));
        }
    }
}
