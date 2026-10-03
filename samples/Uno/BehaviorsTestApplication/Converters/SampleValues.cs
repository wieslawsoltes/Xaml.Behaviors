// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Values for compiled bindings (<c>{x:Bind converters:SampleValues.X}</c>) where the Uno XAML generator cannot
/// convert the literal of the Avalonia view.
/// </summary>
public static class SampleValues
{
    /// <summary>
    /// Gets the minimum date of DatePickerValidationBehaviorView (Avalonia literal <c>Minimum="2020-01-01"</c>).
    /// </summary>
    public static DateTimeOffset? MinimumValidatedDate { get; } = new DateTimeOffset(new DateTime(2020, 1, 1));

    /// <summary>
    /// Gets the maximum date of DatePickerValidationBehaviorView (Avalonia literal <c>Maximum="2030-12-31"</c>).
    /// </summary>
    public static DateTimeOffset? MaximumValidatedDate { get; } = new DateTimeOffset(new DateTime(2030, 12, 31));

    /// <summary>
    /// Gets the time the value is read, as an object (LogActionView, Avalonia <c>Argument="{x:Static sys:DateTime.Now}"</c>).
    /// </summary>
    /// <remarks>
    /// A compiled binding to <c>System.DateTime</c> itself crashes native WinUI (its XAML type information has no base
    /// type); the object typed property avoids it on both platforms.
    /// </remarks>
    public static object Now => DateTime.Now;

    /// <summary>
    /// Converts a color name of a model (for example <c>Tile.Background</c>) to a brush, like the Avalonia binding of a
    /// string to a brush property (compiled bindings do not convert strings).
    /// </summary>
    /// <param name="color">The color name or <c>#ARGB</c> value.</param>
    /// <returns>The brush, or <c>null</c>.</returns>
    public static Brush? ToBrush(string? color)
        => string.IsNullOrEmpty(color) ? null : XamlBindingHelper.ConvertValue(typeof(Brush), color) as Brush;
}
