using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ThemeVariantBehaviorView : UserControl
{
    public ThemeVariantBehaviorView()
    {
        InitializeComponent();

        var lightButton = this.FindControl<Button>("LightButton");
        var darkButton = this.FindControl<Button>("DarkButton");

        if (lightButton is not null)
        {
            lightButton.Click += (_, _) => ChangeTheme(ThemeVariant.Light);
        }

        if (darkButton is not null)
        {
            darkButton.Click += (_, _) => ChangeTheme(ThemeVariant.Dark);
        }
    }

    private static void ChangeTheme(ThemeVariant variant)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = variant;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
