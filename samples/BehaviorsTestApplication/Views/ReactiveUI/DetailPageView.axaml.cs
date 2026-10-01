#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif
using BehaviorsTestApplication.ViewModels;
#if !UNO
using ReactiveUI.Avalonia.Reactive;
#endif

namespace BehaviorsTestApplication.Views.Pages;

#if UNO
// WinUI XAML cannot use a generic root type: DetailPageViewBase closes ReactiveUserControl<DetailPageViewModel> (ReactiveUI.Uno).
public partial class DetailPageView : DetailPageViewBase
#else
public partial class DetailPageView : ReactiveUserControl<DetailPageViewModel>
#endif
{
    public DetailPageView()
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
