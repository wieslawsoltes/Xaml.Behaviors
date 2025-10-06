using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace BehaviorsTestApplication.Behaviors;

public abstract class BaseTreeViewDropHandler : DropHandlerBase
{
    private const string rowDraggingUpStyleClass = "DraggingUp";
    private const string rowDraggingDownStyleClass = "DraggingDown";
    private const string targetHighlightStyleClass = "TargetHighlight";

    protected abstract (bool Valid, bool WillSourceItemBeMovedToDifferentParent) Validate(TreeView tv, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute);

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is TreeView tv)
        {
            var (valid, willSourceItemChangeParent) = Validate(tv, e, sourceContext, targetContext, false);
            var targetVisual = tv.GetVisualAt(e.GetPosition(tv));
            if (valid)
            {
                var targetItem = FindTreeViewItemFromChildView(targetVisual);

                // Node dragged to non-sibling node: becomes sibling
                // Node dragged to sibling node: reordering

                // Reordering effect: adorner layer, on top or bottom.
                // Change of parent: highlight parent.

                if (targetItem is not null)
                {
                    var isDirectionUp = e.GetPosition(targetItem).Y < targetItem.Bounds.Height / 2;
                    var itemToApplyStyle = (willSourceItemChangeParent && targetItem?.Parent is TreeViewItem tviParent) ?
                                           tviParent : targetItem;
                    string direction = e.Data.Contains("direction") ? (string)e.Data.Get("direction")! : "down";
                    ApplyDraggingStyleToItem(itemToApplyStyle!, isDirectionUp, willSourceItemChangeParent);
                    ClearDraggingStyleFromAllItems(sender, exceptThis: itemToApplyStyle);
                }
            }
            return valid;
        }
        ClearDraggingStyleFromAllItems(sender);
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        ClearDraggingStyleFromAllItems(sender);
        if (e.Source is Control && sender is TreeView tv)
        {
            var (valid, _) = Validate(tv, e, sourceContext, targetContext, true);
            return valid;
        }
        return false;
    }

    public override void Cancel(object? sender, RoutedEventArgs e)
    {
        base.Cancel(sender, e);
        // This is necessary to clear styles
        // when mouse exists TreeView, else,
        // they would remain even after changing screens.
        ClearDraggingStyleFromAllItems(sender);
    }

    private static TreeViewItem? FindTreeViewItemFromChildView(StyledElement? sourceChild)
    {
        if (sourceChild is null)
            return null;

        return sourceChild.FindLogicalAncestorOfType<TreeViewItem>();
    }

    private static void ClearDraggingStyleFromAllItems(object? sender, TreeViewItem? exceptThis = null)
    {
        if (sender is not Visual rootVisual)
            return;

        foreach (var item in rootVisual.GetLogicalChildren().OfType<TreeViewItem>())
        {
            if (item == exceptThis)
                continue;

            if (item.Classes is not null)
            {
                item.Classes.Remove(rowDraggingUpStyleClass);
                item.Classes.Remove(rowDraggingDownStyleClass);
                item.Classes.Remove(targetHighlightStyleClass);
            }
            ClearDraggingStyleFromAllItems(item, exceptThis);
        }
    }

    private static void ApplyDraggingStyleToItem(TreeViewItem? item, bool isDirectionUp, bool willSourceItemBeMovedToDifferentParent)
    {
        if (item is null)
            return;

        // Avalonia's Classes.Add() verifies
        // if a class has already been added
        // (avoiding duplications); no need to
        // verify .Contains() here.
        if (willSourceItemBeMovedToDifferentParent)
        {
            item.Classes.Remove(rowDraggingDownStyleClass);
            item.Classes.Remove(rowDraggingUpStyleClass);
            item.Classes.Add(targetHighlightStyleClass);
        }
        else if (isDirectionUp)
        {
            item.Classes.Remove(rowDraggingDownStyleClass);
            item.Classes.Remove(targetHighlightStyleClass);
            item.Classes.Add(rowDraggingUpStyleClass);
        }
        else
        {
            item.Classes.Remove(rowDraggingUpStyleClass);
            item.Classes.Remove(targetHighlightStyleClass);
            item.Classes.Add(rowDraggingDownStyleClass);
        }
    }
}
