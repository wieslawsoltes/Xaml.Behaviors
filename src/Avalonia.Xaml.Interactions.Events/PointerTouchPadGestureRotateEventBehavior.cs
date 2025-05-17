using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for touchpad rotate gestures and triggers its actions when detected.
/// </summary>
public abstract class PointerTouchPadGestureRotateEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PointerTouchPadGestureRotateEvent, PointerTouchPadGestureRotate, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PointerTouchPadGestureRotateEvent, PointerTouchPadGestureRotate);
    }

    private void PointerTouchPadGestureRotate(object? sender, PointerDeltaEventArgs e)
    {
        OnPointerTouchPadGestureRotate(sender, e);
    }

    /// <summary>
    /// Called when a touchpad rotation gesture occurs.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPointerTouchPadGestureRotate(object? sender, PointerDeltaEventArgs e)
    {
    }
}
