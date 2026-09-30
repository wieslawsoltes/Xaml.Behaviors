// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Xaml.Behaviors.Uno.TestCompat;

/// <summary>
/// Uno Platform counterparts of the Avalonia visual helpers used by the shared Core tests.
/// </summary>
internal static class CoreTestCompat
{
    /// <summary>
    /// Translates a point relative to <paramref name="element"/> to coordinates relative to
    /// <paramref name="relativeTo"/> (Avalonia <c>Visual.TranslatePoint</c>).
    /// </summary>
    /// <returns>The translated point, or <see langword="null"/> when the elements do not share a visual tree.</returns>
    public static Point? TranslatePoint(this UIElement element, Point point, UIElement relativeTo)
    {
        if (element.XamlRoot is null || !ReferenceEquals(element.XamlRoot, relativeTo.XamlRoot))
        {
            return null;
        }

        try
        {
            return element.TransformToVisual(relativeTo).TransformPoint(point);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
