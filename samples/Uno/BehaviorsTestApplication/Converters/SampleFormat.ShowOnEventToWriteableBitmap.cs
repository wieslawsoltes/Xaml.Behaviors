// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Globalization;
using Microsoft.UI.Xaml;

namespace BehaviorsTestApplication.Converters;

// Formatters of the ShowOnEventBehaviors … WriteableBitmap sample pages (the StringFormat of the Avalonia bindings).
public static partial class SampleFormat
{
    /// <summary>
    /// Formats a count like the Avalonia binding <c>StringFormat={}Size changed count: {0}</c>.
    /// </summary>
    /// <param name="value">The count.</param>
    /// <returns>The formatted text.</returns>
    public static string SizeChangedCount(int value) => string.Format(CultureInfo.CurrentCulture, "Size changed count: {0}", value);

    /// <summary>
    /// Formats a count like the Avalonia binding <c>StringFormat={}Count: {0}</c>.
    /// </summary>
    /// <param name="value">The count.</param>
    /// <returns>The formatted text.</returns>
    public static string Count(int value) => string.Format(CultureInfo.CurrentCulture, "Count: {0}", value);

    /// <summary>
    /// Formats a path like the Avalonia binding <c>StringFormat='Path: {0}'</c>.
    /// </summary>
    /// <param name="value">The path.</param>
    /// <returns>The formatted text.</returns>
    public static string Path(string? value) => string.Format(CultureInfo.CurrentCulture, "Path: {0}", value);

    /// <summary>
    /// Formats a flag like the Avalonia binding <c>StringFormat={}In viewport: {0}</c>.
    /// </summary>
    /// <param name="value">The flag.</param>
    /// <returns>The formatted text.</returns>
    public static string InViewport(bool value) => string.Format(CultureInfo.CurrentCulture, "In viewport: {0}", value);

    /// <summary>
    /// Formats a flag like the Avalonia binding <c>StringFormat={}Fully in viewport: {0}</c>.
    /// </summary>
    /// <param name="value">The flag.</param>
    /// <returns>The formatted text.</returns>
    public static string FullyInViewport(bool value) => string.Format(CultureInfo.CurrentCulture, "Fully in viewport: {0}", value);

    /// <summary>
    /// Gets the actual theme of an element (Avalonia: <c>{Binding #Scope.ActualThemeVariant}</c>).
    /// </summary>
    /// <remarks>
    /// <see cref="FrameworkElement.ActualTheme"/> is not a dependency property, so the binding passes the requested
    /// theme (a dependency property) as well to be evaluated again when it changes.
    /// </remarks>
    /// <param name="element">The element.</param>
    /// <param name="requestedTheme">The requested theme of the element.</param>
    /// <returns>The name of the actual theme.</returns>
    public static string ActualTheme(FrameworkElement? element, ElementTheme requestedTheme)
        => (element?.ActualTheme ?? requestedTheme).ToString();

    /// <summary>
    /// Converts the name of a theme variant (the items of the theme variant combo boxes) to an <see cref="ElementTheme"/>
    /// (Avalonia: <c>&lt;ThemeVariant&gt;Dark&lt;/ThemeVariant&gt;</c> items).
    /// </summary>
    /// <param name="name">The name (<c>Default</c>, <c>Dark</c> or <c>Light</c>).</param>
    /// <returns>The theme, or <see langword="null"/> when the name is not a theme.</returns>
    public static ElementTheme? ThemeVariant(object? name)
        => Enum.TryParse(name as string, out ElementTheme theme) ? theme : null;
}
