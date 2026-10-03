// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// A set of preprocessor symbol names and wildcard patterns (<c>*</c> and <c>?</c>), matched case-sensitively
    /// like the C# preprocessor does.
    /// </summary>
    internal sealed class SymbolPatternSet
    {
        // Symbols that are known to be used in platform conditions of Avalonia and Uno Platform code.
        private static readonly string[] s_defaultKnownSymbols =
        [
            "DEBUG", "RELEASE", "TRACE",
            "NET*", "NETSTANDARD*", "NETCOREAPP*", "NETFRAMEWORK*", "*_OR_GREATER",
            "DOCFX",
            "UNO", "HAS_UNO*", "__UNO*",
            "__ANDROID__", "__IOS__", "__MACOS__", "__MACCATALYST__", "__WASM__", "__SKIA__", "__TVOS__",
            "WINAPPSDK", "WINDOWS_UWP", "WINUI",
            "AVALONIA",
            "WINDOWS", "ANDROID", "IOS", "MACCATALYST", "MACOS", "TVOS", "BROWSER", "WASI", "LINUX", "OSX", "FREEBSD",
        ];

        private static readonly char[] s_separators = [';', ',', '|', ' ', '\t', '\r', '\n'];

        private readonly HashSet<string> _exact;
        private readonly string[] _wildcards;

        private SymbolPatternSet(HashSet<string> exact, string[] wildcards)
        {
            _exact = exact;
            _wildcards = wildcards;
        }

        /// <summary>
        /// Creates a set from the default known symbols, the port target symbol and a user supplied list.
        /// </summary>
        /// <param name="targetSymbol">The port target symbol.</param>
        /// <param name="additional">Additional symbols separated by <c>;</c>, <c>,</c>, <c>|</c> or white space.</param>
        public static SymbolPatternSet CreateKnown(string targetSymbol, string? additional)
        {
            var exact = new HashSet<string>(StringComparer.Ordinal);
            var wildcards = new List<string>();
            for (var i = 0; i < s_defaultKnownSymbols.Length; i++)
            {
                Add(s_defaultKnownSymbols[i], exact, wildcards);
            }

            Add(targetSymbol, exact, wildcards);
            if (additional is not null)
            {
                var parts = additional.Split(s_separators, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < parts.Length; i++)
                {
                    Add(parts[i], exact, wildcards);
                }
            }

            return new SymbolPatternSet(exact, wildcards.ToArray());
        }

        /// <summary>
        /// Returns whether the symbol matches one of the names or patterns.
        /// </summary>
        public bool IsMatch(string symbol)
        {
            if (_exact.Contains(symbol))
            {
                return true;
            }

            for (var i = 0; i < _wildcards.Length; i++)
            {
                if (WildcardMatch(_wildcards[i], 0, symbol, 0))
                {
                    return true;
                }
            }

            return false;
        }

        private static void Add(string value, HashSet<string> exact, List<string> wildcards)
        {
            var trimmed = value.Trim();
            if (trimmed.Length == 0)
            {
                return;
            }

            if (trimmed.IndexOf('*') >= 0 || trimmed.IndexOf('?') >= 0)
            {
                wildcards.Add(trimmed);
            }
            else
            {
                exact.Add(trimmed);
            }
        }

        private static bool WildcardMatch(string pattern, int pi, string text, int ti)
        {
            while (pi < pattern.Length)
            {
                var pc = pattern[pi];
                if (pc == '*')
                {
                    for (var k = ti; k <= text.Length; k++)
                    {
                        if (WildcardMatch(pattern, pi + 1, text, k))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                if (ti >= text.Length || (pc != '?' && pc != text[ti]))
                {
                    return false;
                }

                pi++;
                ti++;
            }

            return ti == text.Length;
        }
    }
}
