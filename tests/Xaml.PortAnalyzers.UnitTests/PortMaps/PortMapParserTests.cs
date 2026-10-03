// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Xaml.PortAnalyzers.Json;
using Xaml.PortAnalyzers.PortMaps;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.PortMaps;

public class PortMapParserTests
{
    [Fact]
    public void Parse_ReadsAllSections()
    {
        var map = PortMapParser.Parse("""
            {
              "$schema": "./xamlport.schema.json",
              "sourceNamespacePrefixes": ["Avalonia", "MyLib.Avalonia"],
              "mapped": {
                "types": ["Avalonia.AvaloniaObject"],
                "namespaces": ["Avalonia.Xaml.Interactivity"],
                "members": ["Avalonia.AvaloniaObject.GetValue", " Avalonia.AvaloniaObject.SetValue "]
              },
              "unsupported": { "types": ["Avalonia.Controls.Window"], "namespaces": ["Avalonia.LogicalTree"], "members": ["Avalonia.Visual.Bounds"] },
              "renamedNamespaces": { "Avalonia.Xaml.Interactions": "Xaml.Interactions" }
            }
            """);

        Assert.Equal(["Avalonia", "MyLib.Avalonia"], map.SourceNamespacePrefixes);
        Assert.Contains("Avalonia.AvaloniaObject", map.Mapped.Types);
        Assert.Contains("Avalonia.Xaml.Interactivity", map.Mapped.Namespaces);
        Assert.Contains("Avalonia.AvaloniaObject.SetValue", map.Mapped.Members);
        Assert.Contains("Avalonia.Controls.Window", map.Unsupported.Types);
        Assert.Contains("Avalonia.LogicalTree", map.Unsupported.Namespaces);
        Assert.Contains("Avalonia.Visual.Bounds", map.Unsupported.Members);
        Assert.Equal("Xaml.Interactions", map.RenamedNamespaces["Avalonia.Xaml.Interactions"]);
    }

    [Fact]
    public void Parse_DefaultsSourceNamespacePrefixToAvalonia()
    {
        var map = PortMapParser.Parse("{}");

        Assert.Equal([PortMap.DefaultSourceNamespacePrefix], map.SourceNamespacePrefixes);
        Assert.Empty(map.Mapped.Types);
        Assert.Empty(map.Unsupported.Namespaces);
        Assert.Empty(map.RenamedNamespaces);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("{ \"mapped\": [] }")]
    [InlineData("{ \"mapped\": { \"type\": [] } }")]
    [InlineData("{ \"mapped\": { \"types\": \"Avalonia.X\" } }")]
    [InlineData("{ \"mapped\": { \"types\": [1] } }")]
    [InlineData("{ \"mapped\": { \"types\": [\"\"] } }")]
    [InlineData("{ \"unsupported\": 1 }")]
    [InlineData("{ \"sourceNamespacePrefixes\": {} }")]
    [InlineData("{ \"renamedNamespaces\": [] }")]
    [InlineData("{ \"renamedNamespaces\": { \"A\": 1 } }")]
    [InlineData("{ \"maped\": {} }")]
    [InlineData("{ \"mapped\": { } ")]
    public void Parse_RejectsInvalidDocuments(string json)
    {
        Assert.Throws<JsonParseException>(() => PortMapParser.Parse(json));
    }

    [Fact]
    public void GetRenamedNamespace_MapsNestedNamespaces()
    {
        var map = PortMapParser.Parse("{ \"renamedNamespaces\": { \"Avalonia.Xaml.Interactivity\": \"Xaml.Interactivity\" } }");

        Assert.Equal("Xaml.Interactivity", map.GetRenamedNamespace("Avalonia.Xaml.Interactivity"));
        Assert.Equal("Xaml.Interactivity.Core", map.GetRenamedNamespace("Avalonia.Xaml.Interactivity.Core"));
        Assert.Null(map.GetRenamedNamespace("Avalonia.Xaml"));
        Assert.Null(map.GetRenamedNamespace("Avalonia.Xaml.InteractivityX"));
    }

    [Theory]
    [InlineData("Avalonia", "Avalonia", true)]
    [InlineData("Avalonia.Controls", "Avalonia", true)]
    [InlineData("AvaloniaEdit", "Avalonia", false)]
    [InlineData("System", "Avalonia", false)]
    [InlineData("Anything", "", true)]
    public void IsSameOrNested_RespectsNamespaceBoundaries(string name, string prefix, bool expected)
    {
        Assert.Equal(expected, PortMap.IsSameOrNested(name, prefix));
    }

    [Fact]
    public void ContainsNamespace_CoversNestedNamespaces()
    {
        var map = PortMapParser.Parse("{ \"mapped\": { \"namespaces\": [\"Avalonia.Input\"] } }");

        Assert.True(map.Mapped.ContainsNamespace("Avalonia.Input"));
        Assert.True(map.Mapped.ContainsNamespace("Avalonia.Input.Platform"));
        Assert.False(map.Mapped.ContainsNamespace("Avalonia.InputX"));
        Assert.False(map.Mapped.ContainsNamespace("Avalonia"));
    }

    [Fact]
    public void Merge_UnitesAllMaps()
    {
        var first = PortMapParser.Parse("{ \"sourceNamespacePrefixes\": [\"A\"], \"mapped\": { \"types\": [\"A.X\"] }, \"renamedNamespaces\": { \"A.N\": \"N\" } }");
        var second = PortMapParser.Parse("{ \"sourceNamespacePrefixes\": [\"A\", \"B\"], \"unsupported\": { \"namespaces\": [\"B.Y\"] } }");

        var merged = PortMap.Merge([first, second]);

        Assert.Equal(["A", "B"], merged.SourceNamespacePrefixes);
        Assert.Contains("A.X", merged.Mapped.Types);
        Assert.Contains("B.Y", merged.Unsupported.Namespaces);
        Assert.Equal("N", merged.RenamedNamespaces["A.N"]);
        Assert.Same(first, PortMap.Merge([first]));
    }

    [Fact]
    public void Loader_ReadsOnlyPortMapFilesAndCollectsErrors()
    {
        ImmutableArray<AdditionalText> files =
        [
            new TestAdditionalText("/p/a.xamlport.json", "{ \"mapped\": { \"types\": [\"Avalonia.A\"] } }"),
            new TestAdditionalText("/p/B.XamlPort.Json", "{ \"mapped\": { \"types\": [\"Avalonia.B\"] } }"),
            new TestAdditionalText("/p/other.json", "not json"),
            new TestAdditionalText("/p/bad.xamlport.json", "{\n  \"mapped\": 5\n}"),
        ];

        var result = PortMapLoader.Load(files, CancellationToken.None);

        Assert.NotNull(result.Map);
        Assert.Contains("Avalonia.A", result.Map!.Mapped.Types);
        Assert.Contains("Avalonia.B", result.Map.Mapped.Types);
        var error = Assert.Single(result.Errors);
        Assert.Equal("/p/bad.xamlport.json", error.File.Path);
        Assert.Equal(1, error.LineSpan.Start.Line);
        Assert.Equal("/p/bad.xamlport.json", error.GetLocation().GetLineSpan().Path);
    }

    [Fact]
    public void Loader_ReturnsNoMapWithoutPortMapFiles()
    {
        var result = PortMapLoader.Load([new TestAdditionalText("/p/x.json", "{}")], CancellationToken.None);

        Assert.Null(result.Map);
        Assert.Empty(result.Errors);
    }
}
