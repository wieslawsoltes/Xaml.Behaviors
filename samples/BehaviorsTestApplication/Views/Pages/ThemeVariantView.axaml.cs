#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class ThemeVariantView : UserControl
{
#if !UNO
    // A custom Avalonia theme variant; WinUI themes are the fixed ElementTheme values.
    public static ThemeVariant Pink { get; } = new("Pink", ThemeVariant.Light);
#endif

    public ThemeVariantView()
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
