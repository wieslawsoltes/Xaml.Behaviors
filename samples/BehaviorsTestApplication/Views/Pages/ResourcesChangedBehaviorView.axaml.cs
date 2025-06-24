using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ResourcesChangedBehaviorView : UserControl
{
    public ResourcesChangedBehaviorView()
    {
        InitializeComponent();

        if (this.FindControl<Button>("ChangeButton") is { } button &&
            this.FindControl<Border>("Target") is { } border)
        {
            button.Click += (_, _) => border.Resources["Color"] = Brushes.Red;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
