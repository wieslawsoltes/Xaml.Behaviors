// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Markup;

namespace Xaml.Interactivity;

/// <summary>
/// Parses string representations of XAML values (Uno Platform counterpart of the Avalonia parser table).
/// </summary>
/// <remarks>
/// Uses <see cref="XamlBindingHelper.ConvertValue(Type, object)"/>, the WinUI XAML string conversion
/// (colors, brushes, thickness, corner radius, grid length, points, sizes, rectangles, font families, ...).
/// </remarks>
internal static class ParseHelper
{
    public static object? InvokeParse(string s, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(s);
        ArgumentNullException.ThrowIfNull(targetType);

        try
        {
            var result = XamlBindingHelper.ConvertValue(targetType, s);
            return result is not null && targetType.IsInstanceOfType(result) ? result : null;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or InvalidCastException or NotSupportedException)
        {
            return null;
        }
    }
}
