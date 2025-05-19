using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ReactiveNavigationView : UserControl
{
    public ReactiveNavigationView()
    {
        InitializeComponent();
        DataContext = new ReactiveNavigationViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
