using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Xaml.Interactions.DragAndDrop.Controls;

/// <summary>
/// Lightweight always-on-top window used to display drag preview content.
/// </summary>
public class DragPreviewWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragPreviewWindow"/> class.
    /// Configures a transparent, non-interactive, topmost window sized to its content.
    /// </summary>
    public DragPreviewWindow()
    {
        CanResize = false;
        ShowInTaskbar = false;
        SystemDecorations = SystemDecorations.None;
        Topmost = true;
        Background = Brushes.Transparent;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        // Disable focus/activation stealing where possible
        WindowStartupLocation = WindowStartupLocation.Manual;
        SizeToContent = SizeToContent.WidthAndHeight;
        IsEnabled = false; // don't accept input
    }
}
