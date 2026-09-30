// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xaml.PortAnalyzers.Configuration;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Configuration;

public class SymbolPatternSetTests
{
    [Theory]
    [InlineData("UNO")]
    [InlineData("DEBUG")]
    [InlineData("NET8_0")]
    [InlineData("NET10_0_OR_GREATER")]
    [InlineData("NETSTANDARD2_0")]
    [InlineData("NETCOREAPP3_1_OR_GREATER")]
    [InlineData("HAS_UNO_WINUI")]
    [InlineData("__UNO_SKIA__")]
    [InlineData("__ANDROID__")]
    [InlineData("BROWSER")]
    [InlineData("DOCFX")]
    [InlineData("MY_TARGET")]
    [InlineData("CUSTOM")]
    [InlineData("FEATURE_A")]
    public void CreateKnown_IncludesDefaultsTargetAndConfiguredSymbols(string symbol)
    {
        var set = SymbolPatternSet.CreateKnown("MY_TARGET", "CUSTOM|FEATURE_*");

        Assert.True(set.IsMatch(symbol));
    }

    [Theory]
    [InlineData("UNOO")]
    [InlineData("UNO_")]
    [InlineData("uno")]
    [InlineData("Debug")]
    [InlineData("FEATURE")]
    [InlineData("HAS_UN")]
    public void CreateKnown_RejectsUnknownSymbols(string symbol)
    {
        var set = SymbolPatternSet.CreateKnown("UNO", "FEATURE_*");

        Assert.False(set.IsMatch(symbol));
    }

    [Fact]
    public void CreateKnown_SupportsQuestionMarkWildcard()
    {
        var set = SymbolPatternSet.CreateKnown("UNO", "V?");

        Assert.True(set.IsMatch("V1"));
        Assert.False(set.IsMatch("V12"));
    }

    [Fact]
    public void CreateKnown_AcceptsNullAdditionalSymbols()
    {
        var set = SymbolPatternSet.CreateKnown("UNO", null);

        Assert.True(set.IsMatch("UNO"));
        Assert.False(set.IsMatch("OTHER"));
    }
}
