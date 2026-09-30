// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Xaml.PortAnalyzers.UnitTests.Infrastructure;

/// <summary>
/// Metadata references of the in-memory test compilations: the runtime assemblies and Avalonia.
/// </summary>
internal static class TestReferences
{
    private static readonly Lazy<ImmutableArray<MetadataReference>> s_all = new(Create);

    public static ImmutableArray<MetadataReference> All => s_all.Value;

    private static ImmutableArray<MetadataReference> Create()
    {
        var paths = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty;
        foreach (var path in trusted.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var name = Path.GetFileName(path);
            if (name.StartsWith("System.", StringComparison.Ordinal)
                || name is "System.dll" or "mscorlib.dll" or "netstandard.dll"
                || name.StartsWith("Avalonia", StringComparison.Ordinal))
            {
                paths.Add(path);
            }
        }

        // Make sure the Avalonia assemblies providing the symbols used by the tests are present.
        paths.Add(typeof(Avalonia.AvaloniaObject).Assembly.Location);
        paths.Add(typeof(Avalonia.Controls.Control).Assembly.Location);

        var builder = ImmutableArray.CreateBuilder<MetadataReference>(paths.Count);
        foreach (var path in paths)
        {
            builder.Add(MetadataReference.CreateFromFile(path));
        }

        return builder.ToImmutable();
    }
}
