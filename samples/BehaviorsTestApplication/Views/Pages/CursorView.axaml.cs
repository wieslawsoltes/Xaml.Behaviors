using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class CursorView : UserControl
{
    public CursorView()
    {
        InitializeComponent();
        DataContext = new CursorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
