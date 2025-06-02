using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Closes the associated or target window when executed.
/// </summary>
public class CloseWindowAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetWindow"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> TargetWindowProperty =
        AvaloniaProperty.Register<CloseWindowAction, Window?>(nameof(TargetWindow));

    /// <summary>
    /// Gets or sets the target window. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Window? TargetWindow
    {
        get => GetValue(TargetWindowProperty);
        set => SetValue(TargetWindowProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var window = TargetWindow
                     ?? sender as Window
                     ?? (sender as Control)?.GetVisualRoot() as Window;
        if (window is null)
        {
            return false;
        }

        window.Close();
        return true;
    }
}
