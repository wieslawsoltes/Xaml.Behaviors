using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Closes a <see cref="ContextDialogBehavior"/> when executed.
/// </summary>
public class HideContextDialogAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetDialog"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ContextDialogBehavior?> TargetDialogProperty =
        AvaloniaProperty.Register<HideContextDialogAction, ContextDialogBehavior?>(nameof(TargetDialog));

    /// <summary>
    /// Gets or sets the target dialog behavior. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ContextDialogBehavior? TargetDialog
    {
        get => GetValue(TargetDialogProperty);
        set => SetValue(TargetDialogProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var dialog = TargetDialog;
        if (dialog is null)
        {
            return false;
        }

        dialog.IsOpen = false;
        return true;
    }
}
