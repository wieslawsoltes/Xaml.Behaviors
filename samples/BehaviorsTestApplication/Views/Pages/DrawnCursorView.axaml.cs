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

public partial class DrawnCursorView : UserControl
{
    public DrawnCursorView()
    {
        InitializeComponent();
#if !UNO
        // DrawnCursorViewModel creates Avalonia cursors; the Uno Platform view shows that the sample is not available.
        DataContext = new DrawnCursorViewModel();
#endif
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
