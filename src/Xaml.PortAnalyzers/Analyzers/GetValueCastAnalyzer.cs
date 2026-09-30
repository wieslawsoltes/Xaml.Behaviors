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
    /// XPORT004: reports shared property getters that return <c>GetValue(...)</c> without a cast.
    /// </summary>
    /// <remarks>
    /// Avalonia's <c>GetValue&lt;T&gt;(StyledProperty&lt;T&gt;)</c> returns <c>T</c> while WinUI's
    /// <c>GetValue(DependencyProperty)</c> returns <c>object</c>, so the shared getter needs an explicit cast
    /// (redundant but harmless on Avalonia).
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class GetValueCastAnalyzer : DiagnosticAnalyzer
    {
        private const string GetValueMethodName = "GetValue";

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(PortDiagnosticDescriptors.GetValueCast);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        /// <summary>
        /// Returns whether an expression is a (possibly parenthesized or null-forgiven) <c>GetValue(...)</c> call.
        /// </summary>
        internal static bool IsUncastGetValueInvocation(ExpressionSyntax? expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case ParenthesizedExpressionSyntax parenthesized:
                        expression = parenthesized.Expression;
                        continue;
                    case PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.SuppressNullableWarningExpression):
                        expression = postfix.Operand;
                        continue;
                    case InvocationExpressionSyntax invocation:
                        return GetInvokedName(invocation.Expression) == GetValueMethodName;
                    default:
                        return false;
                }
            }
        }

        private static string? GetInvokedName(ExpressionSyntax expression)
        {
            return expression switch
            {
                SimpleNameSyntax name => name.Identifier.ValueText,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
                MemberBindingExpressionSyntax memberBinding => memberBinding.Name.Identifier.ValueText,
                _ => null,
            };
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            var cache = new PortTreeCache(PortConfiguration.Create(context.Options.AnalyzerConfigOptionsProvider));
            var analysis = new Analysis(cache);
            context.RegisterSyntaxNodeAction(analysis.AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        private sealed class Analysis
        {
            private readonly PortTreeCache _cache;

            public Analysis(PortTreeCache cache)
            {
                _cache = cache;
            }

            public void AnalyzeProperty(SyntaxNodeAnalysisContext context)
            {
                var property = (PropertyDeclarationSyntax)context.Node;
                if (!_cache.IsShared(property.SyntaxTree) || IsObjectType(property.Type))
                {
                    return;
                }

                if (property.ExpressionBody is not null)
                {
                    Check(context, property, property.ExpressionBody.Expression);
                    return;
                }

                if (property.AccessorList is null)
                {
                    return;
                }

                foreach (var accessor in property.AccessorList.Accessors)
                {
                    if (!accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        continue;
                    }

                    if (accessor.ExpressionBody is not null)
                    {
                        Check(context, property, accessor.ExpressionBody.Expression);
                    }
                    else if (accessor.Body is not null)
                    {
                        CheckStatements(context, property, accessor.Body);
                    }
                }
            }

            private void CheckStatements(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, SyntaxNode node)
            {
                foreach (var child in node.ChildNodes())
                {
                    switch (child)
                    {
                        case ReturnStatementSyntax returnStatement:
                            Check(context, property, returnStatement.Expression);
                            break;
                        case StatementSyntax statement when statement is not LocalFunctionStatementSyntax:
                            CheckStatements(context, property, statement);
                            break;
                        case ElseClauseSyntax or SwitchSectionSyntax or CatchClauseSyntax or FinallyClauseSyntax:
                            CheckStatements(context, property, child);
                            break;
                    }
                }
            }

            private void Check(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, ExpressionSyntax? expression)
            {
                if (expression is null || !IsUncastGetValueInvocation(expression))
                {
                    return;
                }

                if (!_cache.IsActiveForPort(expression.SyntaxTree, expression.SpanStart, context.CancellationToken))
                {
                    return;
                }

                var propertySymbol = context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken);
                var propertyType = propertySymbol?.Type;
                if (propertyType is null
                    || propertyType.SpecialType == SpecialType.System_Object
                    || propertyType.TypeKind is TypeKind.Dynamic or TypeKind.Error)
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(
                    PortDiagnosticDescriptors.GetValueCast,
                    expression.GetLocation(),
                    property.Identifier.ValueText,
                    property.Type.ToString()));
            }

            private static bool IsObjectType(TypeSyntax type)
            {
                if (type is NullableTypeSyntax nullable)
                {
                    type = nullable.ElementType;
                }

                return type is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.ObjectKeyword);
            }
        }
    }
}
