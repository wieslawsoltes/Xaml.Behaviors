// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>Carousel</c> navigation methods for the WinUI <see cref="FlipView"/> (the Uno Platform carousel).
/// </summary>
internal static class FlipViewExtensions
{
    /// <summary>
    /// Moves to the next item (Avalonia <c>Carousel.Next()</c>; <see cref="FlipView"/> does not wrap).
    /// </summary>
    /// <param name="flipView">The flip view.</param>
    public static void Next(this FlipView flipView)
    {
        if (flipView.SelectedIndex < flipView.Items.Count - 1)
        {
            flipView.SelectedIndex++;
        }
    }

    /// <summary>
    /// Moves to the previous item (Avalonia <c>Carousel.Previous()</c>; <see cref="FlipView"/> does not wrap).
    /// </summary>
    /// <param name="flipView">The flip view.</param>
    public static void Previous(this FlipView flipView)
    {
        if (flipView.SelectedIndex > 0)
        {
            flipView.SelectedIndex--;
        }
    }
}
