using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ClipboardMonitorBehaviorView : UserControl
{
    public ClipboardMonitorBehaviorView()
    {
        InitializeComponent();
    }

    private async void PasteButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.Clipboard is { } clipboard)
        {
            var text = await ClipboardExtensions.TryGetTextAsync(clipboard);
            var outputBox = this.FindControl<TextBox>("OutputBox");
            if (outputBox != null)
            {
                outputBox.Text = text;
            }
        }
    }
}
