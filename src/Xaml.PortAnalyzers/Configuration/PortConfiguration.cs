// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xaml.PortAnalyzers.Configuration
{
    /// <summary>
    /// Port settings of a compilation, read from MSBuild properties (<c>build_property.*</c>) and
    /// .editorconfig keys (<c>xaml_port.*</c>, which take precedence).
    /// </summary>
    internal sealed class PortConfiguration
    {
        /// <summary>
        /// The preprocessor symbol used when none is configured.
        /// </summary>
        public const string DefaultTargetSymbol = "UNO";

        private readonly AnalyzerConfigOptionsProvider _provider;
        private readonly GlobPattern[] _excludes;
        private readonly string? _projectDirectory;
        private readonly string _globalTargetSymbol;
        private readonly string? _globalKnownSymbols;
        private readonly ConcurrentDictionary<string, SymbolPatternSet> _knownSymbolSets = new(StringComparer.Ordinal);

        private PortConfiguration(AnalyzerConfigOptionsProvider provider)
        {
            _provider = provider;
            var global = provider.GlobalOptions;
            _globalTargetSymbol = NormalizeSymbol(GetValue(global, PortOptionNames.TargetSymbolKey) ?? GetValue(global, PortOptionNames.TargetSymbolProperty)) ?? DefaultTargetSymbol;
            _globalKnownSymbols = Combine(GetValue(global, PortOptionNames.KnownSymbolsProperty), GetValue(global, PortOptionNames.KnownSymbolsKey));
            _excludes = GlobPattern.ParseList(GetValue(global, PortOptionNames.SharedSourceExcludesProperty));
            _projectDirectory = NormalizeDirectory(GetValue(global, PortOptionNames.ProjectDirectoryProperty) ?? GetValue(global, PortOptionNames.ProjectDirProperty));
        }

        /// <summary>
        /// Gets the globally configured port target symbol.
        /// </summary>
        public string GlobalTargetSymbol => _globalTargetSymbol;

        /// <summary>
        /// Gets the normalized project directory (forward slashes, trailing slash) or <see langword="null"/>.
        /// </summary>
        public string? ProjectDirectory => _projectDirectory;

        /// <summary>
        /// Creates the configuration of a compilation.
        /// </summary>
        public static PortConfiguration Create(AnalyzerConfigOptionsProvider provider) => new(provider);

        /// <summary>
        /// Resolves the settings of one syntax tree.
        /// </summary>
        public PortTreeSettings GetTreeSettings(SyntaxTree tree)
        {
            var options = _provider.GetOptions(tree);
            var target = NormalizeSymbol(GetValue(options, PortOptionNames.TargetSymbolKey)) ?? _globalTargetSymbol;
            var known = Combine(_globalKnownSymbols, GetValue(options, PortOptionNames.KnownSymbolsKey));
            bool isShared;
            var shared = GetValue(options, PortOptionNames.SharedKey);
            if (shared is not null && bool.TryParse(shared, out var sharedValue))
            {
                isShared = sharedValue;
            }
            else
            {
                isShared = !IsExcluded(tree.FilePath);
            }

            return new PortTreeSettings(target, GetKnownSymbols(target, known), isShared);
        }

        private SymbolPatternSet GetKnownSymbols(string target, string? known)
        {
            var key = known is null ? target : target + "\n" + known;
            if (!_knownSymbolSets.TryGetValue(key, out var set))
            {
                set = _knownSymbolSets.GetOrAdd(key, SymbolPatternSet.CreateKnown(target, known));
            }

            return set;
        }

        /// <summary>
        /// Returns whether the file is matched by one of the shared source exclude globs.
        /// </summary>
        public bool IsExcluded(string? filePath)
        {
            if (_excludes.Length == 0 || filePath is null || filePath.Length == 0)
            {
                return false;
            }

            var path = GlobPattern.NormalizePath(filePath);
            if (_projectDirectory is not null)
            {
                if (path.StartsWith(_projectDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    return MatchesAny(path.Substring(_projectDirectory.Length));
                }

                return MatchesAny(path);
            }

            // Without a project directory, try every suffix that starts at a path segment.
            if (MatchesAny(path))
            {
                return true;
            }

            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] == '/' && i + 1 < path.Length && MatchesAny(path.Substring(i + 1)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool MatchesAny(string relativePath)
        {
            for (var i = 0; i < _excludes.Length; i++)
            {
                if (_excludes[i].IsMatch(relativePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static string? GetValue(AnalyzerConfigOptions options, string key)
        {
            if (options.TryGetValue(key, out var value))
            {
                value = value.Trim();
                if (value.Length > 0)
                {
                    return value;
                }
            }

            return null;
        }

        private static string? NormalizeSymbol(string? value)
        {
            if (value is null)
            {
                return null;
            }

            var trimmed = value.Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }

        private static string? Combine(string? first, string? second)
        {
            if (first is null)
            {
                return second;
            }

            return second is null ? first : first + ";" + second;
        }

        private static string? NormalizeDirectory(string? value)
        {
            if (value is null)
            {
                return null;
            }

            var path = GlobPattern.NormalizePath(value.Trim());
            if (path.Length == 0)
            {
                return null;
            }

            return path[path.Length - 1] == '/' ? path : path + "/";
        }
    }
}
