using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DraggableView : UserControl, ISamplePage
{
    public DraggableView()
    {
        InitializeComponent();

        DataContext = new DraggableViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
