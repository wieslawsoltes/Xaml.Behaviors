// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>ScrollViewer</c> horizontal line and page scrolling methods for the WinUI <see cref="ScrollViewer"/>.
/// </summary>
internal static class ScrollViewerExtensions
{
    /// <summary>
    /// The Avalonia default small change (one line) in device independent pixels.
    /// </summary>
    private const double SmallChange = 16;

    /// <summary>Scrolls left by one line.</summary>
    /// <param name="scrollViewer">The scroll viewer.</param>
    public static void LineLeft(this ScrollViewer scrollViewer) => ScrollHorizontallyBy(scrollViewer, -SmallChange);

    /// <summary>Scrolls right by one line.</summary>
    /// <param name="scrollViewer">The scroll viewer.</param>
    public static void LineRight(this ScrollViewer scrollViewer) => ScrollHorizontallyBy(scrollViewer, SmallChange);

    /// <summary>Scrolls left by one viewport width.</summary>
    /// <param name="scrollViewer">The scroll viewer.</param>
    public static void PageLeft(this ScrollViewer scrollViewer) => ScrollHorizontallyBy(scrollViewer, -scrollViewer.ViewportWidth);

    /// <summary>Scrolls right by one viewport width.</summary>
    /// <param name="scrollViewer">The scroll viewer.</param>
    public static void PageRight(this ScrollViewer scrollViewer) => ScrollHorizontallyBy(scrollViewer, scrollViewer.ViewportWidth);

    private static void ScrollHorizontallyBy(ScrollViewer scrollViewer, double delta)
    {
        var offset = Math.Clamp(scrollViewer.HorizontalOffset + delta, 0, Math.Max(0, scrollViewer.ScrollableWidth));
        scrollViewer.ChangeView(offset, null, null, disableAnimation: true);
    }
}
