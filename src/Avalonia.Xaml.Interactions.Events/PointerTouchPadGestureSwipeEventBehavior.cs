using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for touchpad swipe gestures and triggers its actions when detected.
/// </summary>
public abstract class PointerTouchPadGestureSwipeEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PointerTouchPadGestureSwipeEvent, PointerTouchPadGestureSwipe, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PointerTouchPadGestureSwipeEvent, PointerTouchPadGestureSwipe);
    }

    private void PointerTouchPadGestureSwipe(object? sender, PointerDeltaEventArgs e)
    {
        OnPointerTouchPadGestureSwipe(sender, e);
    }

    /// <summary>
    /// Called when a touchpad swipe gesture occurs.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPointerTouchPadGestureSwipe(object? sender, PointerDeltaEventArgs e)
    {
    }
}
