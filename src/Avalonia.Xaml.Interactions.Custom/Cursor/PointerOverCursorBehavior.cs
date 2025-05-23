using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Changes the cursor when the pointer is over the associated control.
/// </summary>
public class PointerOverCursorBehavior : StyledElementBehavior<InputElement>
{
    /// <summary>
    /// Identifies the <seealso cref="Cursor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Cursor?> CursorProperty =
        AvaloniaProperty.Register<PointerOverCursorBehavior, Cursor?>(nameof(Cursor));

    /// <summary>
    /// Gets or sets the cursor to apply while the pointer is over the control.
    /// </summary>
    public Cursor? Cursor
    {
        get => GetValue(CursorProperty);
        set => SetValue(CursorProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerEnteredEvent, OnPointerEntered,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerExitedEvent, OnPointerExited,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerEnteredEvent, OnPointerEntered);
            AssociatedObject.RemoveHandler(InputElement.PointerExitedEvent, OnPointerExited);
        }
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is not null && Cursor is not null)
        {
            AssociatedObject.SetCurrentValue(InputElement.CursorProperty, Cursor);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
