using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows a <see cref="Popup"/> when executed.
/// </summary>
public class ShowPopupAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Popup"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Popup?> PopupProperty =
        AvaloniaProperty.Register<ShowPopupAction, Popup?>(nameof(Popup));

    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ShowPopupAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the popup instance to show.
    /// </summary>
    public Popup? Popup
    {
        get => GetValue(PopupProperty);
        set => SetValue(PopupProperty, value);
    }

    /// <summary>
    /// Gets or sets the target control that hosts the popup. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        var popup = Popup;
        if (control is null || popup is null)
        {
            return false;
        }

        if (popup.PlacementTarget is null)
        {
            popup.PlacementTarget = control;
        }

        popup.Open();
        return true;
    }
}
