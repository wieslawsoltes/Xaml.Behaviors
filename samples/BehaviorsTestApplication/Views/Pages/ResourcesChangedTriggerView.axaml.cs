using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ResourcesChangedTriggerView : UserControl
{
    public ResourcesChangedTriggerView()
    {
        InitializeComponent();

        if (this.FindControl<Button>("ChangeButton") is { } button &&
            this.FindControl<Border>("Target") is { } border)
        {
            button.Click += (_, _) => border.Resources["Color"] = Brushes.Blue;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
