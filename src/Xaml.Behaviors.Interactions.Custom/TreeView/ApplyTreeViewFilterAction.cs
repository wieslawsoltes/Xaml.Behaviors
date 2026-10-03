// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Filters a <see cref="TreeView"/> using the provided query string.
/// </summary>
public sealed partial class ApplyTreeViewFilterAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the tree view to filter.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial TreeView? TreeView { get; set; }

    /// <summary>
    /// Gets or sets the filter query string.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial string? Query { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var treeView = TreeView ?? sender as TreeView;
        if (treeView is null)
        {
            return false;
        }

        var query = Query ?? string.Empty;
        TreeViewFilterHelper.Filter(treeView, query);
        return true;
    }
}
