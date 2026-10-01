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
using ReactiveUI.Avalonia;
#endif

namespace BehaviorsTestApplication.Views.Pages;

#if UNO
// WinUI XAML cannot use a generic root type: ReactiveNavigationViewBase closes
// ReactiveUserControl<ReactiveNavigationViewModel> (ReactiveUI.Uno).
public partial class ReactiveNavigationView : ReactiveNavigationViewBase
#else
public partial class ReactiveNavigationView : ReactiveUserControl<ReactiveNavigationViewModel>
#endif
{
    public ReactiveNavigationView()
    {
        InitializeComponent();
        DataContext = new ReactiveNavigationViewModel();
#if UNO
        // The Avalonia ReactiveUserControl synchronizes ViewModel with DataContext; the ReactiveUI.Uno one does not.
        ViewModel = (ReactiveNavigationViewModel)DataContext;
#endif
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
