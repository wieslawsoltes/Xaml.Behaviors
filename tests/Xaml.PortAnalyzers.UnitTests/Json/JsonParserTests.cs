// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xaml.PortAnalyzers.Json;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Json;

public class JsonParserTests
{
    [Fact]
    public void Parse_ReadsNestedDocument()
    {
        var value = JsonParser.Parse("""
            {
              "name": "value",
              "list": [1, -2.5e+3, true, false, null, "x"],
              "nested": { "empty": {}, "none": [] }
            }
            """);

        Assert.Equal(JsonValueKind.Object, value.Kind);
        Assert.Equal("value", value.GetProperty("name")!.Text);
        var list = value.GetProperty("list")!;
        Assert.Equal(JsonValueKind.Array, list.Kind);
        Assert.Equal(6, list.Items.Count);
        Assert.Equal("1", list.Items[0].Text);
        Assert.Equal("-2.5e+3", list.Items[1].Text);
        Assert.True(list.Items[2].Boolean);
        Assert.False(list.Items[3].Boolean);
        Assert.Equal(JsonValueKind.Null, list.Items[4].Kind);
        Assert.Equal(JsonValueKind.String, list.Items[5].Kind);
        var nested = value.GetProperty("nested")!;
        Assert.Empty(nested.GetProperty("empty")!.Properties);
        Assert.Empty(nested.GetProperty("none")!.Items);
        Assert.Null(value.GetProperty("missing"));
    }

    [Fact]
    public void Parse_DecodesEscapes()
    {
        var value = JsonParser.Parse("\"a\\\"b\\\\c\\/d\\n\\t\\r\\b\\f\\u0041\\u00e9\"");

        Assert.Equal("a\"b\\c/d\n\t\r\b\fAé", value.Text);
    }

    [Fact]
    public void Parse_AcceptsCommentsAndByteOrderMark()
    {
        var value = JsonParser.Parse("﻿// leading\n{ /* inline */ \"a\": [ \"b\" ] // trailing\n}");

        Assert.Equal("b", value.GetProperty("a")!.Items[0].Text);
    }

    [Fact]
    public void Parse_LastDuplicatePropertyWins()
    {
        var value = JsonParser.Parse("{\"a\": 1, \"a\": 2}");

        Assert.Equal("2", value.GetProperty("a")!.Text);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("{", 1)]
    [InlineData("{\"a\" 1}", 5)]
    [InlineData("{\"a\": 1,}", 8)]
    [InlineData("[1,]", 3)]
    [InlineData("[1 2]", 3)]
    [InlineData("\"abc", 0)]
    [InlineData("\"a\\x\"", 2)]
    [InlineData("\"a\\u12\"", 2)]
    [InlineData("\"a\nb\"", 2)]
    [InlineData("01", 0)]
    [InlineData("1.", 0)]
    [InlineData("-", 0)]
    [InlineData("1e", 0)]
    [InlineData("tru", 0)]
    [InlineData("nullx", 0)]
    [InlineData("{} x", 3)]
    [InlineData("/* open", 0)]
    [InlineData("@", 0)]
    public void Parse_ReportsErrorPosition(string text, int position)
    {
        var exception = Assert.Throws<JsonParseException>(() => JsonParser.Parse(text));

        Assert.Equal(position, exception.Position);
        Assert.False(string.IsNullOrEmpty(exception.Message));
    }

    [Fact]
    public void Parse_RejectsDeepNesting()
    {
        var text = new string('[', 100) + new string(']', 100);

        Assert.Throws<JsonParseException>(() => JsonParser.Parse(text));
    }

    [Fact]
    public void Parse_RecordsValuePositions()
    {
        var value = JsonParser.Parse("{ \"a\": [ \"b\" ] }");

        Assert.Equal(0, value.Position);
        Assert.Equal(7, value.GetProperty("a")!.Position);
        Assert.Equal(9, value.GetProperty("a")!.Items[0].Position);
    }
}
