// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xaml.PortAnalyzers.UnitTests.Infrastructure;

/// <summary>
/// In-memory analyzer config options: global options (MSBuild properties / .globalconfig) plus per-file
/// options (.editorconfig sections). Like the compiler, per-file options include the global ones.
/// </summary>
internal sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly TestAnalyzerConfigOptions _global;
    private readonly Dictionary<string, TestAnalyzerConfigOptions> _files = new();

    public TestAnalyzerConfigOptionsProvider(IReadOnlyDictionary<string, string> globalOptions, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> fileOptions)
    {
        _global = new TestAnalyzerConfigOptions(globalOptions, null);
        foreach (var pair in fileOptions)
        {
            _files[pair.Key] = new TestAnalyzerConfigOptions(pair.Value, globalOptions);
        }
    }

    public override AnalyzerConfigOptions GlobalOptions => _global;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        => _files.TryGetValue(tree.FilePath, out var options) ? options : _global;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _global;

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _values = new(KeyComparer);

        public TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string> values, IReadOnlyDictionary<string, string>? fallback)
        {
            if (fallback is not null)
            {
                foreach (var pair in fallback)
                {
                    _values[pair.Key] = pair.Value;
                }
            }

            foreach (var pair in values)
            {
                _values[pair.Key] = pair.Value;
            }
        }

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _values.TryGetValue(key, out value);
    }
}
