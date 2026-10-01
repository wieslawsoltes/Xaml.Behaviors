#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class CursorView : UserControl
{
    public CursorView()
    {
        InitializeComponent();
#if !UNO
        // CursorViewModel creates Avalonia cursors; the Uno Platform view shows that the sample is not available.
        DataContext = new CursorViewModel();
#endif
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
