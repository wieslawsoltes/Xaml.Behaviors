#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Xaml.Interactions.Core;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Core;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class StorageProviderView : UserControl
{
    public StorageProviderView()
    {
        InitializeComponent();
    }

#if UNO
    /// <summary>
    /// Gets the view model for the compiled bindings (x:Bind) of the Uno Platform view.
    /// </summary>
    public MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif

    private void ButtonOpenFolderPickerBehavior_OnPick(object? sender, FolderPickerEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var items = viewModel.FileItems;

        foreach (var folder in e.Folders)
        {
#if UNO
            // The Uno Platform storage items expose the local path as a string.
            if (items is not null && !string.IsNullOrEmpty(folder.Path))
            {
                items.Add(new System.Uri(folder.Path));
            }
#else
            if (items is not null && folder.Path is { } path)
            {
                items.Add(path);
            }
#endif

            viewModel.DocumentsFolder ??= folder;
        }

        e.Handled = true;
    }
}
