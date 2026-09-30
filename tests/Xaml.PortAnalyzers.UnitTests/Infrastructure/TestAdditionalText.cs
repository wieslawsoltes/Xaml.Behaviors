// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.PortAnalyzers.UnitTests.Infrastructure;

/// <summary>
/// An in-memory additional file.
/// </summary>
internal sealed class TestAdditionalText : AdditionalText
{
    private readonly SourceText _text;

    public TestAdditionalText(string path, string text)
    {
        Path = path;
        _text = SourceText.From(text);
    }

    public override string Path { get; }

    public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
}
