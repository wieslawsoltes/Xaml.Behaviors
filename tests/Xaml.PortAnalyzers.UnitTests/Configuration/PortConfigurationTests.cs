// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xaml.PortAnalyzers.Configuration;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Configuration;

public class PortConfigurationTests
{
    private static PortConfiguration Create(Dictionary<string, string> global, Dictionary<string, IReadOnlyDictionary<string, string>>? files = null)
        => PortConfiguration.Create(new TestAnalyzerConfigOptionsProvider(global, files ?? new Dictionary<string, IReadOnlyDictionary<string, string>>()));

    [Fact]
    public void Defaults_UseUnoAndShareEverything()
    {
        var configuration = Create(new Dictionary<string, string>());
        var tree = CSharpSyntaxTree.ParseText("class C {}", path: "/p/A.cs");

        var settings = configuration.GetTreeSettings(tree);

        Assert.Equal(PortConfiguration.DefaultTargetSymbol, configuration.GlobalTargetSymbol);
        Assert.Equal("UNO", settings.TargetSymbol);
        Assert.True(settings.IsShared);
        Assert.Null(configuration.ProjectDirectory);
    }

    [Fact]
    public void Excludes_AreRelativeToTheProjectDirectory()
    {
        var configuration = Create(new Dictionary<string, string>
        {
            ["build_property.MSBuildProjectDirectory"] = "C:\\repo\\src\\Project",
            ["build_property.XamlPortSharedSourceExcludes"] = "Templates/**; Helpers/Avalonia*.cs",
        });

        Assert.Equal("C:/repo/src/Project/", configuration.ProjectDirectory);
        Assert.True(configuration.IsExcluded("C:\\repo\\src\\Project\\Templates\\A.cs"));
        Assert.True(configuration.IsExcluded("c:/repo/src/project/Helpers/AvaloniaHelper.cs"));
        Assert.False(configuration.IsExcluded("C:\\repo\\src\\Project\\Helpers\\UnoHelper.cs"));
        Assert.False(configuration.IsExcluded("C:\\repo\\src\\Other\\Templates\\A.cs"));
        Assert.False(configuration.IsExcluded(null));
    }

    [Fact]
    public void Excludes_FallBackToPathSuffixesWithoutProjectDirectory()
    {
        var configuration = Create(new Dictionary<string, string>
        {
            ["build_property.XamlPortSharedSourceExcludes"] = "Templates/**",
        });

        Assert.True(configuration.IsExcluded("/any/where/Templates/A.cs"));
        Assert.False(configuration.IsExcluded("/any/where/Other/A.cs"));
    }

    [Fact]
    public void ProjectDir_IsUsedWhenMsBuildProjectDirectoryIsMissing()
    {
        var configuration = Create(new Dictionary<string, string>
        {
            ["build_property.ProjectDir"] = "/repo/p/",
            ["build_property.XamlPortSharedSourceExcludes"] = "A.cs",
        });

        Assert.Equal("/repo/p/", configuration.ProjectDirectory);
        Assert.True(configuration.IsExcluded("/repo/p/A.cs"));
        Assert.False(configuration.IsExcluded("/repo/p/Sub/A.cs"));
    }

    [Fact]
    public void EditorConfig_OverridesMsBuildProperties()
    {
        var files = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["/p/Excluded.cs"] = new Dictionary<string, string> { ["xaml_port.shared"] = "true", ["xaml_port.target_symbol"] = "WINUI" },
            ["/p/Private.cs"] = new Dictionary<string, string> { ["xaml_port.shared"] = "false", ["xaml_port.known_symbols"] = "LOCAL_SYMBOL" },
        };
        var configuration = Create(
            new Dictionary<string, string>
            {
                ["build_property.MSBuildProjectDirectory"] = "/p",
                ["build_property.XamlPortTargetSymbol"] = " PORT ",
                ["build_property.XamlPortSharedSourceExcludes"] = "Excluded.cs",
            },
            files);

        var excluded = configuration.GetTreeSettings(CSharpSyntaxTree.ParseText("", path: "/p/Excluded.cs"));
        var privateFile = configuration.GetTreeSettings(CSharpSyntaxTree.ParseText("", path: "/p/Private.cs"));
        var other = configuration.GetTreeSettings(CSharpSyntaxTree.ParseText("", path: "/p/Other.cs"));

        Assert.True(excluded.IsShared);
        Assert.Equal("WINUI", excluded.TargetSymbol);
        Assert.False(privateFile.IsShared);
        Assert.True(privateFile.KnownSymbols.IsMatch("LOCAL_SYMBOL"));
        Assert.True(privateFile.KnownSymbols.IsMatch("PORT"));
        Assert.True(other.IsShared);
        Assert.Equal("PORT", other.TargetSymbol);
        Assert.False(other.KnownSymbols.IsMatch("LOCAL_SYMBOL"));
        Assert.Same(other.KnownSymbols, configuration.GetTreeSettings(CSharpSyntaxTree.ParseText("", path: "/p/Other2.cs")).KnownSymbols);
    }

    [Fact]
    public void TreeCache_ReusesSettingsAndMaps()
    {
        var cache = new PortTreeCache(Create(new Dictionary<string, string>()));
        var tree = CSharpSyntaxTree.ParseText("#if !UNO\nclass C {}\n#endif\n", path: "/p/A.cs");

        Assert.Same(cache.GetSettings(tree), cache.GetSettings(tree));
        Assert.Same(cache.GetActivityMap(tree, default), cache.GetActivityMap(tree, default));
        Assert.True(cache.IsShared(tree));
        var declaration = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
        Assert.False(cache.IsActiveForPort(tree, declaration.SpanStart, default));
        Assert.NotNull(cache.Configuration);
    }
}
