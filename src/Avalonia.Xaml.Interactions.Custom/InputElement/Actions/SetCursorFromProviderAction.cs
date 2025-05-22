using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the cursor on a target control using an <see cref="ICursorProvider"/>.
/// </summary>
public class SetCursorFromProviderAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<InputElement?> TargetControlProperty =
        AvaloniaProperty.Register<SetCursorFromProviderAction, InputElement?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <see cref="Provider"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ICursorProvider?> ProviderProperty =
        AvaloniaProperty.Register<SetCursorFromProviderAction, ICursorProvider?>(nameof(Provider));

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public InputElement? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="ICursorProvider"/>. This is an avalonia property.
    /// </summary>
    public ICursorProvider? Provider
    {
        get => GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as InputElement;
        if (control is null || Provider is null)
        {
            return false;
        }

        control.Cursor = Provider.CreateCursor();
        return true;
    }
}
