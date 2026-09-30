// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Filters the nodes of a WinUI <see cref="TreeView"/> (Uno Platform counterpart of the Avalonia helper, which walks
/// the logical <c>TreeViewItem</c> children).
/// </summary>
/// <remarks>
/// A node matches when the string representation of its content contains the query (Avalonia matches the item
/// header). Non matching nodes without matching descendants are collapsed; nodes with matching descendants are
/// expanded. Only realized item containers can be hidden, as on Avalonia.
/// </remarks>
internal static class TreeViewFilterHelper
{
    public static int Filter(TreeView treeView, string query)
    {
        query = query.ToLowerInvariant();
        var count = 0;

        foreach (var node in treeView.RootNodes)
        {
            if (FilterNode(treeView, node, query))
            {
                count++;
            }
        }

        return count;
    }

    private static bool FilterNode(TreeView treeView, TreeViewNode node, string query)
    {
        var match = GetHeader(node)?.ToLowerInvariant().Contains(query) ?? false;
        var visibleChildren = false;

        foreach (var child in node.Children)
        {
            if (FilterNode(treeView, child, query))
            {
                visibleChildren = true;
            }
        }

        var visible = string.IsNullOrEmpty(query) || match || visibleChildren;
        if (treeView.ContainerFromNode(node) is TreeViewItem item)
        {
            item.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        node.IsExpanded = visibleChildren && !string.IsNullOrEmpty(query);

        return visible;
    }

    private static string? GetHeader(TreeViewNode node)
        => node.Content is TreeViewItem { Content: { } content } ? content.ToString() : node.Content?.ToString();
}
