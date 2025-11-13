// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides base drag-and-drop visuals and validation helpers for <see cref="TreeView"/> scenarios.
/// </summary>
public abstract class BaseTreeViewDropHandler : DropHandlerBase
{
    private const string RowDraggingUpStyleClass = "DraggingUp";
    private const string RowDraggingDownStyleClass = "DraggingDown";
    private const string TargetHighlightStyleClass = "TargetHighlight";

    /// <summary>
    /// Validates a pending tree-view drag operation and optionally executes it.
    /// </summary>
    /// <param name="treeView">The owning tree view.</param>
    /// <param name="e">The drag event data.</param>
    /// <param name="sourceContext">The source context.</param>
    /// <param name="targetContext">The potential target context.</param>
    /// <param name="execute">When true, perform the drop logic.</param>
    protected abstract (bool Valid, bool WillSourceItemBeMovedToDifferentParent) Validate(TreeView treeView, DragEventArgs e, object? sourceContext, object? targetContext, bool execute);

    /// <inheritdoc />
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is TreeView treeView)
        {
            var (valid, willSourceItemChangeParent) = Validate(treeView, e, sourceContext, targetContext, false);
            var targetVisual = treeView.GetVisualAt(e.GetPosition(treeView));
            if (valid)
            {
                var targetItem = FindTreeViewItemFromChildView(targetVisual);
                if (targetItem is not null)
                {
                    var isDirectionUp = e.GetPosition(targetItem).Y < targetItem.Bounds.Height / 2;
                    var itemToApplyStyle = willSourceItemChangeParent && targetItem?.Parent is TreeViewItem parentItem
                        ? parentItem
                        : targetItem;
                    ApplyDraggingStyleToItem(itemToApplyStyle, isDirectionUp, willSourceItemChangeParent);
                    ClearDraggingStyleFromAllItems(sender, itemToApplyStyle);
                }
            }

            return valid;
        }

        ClearDraggingStyleFromAllItems(sender);
        return false;
    }

    /// <inheritdoc />
    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        ClearDraggingStyleFromAllItems(sender);
        if (e.Source is Control && sender is TreeView treeView)
        {
            var (valid, _) = Validate(treeView, e, sourceContext, targetContext, true);
            return valid;
        }

        return false;
    }

    /// <inheritdoc />
    public override void Cancel(object? sender, RoutedEventArgs e)
    {
        base.Cancel(sender, e);
        ClearDraggingStyleFromAllItems(sender);
    }

    /// <summary>
    /// Finds the <see cref="TreeViewItem"/> associated with the provided visual descendant.
    /// </summary>
    /// <param name="sourceChild">The visual to inspect.</param>
    private static TreeViewItem? FindTreeViewItemFromChildView(StyledElement? sourceChild)
    {
        if (sourceChild is null)
        {
            return null;
        }

        return sourceChild.FindLogicalAncestorOfType<TreeViewItem>();
    }

    private static void ClearDraggingStyleFromAllItems(object? sender, TreeViewItem? exceptThis = null)
    {
        if (sender is not Visual rootVisual)
        {
            return;
        }

        foreach (var item in rootVisual.GetLogicalChildren().OfType<TreeViewItem>())
        {
            if (item == exceptThis)
            {
                continue;
            }

            if (item.Classes is not null)
            {
                item.Classes.Remove(RowDraggingUpStyleClass);
                item.Classes.Remove(RowDraggingDownStyleClass);
                item.Classes.Remove(TargetHighlightStyleClass);
            }

            ClearDraggingStyleFromAllItems(item, exceptThis);
        }
    }

    private static void ApplyDraggingStyleToItem(TreeViewItem? item, bool isDirectionUp, bool willSourceItemBeMovedToDifferentParent)
    {
        if (item is null)
        {
            return;
        }

        if (willSourceItemBeMovedToDifferentParent)
        {
            item.Classes.Remove(RowDraggingDownStyleClass);
            item.Classes.Remove(RowDraggingUpStyleClass);
            item.Classes.Add(TargetHighlightStyleClass);
        }
        else if (isDirectionUp)
        {
            item.Classes.Remove(RowDraggingDownStyleClass);
            item.Classes.Remove(TargetHighlightStyleClass);
            item.Classes.Add(RowDraggingUpStyleClass);
        }
        else
        {
            item.Classes.Remove(RowDraggingUpStyleClass);
            item.Classes.Remove(TargetHighlightStyleClass);
            item.Classes.Add(RowDraggingDownStyleClass);
        }
    }
}
