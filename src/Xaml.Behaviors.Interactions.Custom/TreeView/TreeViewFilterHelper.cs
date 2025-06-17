// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Xaml.Interactions.Custom;

internal static class TreeViewFilterHelper
{
    public static int Filter(TreeView treeView, string query)
    {
        query = query.ToLowerInvariant();
        var count = 0;

        foreach (var item in treeView.GetLogicalChildren().OfType<TreeViewItem>())
        {
            if (FilterItem(item, query))
            {
                count++;
            }
        }

        return count;
    }

    private static bool FilterItem(TreeViewItem item, string query)
    {
        var match = item.Header?.ToString()?.ToLowerInvariant().Contains(query) ?? false;
        var visibleChildren = false;

        foreach (var child in item.GetLogicalChildren().OfType<TreeViewItem>())
        {
            if (FilterItem(child, query))
            {
                visibleChildren = true;
            }
        }

        var visible = string.IsNullOrEmpty(query) || match || visibleChildren;
        item.IsVisible = visible;
        item.IsExpanded = visibleChildren && !string.IsNullOrEmpty(query);

        return visible;
    }
}
