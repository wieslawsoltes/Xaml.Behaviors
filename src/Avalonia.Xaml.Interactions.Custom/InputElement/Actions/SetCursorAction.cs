using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the cursor on a target control.
/// </summary>
public class SetCursorAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<InputElement?> TargetControlProperty =
        AvaloniaProperty.Register<SetCursorAction, InputElement?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <seealso cref="Cursor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Cursor?> CursorProperty =
        AvaloniaProperty.Register<SetCursorAction, Cursor?>(nameof(Cursor));

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
    /// Gets or sets the cursor to apply.
    /// </summary>
    public Cursor? Cursor
    {
        get => GetValue(CursorProperty);
        set => SetValue(CursorProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as InputElement;
        var cursor = Cursor;
        if (control is null || cursor is null)
        {
            return false;
        }

        control.Cursor = cursor;
        return true;
    }
}
