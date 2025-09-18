using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public class ManagedItemsListBoxDropHandler : DropHandlerBase
{
    private static bool TryGetManagedCollections(ListBox listBox, DragAndDropSampleViewModel vm, out ObservableCollection<DragItemViewModel> items)
    {
        if (ReferenceEquals(listBox.ItemsSource, vm.ManagedSourceItems))
        {
            items = vm.ManagedSourceItems;
            return true;
        }

        if (ReferenceEquals(listBox.ItemsSource, vm.ManagedTargetItems))
        {
            items = vm.ManagedTargetItems;
            return true;
        }

        items = default!;
        return false;
    }

    private static int GetTargetIndex(ListBox listBox, DragEventArgs e, ObservableCollection<DragItemViewModel> items, ListBoxItem? explicitTarget)
    {
        if (explicitTarget?.DataContext is DragItemViewModel targetFromContainer)
        {
            var containerIndex = items.IndexOf(targetFromContainer);
            if (containerIndex >= 0)
            {
                return containerIndex;
            }
        }

        if (listBox.GetVisualAt(e.GetPosition(listBox)) is Control targetControl
            && targetControl.DataContext is DragItemViewModel targetItem)
        {
            var index = items.IndexOf(targetItem);
            if (index >= 0)
            {
                return index;
            }
        }

        return items.Count > 0 ? items.Count - 1 : -1;
    }

    private bool Validate(ListBox listBox, ListBoxItem? targetContainer, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        if (sourceContext is not DragItemViewModel sourceItem
            || targetContext is not DragAndDropSampleViewModel vm
            || TryGetManagedCollections(listBox, vm, out var targetItems) == false)
        {
            return false;
        }

        var targetIndex = GetTargetIndex(listBox, e, targetItems, targetContainer);
        var insertIndex = targetIndex >= 0 ? targetIndex + 1 : targetItems.Count;

        var sourceItems = vm.ManagedSourceItems.Contains(sourceItem)
            ? vm.ManagedSourceItems
            : vm.ManagedTargetItems.Contains(sourceItem)
                ? vm.ManagedTargetItems
                : null;

        if (sourceItems is null)
        {
            return false;
        }

        var sourceIndex = sourceItems.IndexOf(sourceItem);
        if (sourceIndex < 0)
        {
            return false;
        }

        switch (e.DragEffects)
        {
            case DragDropEffects.Copy:
            {
                if (execute)
                {
                    var clone = new DragItemViewModel { Title = sourceItem.Title + "_copy" };
                    InsertItem(targetItems, clone, insertIndex);
                }
                return true;
            }
            case DragDropEffects.Move:
            {
                if (execute)
                {
                    if (ReferenceEquals(sourceItems, targetItems))
                    {
                        var moveIndex = targetIndex >= 0 ? targetIndex : targetItems.Count - 1;
                        if (moveIndex < 0)
                        {
                            moveIndex = 0;
                        }

                        MoveItem(targetItems, sourceIndex, moveIndex);
                    }
                    else
                    {
                        MoveItem(sourceItems, targetItems, sourceIndex, insertIndex);
                    }
                }
                return true;
            }
            case DragDropEffects.Link:
            {
                if (targetIndex < 0)
                {
                    return false;
                }

                if (execute)
                {
                    if (ReferenceEquals(sourceItems, targetItems))
                    {
                        SwapItem(targetItems, sourceIndex, targetIndex);
                    }
                    else
                    {
                        SwapItem(sourceItems, targetItems, sourceIndex, targetIndex);
                    }
                }
                return true;
            }
            default:
                return false;
        }
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (sender is ListBox listBox)
        {
            return Validate(listBox, null, e, sourceContext, targetContext, false);
        }

        if (sender is ListBoxItem listBoxItem && listBoxItem.FindAncestorOfType<ListBox>() is { } owner)
        {
            return Validate(owner, listBoxItem, e, sourceContext, targetContext, false);
        }

        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (sender is ListBox listBox)
        {
            return Validate(listBox, null, e, sourceContext, targetContext, true);
        }

        if (sender is ListBoxItem listBoxItem && listBoxItem.FindAncestorOfType<ListBox>() is { } owner)
        {
            return Validate(owner, listBoxItem, e, sourceContext, targetContext, true);
        }

        return false;
    }
}
