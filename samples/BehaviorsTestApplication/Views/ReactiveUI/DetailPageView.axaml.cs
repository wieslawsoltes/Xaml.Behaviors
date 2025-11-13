using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;
using ReactiveUI.Avalonia;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DetailPageView : ReactiveUserControl<DetailPageViewModel>
{
    public DetailPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
