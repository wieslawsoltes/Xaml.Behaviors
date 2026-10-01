#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views;

public partial class SampleView : UserControl
{
    public SampleView()
    {
        InitializeComponent();
        DataContext = new SampleViewModel();
    }

#if UNO
    /// <summary>
    /// Gets the view model for the compiled bindings (x:Bind) of the Uno Platform view.
    /// </summary>
    public SampleViewModel? ViewModel => DataContext as SampleViewModel;
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
