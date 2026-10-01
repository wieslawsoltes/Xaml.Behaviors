#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Xaml.Interactions.Core;
#else
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class ClipboardMonitorBehaviorView : UserControl
{
    public ClipboardMonitorBehaviorView()
    {
        InitializeComponent();
    }

#if UNO
    // WinUI has a single application clipboard (SystemClipboard wraps Windows.ApplicationModel.DataTransfer.Clipboard).
    private async void PasteButton_Click(object? sender, RoutedEventArgs e)
    {
        var text = await SystemClipboard.Instance.GetTextAsync();
        OutputBox.Text = text;
    }
#else
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
#endif
}
