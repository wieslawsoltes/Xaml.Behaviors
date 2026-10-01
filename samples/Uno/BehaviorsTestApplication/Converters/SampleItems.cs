// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using BehaviorsTestApplication.ViewModels;
using Microsoft.UI.Xaml;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Objects created in XAML by the Avalonia sample (<c>x:Arguments</c>, <c>x:TypeArguments</c>, enum items), for
/// compiled bindings of the Uno Platform views: WinUI XAML only creates objects with a parameterless constructor.
/// </summary>
public static class SampleItems
{
    /// <summary>
    /// Gets the items added by the <c>AddRangeAction</c> sample (Avalonia: a <c>generic:List</c> of two
    /// <see cref="ItemViewModel"/> created with <c>x:Arguments</c>).
    /// </summary>
    public static IReadOnlyList<ItemViewModel> AddedRange { get; } =
    [
        new ItemViewModel("Added 1", "Black"),
        new ItemViewModel("Added 2", "Gray"),
    ];

    /// <summary>
    /// Gets the theme variants of the theme samples (Avalonia: <c>ThemeVariant</c> items of a <c>ComboBox</c>).
    /// </summary>
    public static IReadOnlyList<ElementTheme> ThemeVariants { get; } =
    [
        ElementTheme.Default,
        ElementTheme.Dark,
        ElementTheme.Light,
    ];

    /// <summary>
    /// Creates an item (Avalonia: <c>&lt;vm:ItemViewModel&gt;&lt;x:Arguments&gt;…</c>).
    /// </summary>
    /// <param name="value">The item value.</param>
    /// <param name="color">The item color.</param>
    /// <returns>The item.</returns>
    public static ItemViewModel Create(string value, string color) => new(value, color);
}
