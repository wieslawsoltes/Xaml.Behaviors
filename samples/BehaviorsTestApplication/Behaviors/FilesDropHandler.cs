using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public sealed class FilesDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return e.Data.Contains(DataFormats.FileNames) && targetContext is MainWindowViewModel;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (!e.Data.Contains(DataFormats.FileNames) || targetContext is not MainWindowViewModel vm)
        {
            return false;
        }

        var files = e.Data.GetFiles();
        if (files is null)
        {
            return false;
        }
            
        foreach (var file in files)
        {
            vm.FileItems?.Add(file.Path);
        }

        return true;
    }
}
