using System.Collections.ObjectModel;
#if UNO
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.DragAndDrop;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public sealed class ItemsDataGridDropHandler : BaseDataGridDropHandler<DragItemViewModel>
{
    protected override DragItemViewModel MakeCopy(ObservableCollection<DragItemViewModel> parentCollection, DragItemViewModel dragItem) =>
        new() { Title = dragItem.Title };

    protected override bool Validate(DataGrid dg, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        if (sourceContext is not DragItemViewModel sourceItem
         || targetContext is not DragAndDropSampleViewModel vm
         || dg.GetVisualAt(e.GetPosition(dg)) is not Control targetControl
         || targetControl.DataContext is not DragItemViewModel targetItem)
        {
            return false;
        }

        var items = vm.Items;
        return RunDropAction(dg, e, execute, sourceItem, targetItem, items);
    }
}
