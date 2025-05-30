using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Filters a <see cref="TreeView"/> using the provided query string.
/// </summary>
public sealed class ApplyTreeViewFilterAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TreeView"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TreeView?> TreeViewProperty =
        AvaloniaProperty.Register<ApplyTreeViewFilterAction, TreeView?>(nameof(TreeView));

    /// <summary>
    /// Identifies the <seealso cref="Query"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> QueryProperty =
        AvaloniaProperty.Register<ApplyTreeViewFilterAction, string?>(nameof(Query));

    /// <summary>
    /// Gets or sets the tree view to filter.
    /// </summary>
    [ResolveByName]
    public TreeView? TreeView
    {
        get => GetValue(TreeViewProperty);
        set => SetValue(TreeViewProperty, value);
    }

    /// <summary>
    /// Gets or sets the filter query string.
    /// </summary>
    [Content]
    public string? Query
    {
        get => GetValue(QueryProperty);
        set => SetValue(QueryProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var treeView = TreeView ?? AssociatedObject as TreeView;
        if (treeView is null)
        {
            return false;
        }

        var query = Query ?? string.Empty;
        TreeViewFilterHelper.Filter(treeView, query);
        return true;
    }
}
