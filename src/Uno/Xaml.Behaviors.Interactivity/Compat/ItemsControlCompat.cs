// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>ItemsControl</c> members used by the shared sources.
/// </summary>
internal static class ItemsControlCompatExtensions
{
    /// <summary>
    /// Gets the realized item containers (Avalonia <c>ItemsControl.GetRealizedContainers</c>).
    /// </summary>
    /// <param name="itemsControl">The items control.</param>
    /// <returns>The containers of the realized items, in item order.</returns>
    public static IEnumerable<FrameworkElement> GetRealizedContainers(this ItemsControl itemsControl)
    {
        var count = itemsControl.Items.Count;
        for (var i = 0; i < count; i++)
        {
            if (itemsControl.ContainerFromIndex(i) is FrameworkElement container)
            {
                yield return container;
            }
        }
    }

    /// <summary>
    /// Gets the items control that owns an item container (Avalonia: the logical parent of the container).
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The owner, or <c>null</c>.</returns>
    public static ItemsControl? GetItemsControlOwner(this DependencyObject container)
        => ItemsControl.ItemsControlFromItemContainer(container);
}
