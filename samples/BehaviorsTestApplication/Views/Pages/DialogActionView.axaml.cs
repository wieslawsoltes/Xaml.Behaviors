#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class DialogActionView : UserControl
{
    public DialogActionView()
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
