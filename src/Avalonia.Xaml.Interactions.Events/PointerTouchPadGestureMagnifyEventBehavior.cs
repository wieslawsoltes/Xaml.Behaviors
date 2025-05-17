using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for touchpad magnify gestures and triggers its actions when detected.
/// </summary>
public abstract class PointerTouchPadGestureMagnifyEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PointerTouchPadGestureMagnifyEvent, PointerTouchPadGestureMagnify, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PointerTouchPadGestureMagnifyEvent, PointerTouchPadGestureMagnify);
    }

    private void PointerTouchPadGestureMagnify(object? sender, PointerDeltaEventArgs e)
    {
        OnPointerTouchPadGestureMagnify(sender, e);
    }

    /// <summary>
    /// Called when a touchpad magnify gesture occurs.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPointerTouchPadGestureMagnify(object? sender, PointerDeltaEventArgs e)
    {
    }
}
