using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for pinch gestures and triggers its actions when detected.
/// </summary>
public abstract class PinchEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PinchEvent, Pinch, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PinchEvent, Pinch);
    }

    private void Pinch(object? sender, PinchEventArgs e)
    {
        OnPinch(sender, e);
    }

    /// <summary>
    /// Called when a pinch gesture is detected.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPinch(object? sender, PinchEventArgs e)
    {
    }
}
