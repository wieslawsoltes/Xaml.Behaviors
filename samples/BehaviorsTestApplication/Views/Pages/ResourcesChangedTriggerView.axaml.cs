#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class ResourcesChangedTriggerView : UserControl
{
    public ResourcesChangedTriggerView()
    {
        InitializeComponent();

#if !UNO
        // Not available on Uno Platform: WinUI raises no ResourcesChanged notification (the twin shows a
        // NotAvailableOnUnoView).
        if (this.FindControl<Button>("ChangeButton") is { } button &&
            this.FindControl<Border>("Target") is { } border)
        {
            button.Click += (_, _) => border.Resources["Color"] = Brushes.Blue;
        }
#endif
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
