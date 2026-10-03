// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Xaml.Interactions.Draggable;

/// <summary>
/// Shows an element over another element (Uno Platform replacement of the Avalonia adorner layer).
/// </summary>
internal static class PlaceholderOverlay
{
    /// <summary>
    /// Shows <paramref name="content"/> over <paramref name="adornedElement"/>, sized like it and not hit testable.
    /// </summary>
    /// <param name="adornedElement">The adorned element.</param>
    /// <param name="content">The content.</param>
    /// <returns>The popup hosting the content.</returns>
    public static Popup Show(FrameworkElement adornedElement, FrameworkElement content)
    {
        var origin = adornedElement.TransformToVisual(null).TransformPoint(new Point(0, 0));

        content.Width = adornedElement.ActualWidth;
        content.Height = adornedElement.ActualHeight;
        content.IsHitTestVisible = false;

        var popup = new Popup
        {
            Child = content,
            XamlRoot = adornedElement.XamlRoot,
            HorizontalOffset = origin.X,
            VerticalOffset = origin.Y,
            IsHitTestVisible = false,
            IsLightDismissEnabled = false,
        };
        popup.IsOpen = true;
        return popup;
    }

    /// <summary>
    /// Hides a popup shown by <see cref="Show"/>.
    /// </summary>
    /// <param name="popup">The popup.</param>
    public static void Hide(Popup popup)
    {
        popup.IsOpen = false;
        popup.Child = null;
    }
}
