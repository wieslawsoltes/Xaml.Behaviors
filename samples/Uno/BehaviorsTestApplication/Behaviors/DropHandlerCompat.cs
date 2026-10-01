// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace BehaviorsTestApplication.Behaviors;

/// <summary>
/// WinUI counterparts of the Avalonia drag and drop and visual tree members used by the shared drop handlers of the
/// sample (<c>e.DragEffects</c>, <c>e.Source</c>, <c>GetVisualAt</c>, <c>FindLogicalAncestorOfType</c>, <c>Bounds</c>).
/// </summary>
internal static class DropHandlerCompat
{
    extension(DragEventArgs e)
    {
        /// <summary>
        /// Gets the effect requested by the user, like Avalonia's <c>DragEffects</c> while dragging: the modifier keys
        /// select copy (Ctrl), link (Alt) or move, limited to the operations allowed by the drag source.
        /// </summary>
        public DataPackageOperation DragEffects
        {
            get
            {
                var allowed = e.AllowedOperations;
                var requested = (e.Modifiers & DragDropModifiers.Control) != 0
                    ? DataPackageOperation.Copy
                    : (e.Modifiers & DragDropModifiers.Alt) != 0
                        ? DataPackageOperation.Link
                        : DataPackageOperation.Move;

                if ((allowed & requested) != 0)
                {
                    return requested;
                }

                // Fall back to the first operation the source allows.
                if ((allowed & DataPackageOperation.Move) != 0)
                {
                    return DataPackageOperation.Move;
                }

                if ((allowed & DataPackageOperation.Copy) != 0)
                {
                    return DataPackageOperation.Copy;
                }

                return allowed & DataPackageOperation.Link;
            }
        }

        /// <summary>
        /// Gets the element that raised the event (Avalonia <c>Source</c>, WinUI <c>OriginalSource</c>).
        /// </summary>
        public object? Source => e.OriginalSource;
    }

    extension(FrameworkElement element)
    {
        /// <summary>
        /// Gets the layout bounds of the element (only the size is used by the sample).
        /// </summary>
        public Windows.Foundation.Rect Bounds =>
            new(element.ActualOffset.X, element.ActualOffset.Y, element.ActualWidth, element.ActualHeight);
    }

    /// <summary>
    /// Returns the topmost element under a point (Avalonia <c>Visual.GetVisualAt</c>).
    /// </summary>
    /// <param name="root">The element to search.</param>
    /// <param name="point">The point relative to <paramref name="root"/>.</param>
    /// <returns>The topmost element, or <c>null</c>.</returns>
    public static UIElement? GetVisualAt(this UIElement root, Windows.Foundation.Point point)
    {
        if (root.XamlRoot is null)
        {
            return null;
        }

        var hostPoint = root.TransformToVisual(null).TransformPoint(point);
        foreach (var element in VisualTreeHelper.FindElementsInHostCoordinates(hostPoint, root))
        {
            return element;
        }

        return null;
    }

    /// <summary>
    /// Returns the first ancestor of a type (WinUI has no logical tree: the visual ancestors are searched).
    /// </summary>
    /// <typeparam name="T">The ancestor type.</typeparam>
    /// <param name="element">The element.</param>
    /// <returns>The ancestor, or <c>null</c>.</returns>
    public static T? FindLogicalAncestorOfType<T>(this DependencyObject element)
        where T : class
    {
        var current = VisualTreeHelper.GetParent(element);
        while (current is not null)
        {
            if (current is T result)
            {
                return result;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
