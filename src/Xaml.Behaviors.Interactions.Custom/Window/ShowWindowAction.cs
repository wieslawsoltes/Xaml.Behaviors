using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows the specified window when executed.
/// </summary>
public class ShowWindowAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Window"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> WindowProperty =
        AvaloniaProperty.Register<ShowWindowAction, Window?>(nameof(Window));

    /// <summary>
    /// Gets or sets the window instance to show. This is an avalonia property.
    /// </summary>
    [Content]
    public Window? Window
    {
        get => GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var window = Window;
        if (window is null)
        {
            return false;
        }

        if (sender is Control control)
        {
            window.DataContext = control.DataContext;
        }

        window.Show();
        return true;
    }
}
