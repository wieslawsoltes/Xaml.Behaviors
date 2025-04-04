using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public sealed class ItemsDataGridDropHandler : BaseDataGridDropHandler<DragItemViewModel>
{
    protected override DragItemViewModel MakeCopy(ObservableCollection<DragItemViewModel> parentCollection, DragItemViewModel dragItem) =>
        new() { Title = dragItem.Title };

    protected override bool Validate(DataGrid dg, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute)
    {
        if (sourceContext is not DragItemViewModel sourceItem
         || targetContext is not DragAndDropSampleViewModel vm
         || dg.GetVisualAt(e.GetPosition(dg)) is not Control targetControl
         || targetControl.DataContext is not DragItemViewModel targetItem)
        {
            return false;
        }

        var items = vm.Items;
        return RunDropAction(dg, e, bExecute, sourceItem, targetItem, items);
    }
}
