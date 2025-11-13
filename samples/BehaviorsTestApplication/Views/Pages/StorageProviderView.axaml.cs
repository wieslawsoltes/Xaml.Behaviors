using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Core;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class StorageProviderView : UserControl
{
    public StorageProviderView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ButtonOpenFolderPickerBehavior_OnPick(object? sender, FolderPickerEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var items = viewModel.FileItems;

        foreach (var folder in e.Folders)
        {
            if (items is not null && folder.Path is { } path)
            {
                items.Add(path);
            }

            viewModel.DocumentsFolder ??= folder;
        }

        e.Handled = true;
    }
}
