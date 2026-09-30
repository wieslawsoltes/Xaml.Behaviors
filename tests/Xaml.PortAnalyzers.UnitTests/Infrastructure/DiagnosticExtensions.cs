// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Xaml.PortAnalyzers.UnitTests.Infrastructure;

/// <summary>
/// Helpers that turn diagnostics into compact strings for assertions.
/// </summary>
internal static class DiagnosticExtensions
{
    /// <summary>
    /// Formats a diagnostic as <c>ID:line:'span text'</c> (1-based line).
    /// </summary>
    public static string Describe(this Diagnostic diagnostic)
    {
        var line = diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1;
        var tree = diagnostic.Location.SourceTree;
        var text = tree is null ? string.Empty : tree.GetText().ToString(diagnostic.Location.SourceSpan);
        return diagnostic.Id + ":" + line + ":'" + text + "'";
    }

    public static string[] Describe(this IEnumerable<Diagnostic> diagnostics)
        => diagnostics.Select(Describe).ToArray();
}
