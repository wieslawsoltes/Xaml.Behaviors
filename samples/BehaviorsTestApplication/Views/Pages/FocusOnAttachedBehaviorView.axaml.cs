#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class FocusOnAttachedBehaviorView : UserControl
{
    public FocusOnAttachedBehaviorView()
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
