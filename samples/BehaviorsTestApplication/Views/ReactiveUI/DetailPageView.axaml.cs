using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BehaviorsTestApplication.ViewModels;

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
