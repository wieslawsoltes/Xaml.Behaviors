// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI counterpart of the in-place item sorting used by <see cref="SelectingItemsControlSearchBehavior"/>.
/// </content>
public sealed partial class SelectingItemsControlSearchBehavior
{
    /// <summary>
    /// Sorts the tab items of a <c>TabView</c> in place (Avalonia sorts <c>ItemCollection</c> through
    /// <c>ArrayList.Adapter</c>, which WinUI's <c>IList&lt;object&gt;</c> collections do not support).
    /// </summary>
    private static void SortTabItems(IList<object> items, IComparer<object> comparer)
    {
        var sorted = new List<object>(items);
        sorted.Sort(comparer);

        for (var index = 0; index < sorted.Count; index++)
        {
            if (ReferenceEquals(items[index], sorted[index]))
            {
                continue;
            }

            // Move the item into place; the remaining items keep their relative order.
            items.Remove(sorted[index]);
            items.Insert(index, sorted[index]);
        }
    }
}
