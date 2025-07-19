using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DragDropCommandsView : UserControl
{
    public DragDropCommandsView()
    {
        InitializeComponent();
        DataContext = new DragDropCommandsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
