// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of Avalonia <c>Visual</c>/<c>InputElement</c> members used by the shared sources.
/// </summary>
internal static class UIElementCompatExtensions
{
    extension(UIElement element)
    {
        /// <summary>
        /// Gets or sets a value indicating whether the element is visible (Avalonia <c>Visual.IsVisible</c>).
        /// </summary>
        /// <remarks>Maps to <see cref="UIElement.Visibility"/>: <c>false</c> collapses the element.</remarks>
        public bool IsVisible
        {
            get => element.Visibility == Visibility.Visible;
            set => element.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Focuses the element (Avalonia <c>InputElement.Focus()</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> when the element received focus.</returns>
    public static bool Focus(this UIElement element)
        => element.Focus(FocusState.Programmatic);

    /// <summary>
    /// Gets the pointer position relative to an element (Avalonia <c>PointerEventArgs.GetPosition</c>).
    /// </summary>
    /// <param name="e">The pointer event arguments.</param>
    /// <param name="relativeTo">The element the position is relative to, or <c>null</c> for the root.</param>
    /// <returns>The position.</returns>
    public static Point GetPosition(this PointerRoutedEventArgs e, UIElement? relativeTo)
        => e.GetCurrentPoint(relativeTo).Position;

    /// <summary>
    /// Deconstructs a point into its coordinates (Avalonia <c>Point.Deconstruct</c>).
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public static void Deconstruct(this Point point, out double x, out double y)
    {
        x = point.X;
        y = point.Y;
    }
}
