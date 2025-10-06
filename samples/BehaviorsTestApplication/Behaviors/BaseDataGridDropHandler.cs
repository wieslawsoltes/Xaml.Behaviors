using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public abstract class BaseDataGridDropHandler<T> : DropHandlerBase
    where T : ViewModelBase
{
    private const string rowDraggingUpStyleClass = "DraggingUp";
    private const string rowDraggingDownStyleClass = "DraggingDown";

    protected abstract T MakeCopy(ObservableCollection<T> parentCollection, T item);

    protected abstract bool Validate(DataGrid dg, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute);

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control c && sender is DataGrid dg)
        {
            bool valid = Validate(dg, e, sourceContext, targetContext, false);
            if (valid)
            {
                var row = FindDataGridRowFromChildView(c);
                if (row is not null)
                {
                    var isDirectionUp = e.GetPosition(row).Y < row.Bounds.Height / 2;
                    ApplyDraggingStyleToRow(row, isDirectionUp);
                }
                ClearDraggingStyleFromAllRows(sender, exceptThis: row);
            }
            return valid;
        }
        ClearDraggingStyleFromAllRows(sender);
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        ClearDraggingStyleFromAllRows(sender);
        if (e.Source is Control && sender is DataGrid dg)
        {
            return Validate(dg, e, sourceContext, targetContext, true);
        }
        return false;
    }

    public override void Cancel(object? sender, RoutedEventArgs e)
    {
        base.Cancel(sender, e);
        // this is necessary to clear adorner borders when mouse leaves DataGrid
        // they would remain even after changing screens
        ClearDraggingStyleFromAllRows(sender);
    }

    protected bool RunDropAction(DataGrid dg, DragEventArgs e, bool bExecute, T sourceItem, T targetItem, ObservableCollection<T> items)
    {
        int sourceIndex = items.IndexOf(sourceItem);
        int targetIndex = items.IndexOf(targetItem);

        if (sourceIndex < 0 || targetIndex < 0)
        {
            return false;
        }

        if (e.Source is Control c)
        {
            var row = FindDataGridRowFromChildView(c);
            if (row is not null && e.GetPosition(row).Y > row.Bounds.Height / 2)
                targetIndex++;
            if (targetIndex > sourceIndex)
                targetIndex--;
        }

        switch (e.DragEffects)
        {
            case DragDropEffects.Copy:
                {
                    if (bExecute)
                    {
                        var clone = MakeCopy(items, sourceItem);
                        InsertItem(items, clone, targetIndex + 1);
                        dg.SelectedIndex = targetIndex + 1;
                    }
                    return true;
                }
            case DragDropEffects.Move:
                {
                    if (bExecute)
                    {
                        MoveItem(items, sourceIndex, targetIndex);
                        dg.SelectedIndex = targetIndex;
                    }
                    return true;
                }
            case DragDropEffects.Link:
                {
                    if (bExecute)
                    {
                        SwapItem(items, sourceIndex, targetIndex);
                        dg.SelectedIndex = targetIndex;
                    }
                    return true;
                }
            default:
                return false;
        }
    }

    private static DataGridRow? FindDataGridRowFromChildView(StyledElement sourceChild)
    {
        return sourceChild.FindLogicalAncestorOfType<DataGridRow>();
    }

    private static DataGridRowsPresenter? GetRowsPresenter(Visual v)
    {
        foreach (var cv in v.GetVisualChildren())
        {
            if (cv is DataGridRowsPresenter dgrp)
                return dgrp;
            else if (GetRowsPresenter(cv) is DataGridRowsPresenter dgrp2)
                return dgrp2;
        }
        return null;
    }

    private static void ClearDraggingStyleFromAllRows(object? sender, DataGridRow? exceptThis = null)
    {
        if (sender is DataGrid dg)
        {
            var presenter = GetRowsPresenter(dg);
            if (presenter is null)
                return;

            foreach (var r in presenter.Children)
            {
                if (r == exceptThis)
                    continue;

                r?.Classes?.Remove(rowDraggingUpStyleClass);
                r?.Classes?.Remove(rowDraggingDownStyleClass);
            }
        }
    }

    private static void ApplyDraggingStyleToRow(DataGridRow row, bool IsDirectionUp)
    {
        if (IsDirectionUp)
        {
            row.Classes.Remove(rowDraggingDownStyleClass);
            row.Classes.Add(rowDraggingUpStyleClass);
        }
        else
        {
            row.Classes.Remove(rowDraggingUpStyleClass);
            row.Classes.Add(rowDraggingDownStyleClass);
        }
    }
}
