using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ThemeVariantView : UserControl
{
    public static ThemeVariant Pink { get; } = new("Pink", ThemeVariant.Light);

    public ThemeVariantView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
