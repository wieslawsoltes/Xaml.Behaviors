// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

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

    /// <summary>
    /// Finds the item container hosting an element and the items control that owns it.
    /// </summary>
    /// <remarks>
    /// On Avalonia item behaviors are attached to the containers through styles (the container's logical parent is the
    /// items control). WinUI styles cannot attach behaviors, so they are usually attached to the root of the item
    /// template: the element itself or its closest visual ancestor that is an item container is used.
    /// </remarks>
    /// <param name="element">The element (a container or an element of an item template).</param>
    /// <param name="owner">The owning items control.</param>
    /// <param name="container">The item container.</param>
    /// <returns><c>true</c> when a container was found.</returns>
    public static bool TryGetItemContainer(this DependencyObject? element, [NotNullWhen(true)] out ItemsControl? owner, [NotNullWhen(true)] out FrameworkElement? container)
    {
        for (var current = element; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (current is FrameworkElement candidate && ItemsControl.ItemsControlFromItemContainer(candidate) is { } itemsControl)
            {
                owner = itemsControl;
                container = candidate;
                return true;
            }

            if (current is ItemsControl)
            {
                break;
            }
        }

        owner = null;
        container = null;
        return false;
    }
}
