using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Centers the attached window on the current screen when it is attached to the visual tree.
/// </summary>
public class CenterWindowBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Window"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> WindowProperty =
        AvaloniaProperty.Register<CenterWindowBehavior, Window?>(nameof(Window));

    /// <summary>
    /// Gets or sets the window to center. If not set, the visual root window is used.
    /// </summary>
    [ResolveByName]
    public Window? Window
    {
        get => GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        Center();
    }

    private void Center()
    {
        var window = Window ?? AssociatedObject?.GetVisualRoot() as Window;
        if (window is null)
        {
            return;
        }

        var screen = window.Screens.ScreenFromWindow(window);
        if (screen is null)
        {
            return;
        }

        var rect = screen.WorkingArea;
        var x = rect.X + (rect.Width - window.Width) / 2;
        var y = rect.Y + (rect.Height - window.Height) / 2;
        window.Position = new PixelPoint((int)x, (int)y);
    }
}
