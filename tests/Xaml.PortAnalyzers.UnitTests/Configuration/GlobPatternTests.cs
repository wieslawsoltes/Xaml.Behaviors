// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xaml.PortAnalyzers.Configuration;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Configuration;

public class GlobPatternTests
{
    [Theory]
    [InlineData("Foo.cs", "Foo.cs", true)]
    [InlineData("Foo.cs", "foo.CS", true)]
    [InlineData("Foo.cs", "Bar/Foo.cs", false)]
    [InlineData("*.cs", "Foo.cs", true)]
    [InlineData("*.cs", "Dir/Foo.cs", false)]
    [InlineData("Dir/*.cs", "Dir/Foo.cs", true)]
    [InlineData("Dir/*.cs", "Dir/Sub/Foo.cs", false)]
    [InlineData("Dir/**", "Dir/Foo.cs", true)]
    [InlineData("Dir/**", "Dir/Sub/Deep/Foo.cs", true)]
    [InlineData("Dir/**", "Other/Foo.cs", false)]
    [InlineData("Dir/**", "DirX/Foo.cs", false)]
    [InlineData("**/*.cs", "Foo.cs", true)]
    [InlineData("**/*.cs", "A/B/Foo.cs", true)]
    [InlineData("**/Handlers/*.cs", "Events/Handlers/Click.cs", true)]
    [InlineData("**/Handlers/*.cs", "Handlers/Click.cs", true)]
    [InlineData("**/Handlers/*.cs", "Events/Handlers/Sub/Click.cs", false)]
    [InlineData("A/**/B.cs", "A/B.cs", true)]
    [InlineData("A/**/B.cs", "A/x/y/B.cs", true)]
    [InlineData("F?o.cs", "Foo.cs", true)]
    [InlineData("F?o.cs", "F/o.cs", false)]
    [InlineData("F?o.cs", "Fo.cs", false)]
    [InlineData("*Handler.cs", "ButtonClickEventHandler.cs", true)]
    [InlineData("**Handler.cs", "Events/ButtonClickEventHandler.cs", true)]
    [InlineData("Templates/", "Templates/A.cs", true)]
    [InlineData("./Foo.cs", "Foo.cs", true)]
    [InlineData("Dir\\Foo.cs", "Dir/Foo.cs", true)]
    [InlineData("Templates%2F%2A%2A", "Templates/A.cs", true)]
    [InlineData("Templates/%2a.cs", "Templates/A.cs", true)]
    public void IsMatch_FollowsMsBuildGlobSemantics(string pattern, string path, bool expected)
    {
        var glob = GlobPattern.Create(pattern);

        Assert.NotNull(glob);
        Assert.Equal(expected, glob!.IsMatch(path));
    }

    [Fact]
    public void ParseList_IgnoresBlankEntriesAndWhitespace()
    {
        var globs = GlobPattern.ParseList(" A.cs ;\n  Templates/** ; ;\r\n");

        Assert.Equal(["A.cs", "Templates/**"], [globs[0].Pattern, globs[1].Pattern]);
        Assert.Equal(2, globs.Length);
    }

    [Fact]
    public void ParseList_AcceptsPipeSeparators()
    {
        var globs = GlobPattern.ParseList("Templates/**|A.cs;B.cs");

        Assert.Equal(["Templates/**", "A.cs", "B.cs"], [globs[0].Pattern, globs[1].Pattern, globs[2].Pattern]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_ReturnsNullForBlankPatterns(string? pattern)
    {
        Assert.Null(GlobPattern.Create(pattern));
    }

    [Fact]
    public void ParseList_ReturnsEmptyForNull()
    {
        Assert.Empty(GlobPattern.ParseList(null));
    }

    [Fact]
    public void ToString_ReturnsNormalizedPattern()
    {
        Assert.Equal("Dir/**", GlobPattern.Create(".\\Dir\\")!.ToString());
    }
}
