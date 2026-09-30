// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterparts of the Avalonia layout members used by the shared sources.
/// </summary>
internal static class LayoutCompatExtensions
{
    extension(FrameworkElement element)
    {
        /// <summary>
        /// Gets the arranged bounds of the element relative to its parent (Avalonia <c>Visual.Bounds</c>).
        /// </summary>
        public Rect Bounds
        {
            get
            {
                System.Numerics.Vector3 offset = element.ActualOffset;
                return new Rect(offset.X, offset.Y, element.ActualWidth, element.ActualHeight);
            }
        }
    }

    extension(Rect rect)
    {
        /// <summary>
        /// Gets the size of the rectangle (Avalonia <c>Rect.Size</c>).
        /// </summary>
        public Size Size => new(rect.Width, rect.Height);
    }
}
