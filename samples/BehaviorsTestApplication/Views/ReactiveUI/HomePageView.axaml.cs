#if UNO
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Markup.Xaml;
#endif
using BehaviorsTestApplication.ViewModels;
#if !UNO
using ReactiveUI.Avalonia.Reactive;
#endif

namespace BehaviorsTestApplication.Views.Pages;

#if UNO
// WinUI XAML cannot use a generic root type: HomePageViewBase closes ReactiveUserControl<HomePageViewModel> (ReactiveUI.Uno).
public partial class HomePageView : HomePageViewBase
#else
public partial class HomePageView : ReactiveUserControl<HomePageViewModel>
#endif
{
    public HomePageView()
    {
        InitializeComponent();
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
