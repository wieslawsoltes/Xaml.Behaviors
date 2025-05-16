using System;
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
        if (e.Data.Contains(DataFormats.FileNames) && targetContext is MainWindowViewModel vm)
        {
            foreach (var file in e.Data.GetFileNames())
            {
                if (Uri.TryCreate(file, UriKind.Absolute, out var uri))
                {
                    vm.FileItems.Add(uri);
                }
            }

            return true;
        }

        return false;
    }
}
