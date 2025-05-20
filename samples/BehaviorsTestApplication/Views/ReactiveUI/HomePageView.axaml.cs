using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BehaviorsTestApplication.ViewModels;

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
