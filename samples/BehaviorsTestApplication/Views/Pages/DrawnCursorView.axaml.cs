using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DrawnCursorView : UserControl, ISamplePage
{
    public DrawnCursorView()
    {
        InitializeComponent();
        DataContext = new DrawnCursorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
