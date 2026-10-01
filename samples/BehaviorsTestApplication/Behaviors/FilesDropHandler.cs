#if UNO
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactions.DragAndDrop;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public sealed class FilesDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
#if UNO
        return e.DataView.Contains(StandardDataFormats.StorageItems) && targetContext is MainWindowViewModel;
#else
        return e.DataTransfer.Contains(DataFormat.File) && targetContext is MainWindowViewModel;
#endif
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
#if UNO
        if (!e.DataView.Contains(StandardDataFormats.StorageItems) || targetContext is not MainWindowViewModel vm)
        {
            return false;
        }

        // WinUI reads the dropped files asynchronously.
        _ = AddFilesAsync(e.DataView, vm);
        return true;
#else
        if (!e.DataTransfer.Contains(DataFormat.File) || targetContext is not MainWindowViewModel vm)
        {
            return false;
        }

        var files = e.DataTransfer.TryGetFiles();
        if (files is null || files.Length == 0)
        {
            return false;
        }
            
        foreach (var file in files)
        {
            vm.FileItems?.Add(file.Path);
        }

        return true;
#endif
    }

#if UNO
    private static async Task AddFilesAsync(DataPackageView view, MainWindowViewModel vm)
    {
        var items = await view.GetStorageItemsAsync();
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Path))
            {
                vm.FileItems?.Add(new Uri(item.Path));
            }
        }
    }
#endif
}
