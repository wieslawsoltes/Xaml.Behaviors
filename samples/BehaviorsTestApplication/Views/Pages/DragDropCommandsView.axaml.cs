#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DragDropCommandsView : UserControl
{
    public DragDropCommandsView()
    {
        InitializeComponent();
        DataContext = new DragDropCommandsViewModel();
    }

#if UNO
    /// <summary>
    /// Gets the view model for the compiled bindings (x:Bind) of the Uno Platform view.
    /// </summary>
    public DragDropCommandsViewModel? ViewModel => DataContext as DragDropCommandsViewModel;
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
