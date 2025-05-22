using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Custom.WriteableBitmap;

namespace BehaviorsTestApplication.Views.Pages;

public partial class WriteableBitmapView : UserControl
{
    public WriteableBitmapView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRenderOnce(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var behavior = this.FindControl<WriteableBitmapBehavior>("StaticBehavior");
        behavior?.Render();
    }
}
