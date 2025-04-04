using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public class ItemsListBoxDropHandler : DropHandlerBase
{
    private bool Validate<T>(ListBox listBox, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute) where T : DragItemViewModel
    {
        if (sourceContext is not T sourceItem
            || targetContext is not DragAndDropSampleViewModel vm
            || listBox.GetVisualAt(e.GetPosition(listBox)) is not Control targetControl
            || targetControl.DataContext is not T targetItem)
        {
            return false;
        }

        var items = vm.Items;
        var sourceIndex = items.IndexOf(sourceItem);
        var targetIndex = items.IndexOf(targetItem);

        if (sourceIndex < 0 || targetIndex < 0)
        {
            return false;
        }

        switch (e.DragEffects)
        {
            case DragDropEffects.Copy:
            {
                if (bExecute)
                {
                    var clone = new DragItemViewModel() { Title = sourceItem.Title + "_copy" };
                    InsertItem(items, clone, targetIndex + 1);
                }
                return true;
            }
            case DragDropEffects.Move:
            {
                if (bExecute)
                {
                    MoveItem(items, sourceIndex, targetIndex);
                }
                return true;
            }
            case DragDropEffects.Link:
            {
                if (bExecute)
                {
                    SwapItem(items, sourceIndex, targetIndex);
                }
                return true;
            }
            default:
                return false;
        }
    }
        
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return Validate<DragItemViewModel>(listBox, e, sourceContext, targetContext, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return Validate<DragItemViewModel>(listBox, e, sourceContext, targetContext, true);
        }
        return false;
    }
}
