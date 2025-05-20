using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Shows the associated or target control when executed.
/// </summary>
public class ShowControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ShowControlAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = GetValue(TargetControlProperty) is not null ? TargetControl : sender as Control;
        if (control is null)
        {
            return false;
        }

        if (!control.IsVisible)
        {
            control.SetCurrentValue(Visual.IsVisibleProperty, true);
            Dispatcher.UIThread.Post(() => control.Focus());
        }

        return true;
    }
}
