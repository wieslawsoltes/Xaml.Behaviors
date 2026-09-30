// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Text;

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// A file glob relative to a project directory, following MSBuild item semantics:
    /// <c>*</c> matches any characters inside a path segment, <c>?</c> matches a single character
    /// inside a path segment and <c>**</c> matches any number of path segments (including none).
    /// Matching is case-insensitive and treats <c>\</c> and <c>/</c> as the same separator.
    /// </summary>
    internal sealed class GlobPattern
    {
        // '|' is accepted because the generated analyzer config file treats ';' as the start of a comment.
        private static readonly char[] s_separators = [';', '|'];

        private readonly string _pattern;

        private GlobPattern(string pattern)
        {
            _pattern = pattern;
        }

        /// <summary>
        /// Gets the normalized pattern text.
        /// </summary>
        public string Pattern => _pattern;

        /// <summary>
        /// Parses a list of globs separated by <c>;</c> or <c>|</c>. Blank entries are ignored.
        /// </summary>
        public static GlobPattern[] ParseList(string? value)
        {
            if (value is null || value.Length == 0)
            {
                return [];
            }

            var result = new List<GlobPattern>();
            var parts = value.Split(s_separators);
            for (var i = 0; i < parts.Length; i++)
            {
                var pattern = Create(parts[i]);
                if (pattern is not null)
                {
                    result.Add(pattern);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Creates a normalized glob or returns <see langword="null"/> for a blank pattern.
        /// </summary>
        public static GlobPattern? Create(string? raw)
        {
            if (raw is null)
            {
                return null;
            }

            var text = Unescape(raw.Trim()).Replace('\\', '/');
            while (text.StartsWith("./", System.StringComparison.Ordinal))
            {
                text = text.Substring(2);
            }

            if (text.Length == 0)
            {
                return null;
            }

            if (text[text.Length - 1] == '/')
            {
                text += "**";
            }

            return new GlobPattern(text);
        }

        /// <summary>
        /// Normalizes a path to forward slashes.
        /// </summary>
        public static string NormalizePath(string path) => path.Replace('\\', '/');

        /// <summary>
        /// Returns whether the relative (forward-slash separated) path matches the pattern.
        /// </summary>
        public bool IsMatch(string relativePath) => Match(_pattern, 0, relativePath, 0);

        private static bool Match(string pattern, int pi, string path, int si)
        {
            while (pi < pattern.Length)
            {
                var pc = pattern[pi];
                if (pc == '*')
                {
                    if (pi + 1 < pattern.Length && pattern[pi + 1] == '*')
                    {
                        return MatchGlobStar(pattern, pi, path, si);
                    }

                    // '*' matches any run of characters inside the current segment.
                    for (var k = si; ; k++)
                    {
                        if (Match(pattern, pi + 1, path, k))
                        {
                            return true;
                        }

                        if (k >= path.Length || IsSeparator(path[k]))
                        {
                            return false;
                        }
                    }
                }

                if (si >= path.Length)
                {
                    return false;
                }

                var c = path[si];
                if (pc == '?')
                {
                    if (IsSeparator(c))
                    {
                        return false;
                    }
                }
                else if (pc == '/')
                {
                    if (!IsSeparator(c))
                    {
                        return false;
                    }
                }
                else if (char.ToUpperInvariant(pc) != char.ToUpperInvariant(c))
                {
                    return false;
                }

                pi++;
                si++;
            }

            return si == path.Length;
        }

        private static bool MatchGlobStar(string pattern, int pi, string path, int si)
        {
            var next = pi + 2;
            if (next < pattern.Length && pattern[next] == '/')
            {
                // "**/" matches zero or more complete segments.
                next++;
                if (Match(pattern, next, path, si))
                {
                    return true;
                }

                for (var k = si; k < path.Length; k++)
                {
                    if (IsSeparator(path[k]) && Match(pattern, next, path, k + 1))
                    {
                        return true;
                    }
                }

                return false;
            }

            // A trailing "**" or "**" glued to other characters matches anything, separators included.
            for (var k = si; k <= path.Length; k++)
            {
                if (Match(pattern, next, path, k))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSeparator(char c) => c == '/' || c == '\\';

        private static string Unescape(string value)
        {
            // MSBuild escapes special characters (e.g. '*' as %2A) when values pass through property functions.
            if (value.IndexOf('%') < 0)
            {
                return value;
            }

            var builder = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '%' && i + 2 < value.Length && TryHex(value[i + 1], out var high) && TryHex(value[i + 2], out var low))
                {
                    builder.Append((char)((high << 4) | low));
                    i += 2;
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static bool TryHex(char c, out int value)
        {
            if (c >= '0' && c <= '9')
            {
                value = c - '0';
                return true;
            }

            if (c >= 'a' && c <= 'f')
            {
                value = c - 'a' + 10;
                return true;
            }

            if (c >= 'A' && c <= 'F')
            {
                value = c - 'A' + 10;
                return true;
            }

            value = 0;
            return false;
        }

        /// <inheritdoc />
        public override string ToString() => _pattern;
    }
}
