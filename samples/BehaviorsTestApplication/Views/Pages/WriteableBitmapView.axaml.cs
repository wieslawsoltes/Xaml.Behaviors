using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class WriteableBitmapView : UserControl, ISamplePage
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
        // TODO:
        // var behavior = this.FindControl<WriteableBitmapBehavior>("StaticBehavior");
        // behavior?.Render();
    }
}
