using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the cursor for the associated control when attached.
/// </summary>
public class SetCursorBehavior : StyledElementBehavior<InputElement>
{
    /// <summary>
    /// Identifies the <seealso cref="Cursor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Cursor?> CursorProperty =
        AvaloniaProperty.Register<SetCursorBehavior, Cursor?>(nameof(Cursor));

    /// <summary>
    /// Gets or sets the cursor to apply.
    /// </summary>
    public Cursor? Cursor
    {
        get => GetValue(CursorProperty);
        set => SetValue(CursorProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null && Cursor is not null)
        {
            AssociatedObject.SetCurrentValue(InputElement.CursorProperty, Cursor);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
