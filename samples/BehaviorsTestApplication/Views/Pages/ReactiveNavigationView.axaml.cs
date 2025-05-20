using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ReactiveNavigationView : ReactiveUserControl<ReactiveNavigationViewModel>
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
