// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>TabControl</c> members for the WinUI <see cref="TabView"/> (the Uno Platform tab control).
/// </summary>
internal static class TabViewExtensions
{
    extension(TabView tabView)
    {
        /// <summary>
        /// Gets the number of tabs (Avalonia <c>ItemsControl.ItemCount</c>): the items of
        /// <see cref="TabView.TabItemsSource"/> when set, otherwise <see cref="TabView.TabItems"/>.
        /// </summary>
        public int ItemCount
        {
            get
            {
                switch (tabView.TabItemsSource)
                {
                    case null:
                        return tabView.TabItems.Count;
                    case ICollection collection:
                        return collection.Count;
                    case IEnumerable enumerable:
                    {
                        var count = 0;
                        foreach (var _ in enumerable)
                        {
                            count++;
                        }

                        return count;
                    }
                    default:
                        return tabView.TabItems.Count;
                }
            }
        }
    }
}
