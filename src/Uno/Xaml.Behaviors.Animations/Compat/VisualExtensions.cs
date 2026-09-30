// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of <c>Avalonia.VisualTree.VisualExtensions.GetVisualDescendants</c> used by the shared sources.
/// </summary>
internal static class VisualExtensions
{
    /// <summary>
    /// Enumerates the visual descendants of an element in depth-first order.
    /// </summary>
    /// <param name="visual">The root element.</param>
    /// <returns>The descendant framework elements.</returns>
    public static IEnumerable<FrameworkElement> GetVisualDescendants(this DependencyObject visual)
    {
        int count = VisualTreeHelper.GetChildrenCount(visual);
        for (int i = 0; i < count; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(visual, i);
            if (child is FrameworkElement element)
            {
                yield return element;
            }

            foreach (FrameworkElement descendant in GetVisualDescendants(child))
            {
                yield return descendant;
            }
        }
    }
}
