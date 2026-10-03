#if UNO
using BehaviorsTestApplication.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class NetworkView : UserControl
{
    public NetworkView()
    {
        InitializeComponent();
    }

#if UNO
    /// <summary>
    /// Gets the view model for the compiled bindings (x:Bind) of the Uno Platform view.
    /// </summary>
    public NetworkViewModel? ViewModel => DataContext as NetworkViewModel;
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
