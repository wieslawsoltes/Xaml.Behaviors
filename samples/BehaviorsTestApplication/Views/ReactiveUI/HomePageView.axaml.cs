using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;
using ReactiveUI.Avalonia;

namespace BehaviorsTestApplication.Views.Pages;

public partial class HomePageView : ReactiveUserControl<HomePageViewModel>
{
    public HomePageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
