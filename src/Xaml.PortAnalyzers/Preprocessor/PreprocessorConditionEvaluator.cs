// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xaml.PortAnalyzers.Preprocessor
{
    /// <summary>
    /// Evaluates <c>#if</c>/<c>#elif</c> conditions against an arbitrary set of defined symbols.
    /// </summary>
    internal static class PreprocessorConditionEvaluator
    {
        /// <summary>
        /// Evaluates a preprocessor condition.
        /// </summary>
        /// <param name="condition">The condition expression of an <c>#if</c> or <c>#elif</c> directive.</param>
        /// <param name="definedSymbols">The symbols considered defined.</param>
        /// <returns>The value of the condition; malformed conditions evaluate to <see langword="false"/>.</returns>
        public static bool Evaluate(ExpressionSyntax? condition, ISet<string> definedSymbols)
        {
            switch (condition)
            {
                case null:
                    return false;
                case IdentifierNameSyntax identifier:
                    return definedSymbols.Contains(identifier.Identifier.ValueText);
                case ParenthesizedExpressionSyntax parenthesized:
                    return Evaluate(parenthesized.Expression, definedSymbols);
                case PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.LogicalNotExpression):
                    return !Evaluate(unary.Operand, definedSymbols);
                case BinaryExpressionSyntax binary:
                    return EvaluateBinary(binary, definedSymbols);
                case LiteralExpressionSyntax literal:
                    return literal.IsKind(SyntaxKind.TrueLiteralExpression);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Collects the symbol identifiers used by a preprocessor condition.
        /// </summary>
        public static void CollectIdentifiers(ExpressionSyntax? condition, List<IdentifierNameSyntax> identifiers)
        {
            switch (condition)
            {
                case IdentifierNameSyntax identifier:
                    identifiers.Add(identifier);
                    break;
                case ParenthesizedExpressionSyntax parenthesized:
                    CollectIdentifiers(parenthesized.Expression, identifiers);
                    break;
                case PrefixUnaryExpressionSyntax unary:
                    CollectIdentifiers(unary.Operand, identifiers);
                    break;
                case BinaryExpressionSyntax binary:
                    CollectIdentifiers(binary.Left, identifiers);
                    CollectIdentifiers(binary.Right, identifiers);
                    break;
            }
        }

        private static bool EvaluateBinary(BinaryExpressionSyntax binary, ISet<string> definedSymbols)
        {
            switch (binary.Kind())
            {
                case SyntaxKind.LogicalAndExpression:
                    return Evaluate(binary.Left, definedSymbols) && Evaluate(binary.Right, definedSymbols);
                case SyntaxKind.LogicalOrExpression:
                    return Evaluate(binary.Left, definedSymbols) || Evaluate(binary.Right, definedSymbols);
                case SyntaxKind.EqualsExpression:
                    return Evaluate(binary.Left, definedSymbols) == Evaluate(binary.Right, definedSymbols);
                case SyntaxKind.NotEqualsExpression:
                    return Evaluate(binary.Left, definedSymbols) != Evaluate(binary.Right, definedSymbols);
                default:
                    return false;
            }
        }
    }
}
