// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Text.RegularExpressions;

namespace Xaml.Behaviors.SourceGenerators.UnitTests;

/// <summary>
/// Translates the Avalonia-targeted input of the shared generator scenarios (<c>GeneratorTestHelper.RunGenerator</c>)
/// to WinUI, so the Uno twin runs the same scenarios through the WinUI emitter. It applies the mappings of the source
/// port (build/UnoPort/uno_share.py, src/Uno/UnoPortAliases.props) to the few Avalonia APIs the scenarios use.
/// Scenarios using Avalonia APIs without a mapping fail loudly: give them a WinUI source with <c>#if UNO</c> instead.
/// </summary>
internal static partial class WinUISourceTranslator
{
    private static readonly (string Avalonia, string WinUI)[] s_replacements =
    [
        // using directives (uno_share.py USING_MAP).
        ("using Avalonia.Xaml.Interactivity;", "using Xaml.Interactivity;"),
        ("using Avalonia.Interactivity;", "using Microsoft.UI.Xaml;"),
        ("using Avalonia.Controls;", "using Microsoft.UI.Xaml;\nusing Microsoft.UI.Xaml.Controls;"),
        ("using Avalonia.Input;", "using Microsoft.UI.Xaml.Input;"),
        ("using Avalonia;", "using Microsoft.UI.Xaml;"),
        // Qualified names.
        ("Avalonia.Xaml.Interactivity.", "Xaml.Interactivity."),
        ("Avalonia.Interactivity.RoutedEventArgs", "Microsoft.UI.Xaml.RoutedEventArgs"),
        ("Avalonia.AvaloniaObject", "Microsoft.UI.Xaml.DependencyObject"),
    ];

    /// <summary>Translates <paramref name="source"/> to WinUI.</summary>
    /// <param name="source">The Avalonia-targeted scenario source.</param>
    /// <returns>The WinUI source.</returns>
    /// <exception cref="InvalidOperationException">The source uses an Avalonia API without a WinUI mapping.</exception>
    public static string Translate(string source)
    {
        foreach (var (avalonia, winUI) in s_replacements)
        {
            source = source.Replace(avalonia, winUI, StringComparison.Ordinal);
        }

        // Avalonia input event arguments (UnoPortAliases.props).
        source = PointerEventArgsRegex().Replace(source, "PointerRoutedEventArgs");

        // Styled properties become dependency properties with typed CLR accessors.
        source = StyledPropertyRegex().Replace(
            source,
            static m => $"DependencyProperty {m.Groups["field"].Value} =\n            DependencyProperty.Register({m.Groups["name"].Value}, typeof({m.Groups["type"].Value}), typeof({m.Groups["owner"].Value}), null)");
        source = GetterRegex().Replace(source, static m => $"{m.Groups["head"].Value}({m.Groups["type"].Value})GetValue(");

        if (AvaloniaApiRegex().IsMatch(source))
        {
            throw new InvalidOperationException(
                "The scenario uses Avalonia APIs without a WinUI mapping; add a mapping to WinUISourceTranslator or a WinUI scenario (#if UNO):" +
                Environment.NewLine + source);
        }

        return source;
    }

    [GeneratedRegex(@"(?<!\w)(?:Avalonia(?:\.|Property\b|Object\b)|StyledProperty<|DirectProperty<)")]
    private static partial Regex AvaloniaApiRegex();

    [GeneratedRegex(@"\bPointerEventArgs\b")]
    private static partial Regex PointerEventArgsRegex();

    // StyledProperty<T> NameProperty = AvaloniaProperty.Register<Owner, T>(nameof(Name))
    [GeneratedRegex(@"StyledProperty<(?<type>[^>]+)>\s+(?<field>\w+)\s*=\s*AvaloniaProperty\.Register<(?<owner>[^,]+),\s*[^>]+>\(\s*(?<name>nameof\(\w+\))\s*\)")]
    private static partial Regex StyledPropertyRegex();

    // public T Name { get => GetValue(NameProperty); ... }  (WinUI GetValue returns object)
    [GeneratedRegex(@"(?<head>\b(?<type>[\w.?<>]+)\s+\w+\s*\{\s*get\s*=>\s*)GetValue\(")]
    private static partial Regex GetterRegex();
}
