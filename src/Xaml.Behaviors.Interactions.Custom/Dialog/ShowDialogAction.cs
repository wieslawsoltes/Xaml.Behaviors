using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows a <see cref="Window"/> as a dialog.
/// </summary>
public class ShowDialogAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Dialog"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> DialogProperty =
        AvaloniaProperty.Register<ShowDialogAction, Window?>(nameof(Dialog));

    /// <summary>
    /// Identifies the <seealso cref="Owner"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> OwnerProperty =
        AvaloniaProperty.Register<ShowDialogAction, Window?>(nameof(Owner));

    /// <summary>
    /// Gets or sets the dialog window instance. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Window? Dialog
    {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    /// <summary>
    /// Gets or sets the owner window for the dialog. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Window? Owner
    {
        get => GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var dialog = Dialog;
        if (dialog is null)
        {
            return false;
        }

        var owner = Owner ?? (sender as Visual)?.GetVisualRoot() as Window;
        if (owner is not null)
        {
            dialog.ShowDialog(owner);
        }
        else
        {
            dialog.Show();
        }

        return true;
    }
}
