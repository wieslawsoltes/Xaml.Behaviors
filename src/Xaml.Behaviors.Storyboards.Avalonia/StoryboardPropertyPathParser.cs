// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Xaml.Behaviors.Storyboards;

internal static class StoryboardPropertyPathParser
{
    public static IReadOnlyList<StoryboardPropertyPathSegment> Parse(string path)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        path = path.Trim();
        if (path.Length == 0)
        {
            throw new InvalidOperationException("Storyboard target property paths cannot be empty.");
        }

        var tokens = Split(path);
        if (tokens.Count == 0)
        {
            throw new InvalidOperationException($"Storyboard target property path '{path}' did not produce any segments.");
        }

        var segments = new List<StoryboardPropertyPathSegment>(tokens.Count);
        foreach (var token in tokens)
        {
            segments.Add(ParseSegment(token, path));
        }

        return segments;
    }

    private static List<string> Split(string path)
    {
        var tokens = new List<string>();
        var builder = new StringBuilder();
        var parenthesesDepth = 0;
        var bracketDepth = 0;

        for (var i = 0; i < path.Length; i++)
        {
            var ch = path[i];

            switch (ch)
            {
                case '.':
                    if (parenthesesDepth == 0 && bracketDepth == 0)
                    {
                        CommitToken(builder, tokens);
                        continue;
                    }
                    break;
                case '(':
                    parenthesesDepth++;
                    break;
                case ')':
                    parenthesesDepth = Math.Max(parenthesesDepth - 1, -1);
                    break;
                case '[':
                    bracketDepth++;
                    break;
                case ']':
                    bracketDepth = Math.Max(bracketDepth - 1, -1);
                    break;
            }

            if (parenthesesDepth < 0 || bracketDepth < 0)
            {
                throw new InvalidOperationException($"Storyboard target property path '{path}' contains mismatched brackets or parentheses.");
            }

            builder.Append(ch);
        }

        if (parenthesesDepth != 0 || bracketDepth != 0)
        {
            throw new InvalidOperationException($"Storyboard target property path '{path}' contains mismatched brackets or parentheses.");
        }

        CommitToken(builder, tokens);
        return tokens;
    }

    private static void CommitToken(StringBuilder builder, ICollection<string> tokens)
    {
        if (builder.Length == 0)
        {
            return;
        }

        tokens.Add(builder.ToString());
        builder.Clear();
    }

    private static StoryboardPropertyPathSegment ParseSegment(string rawToken, string fullPath)
    {
        var token = rawToken.Trim();
        if (token.Length == 0)
        {
            throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains an empty segment.");
        }

        var indexers = ParseIndexers(ref token, fullPath);

        string? owner = null;
        var isAttached = token.StartsWith("(", StringComparison.Ordinal) &&
                         token.EndsWith(")", StringComparison.Ordinal);
        if (isAttached)
        {
            token = token.Substring(1, token.Length - 2).Trim();
            if (token.Length == 0)
            {
                throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains an invalid attached property declaration.");
            }

            var lastDotIndex = token.LastIndexOf('.');
            if (lastDotIndex <= 0 || lastDotIndex == token.Length - 1)
            {
                throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains an invalid attached property segment '{rawToken}'.");
            }

            owner = token.Substring(0, lastDotIndex);
            token = token.Substring(lastDotIndex + 1);
        }

        token = token.Trim();
        if (token.Length == 0)
        {
            throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains an invalid segment '{rawToken}'.");
        }

        var propertyName = token;
        var indexerArray = indexers.Count == 0 ? Array.Empty<string>() : indexers.ToArray();

        return new StoryboardPropertyPathSegment(propertyName, owner, indexerArray);
    }

    private static List<string> ParseIndexers(ref string token, string fullPath)
    {
        var indexers = new List<string>();

        while (token.Length > 0 && token[token.Length - 1] == ']')
        {
            var depth = 0;
            var indexStart = -1;

            for (var i = token.Length - 1; i >= 0; i--)
            {
                var ch = token[i];
                if (ch == ']')
                {
                    depth++;
                    continue;
                }

                if (ch == '[')
                {
                    depth--;
                    if (depth == 0)
                    {
                        indexStart = i;
                        break;
                    }
                }
            }

            if (indexStart < 0)
            {
                throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains mismatched indexer brackets.");
            }

            var argumentLength = token.Length - indexStart - 2;
            var argument = token.Substring(indexStart + 1, argumentLength).Trim();
            if (argument.Length == 0)
            {
                throw new InvalidOperationException($"Storyboard target property path '{fullPath}' contains an empty indexer argument.");
            }

            indexers.Insert(0, argument);
            token = token.Substring(0, indexStart).TrimEnd();
        }

        return indexers;
    }
}
