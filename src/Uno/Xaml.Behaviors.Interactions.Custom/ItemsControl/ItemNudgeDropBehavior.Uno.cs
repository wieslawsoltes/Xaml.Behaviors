// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI counterparts of the Avalonia container helpers used by <see cref="ItemNudgeDropBehavior"/>.
/// </content>
public partial class ItemNudgeDropBehavior
{
    private static IEnumerable<FrameworkElement> GetRealizedContainers(ItemsControl itemsControl)
    {
        for (var index = 0; index < itemsControl.Items.Count; index++)
        {
            if (itemsControl.ContainerFromIndex(index) is FrameworkElement container)
            {
                yield return container;
            }
        }
    }

    /// <summary>
    /// Gets the layout bounds of <paramref name="container"/> relative to <paramref name="relativeTo"/>.
    /// </summary>
    /// <remarks>
    /// Like Avalonia's <c>Bounds</c> the rectangle ignores render transforms (the nudge translation itself), but it
    /// includes the scroll offset of the items host.
    /// </remarks>
    private static Rect GetLayoutBounds(FrameworkElement container, UIElement relativeTo)
    {
        var offset = container.ActualOffset;
        var origin = new Point(offset.X, offset.Y);
        if (Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(container) is UIElement parent)
        {
            origin = parent.TransformToVisual(relativeTo).TransformPoint(origin);
        }

        return new Rect(origin.X, origin.Y, container.ActualWidth, container.ActualHeight);
    }
}
