// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xaml.PortAnalyzers.Preprocessor
{
    /// <summary>
    /// Describes, for every position of a syntax tree, whether the code at that position would be compiled
    /// when the port target symbol is defined.
    /// </summary>
    /// <remarks>
    /// The map replays all conditional directives of the tree (including the ones nested in regions that are
    /// inactive for the current build) with the port target symbol defined, the other symbols of the current
    /// parse options defined, and <c>#define</c>/<c>#undef</c> directives applied in order. The result is stored
    /// as a sorted list of transitions so that queries are a binary search.
    /// </remarks>
    internal sealed class PortActivityMap
    {
        private static readonly PortActivityMap s_alwaysActive = new([], []);

        private readonly int[] _positions;
        private readonly bool[] _states;

        private PortActivityMap(int[] positions, bool[] states)
        {
            _positions = positions;
            _states = states;
        }

        /// <summary>
        /// Computes the map of a syntax tree.
        /// </summary>
        /// <param name="tree">The syntax tree.</param>
        /// <param name="targetSymbol">The preprocessor symbol that marks the port target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static PortActivityMap Create(SyntaxTree tree, string targetSymbol, CancellationToken cancellationToken)
        {
            var root = tree.GetRoot(cancellationToken);
            if (!root.ContainsDirectives || root is not CSharpSyntaxNode csharpRoot)
            {
                return s_alwaysActive;
            }

            var symbols = new HashSet<string>(tree.Options.PreprocessorSymbolNames, StringComparer.Ordinal)
            {
                targetSymbol,
            };

            var positions = new List<int>();
            var states = new List<bool>();
            var frames = new List<Frame>();
            var active = true;

            for (var directive = csharpRoot.GetFirstDirective(); directive is not null; directive = directive.GetNextDirective())
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (directive)
                {
                    case IfDirectiveTriviaSyntax ifDirective:
                    {
                        var taken = PreprocessorConditionEvaluator.Evaluate(ifDirective.Condition, symbols);
                        frames.Add(new Frame(active, taken));
                        active = active && taken;
                        break;
                    }

                    case ElifDirectiveTriviaSyntax elifDirective:
                    {
                        if (frames.Count == 0)
                        {
                            continue;
                        }

                        var frame = frames[frames.Count - 1];
                        var taken = !frame.BranchTaken && PreprocessorConditionEvaluator.Evaluate(elifDirective.Condition, symbols);
                        frames[frames.Count - 1] = new Frame(frame.ParentActive, frame.BranchTaken || taken);
                        active = frame.ParentActive && taken;
                        break;
                    }

                    case ElseDirectiveTriviaSyntax:
                    {
                        if (frames.Count == 0)
                        {
                            continue;
                        }

                        var frame = frames[frames.Count - 1];
                        frames[frames.Count - 1] = new Frame(frame.ParentActive, true);
                        active = frame.ParentActive && !frame.BranchTaken;
                        break;
                    }

                    case EndIfDirectiveTriviaSyntax:
                    {
                        if (frames.Count == 0)
                        {
                            continue;
                        }

                        active = frames[frames.Count - 1].ParentActive;
                        frames.RemoveAt(frames.Count - 1);
                        break;
                    }

                    case DefineDirectiveTriviaSyntax define:
                        if (active)
                        {
                            symbols.Add(define.Name.ValueText);
                        }

                        continue;

                    case UndefDirectiveTriviaSyntax undef:
                        if (active)
                        {
                            symbols.Remove(undef.Name.ValueText);
                        }

                        continue;

                    default:
                        continue;
                }

                AddTransition(positions, states, directive.FullSpan.End, active);
            }

            return positions.Count == 0 ? s_alwaysActive : new PortActivityMap(positions.ToArray(), states.ToArray());
        }

        /// <summary>
        /// Returns whether the code at <paramref name="position"/> is compiled for the port target.
        /// </summary>
        public bool IsActiveForPort(int position)
        {
            var positions = _positions;
            if (positions.Length == 0 || position < positions[0])
            {
                return true;
            }

            var low = 0;
            var high = positions.Length - 1;
            while (low < high)
            {
                var mid = low + ((high - low + 1) >> 1);
                if (positions[mid] <= position)
                {
                    low = mid;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return _states[low];
        }

        private static void AddTransition(List<int> positions, List<bool> states, int position, bool active)
        {
            var count = states.Count;
            var previous = count == 0 || states[count - 1];
            if (previous == active)
            {
                return;
            }

            if (count > 0 && positions[count - 1] == position)
            {
                states[count - 1] = active;
                return;
            }

            positions.Add(position);
            states.Add(active);
        }

        private readonly struct Frame
        {
            public Frame(bool parentActive, bool branchTaken)
            {
                ParentActive = parentActive;
                BranchTaken = branchTaken;
            }

            public bool ParentActive { get; }

            public bool BranchTaken { get; }
        }
    }
}
