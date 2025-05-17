using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for pinch end gestures and triggers its actions when detected.
/// </summary>
public abstract class PinchEndedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PinchEndedEvent, PinchEnded, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PinchEndedEvent, PinchEnded);
    }

    private void PinchEnded(object? sender, PinchEndedEventArgs e)
    {
        OnPinchEnded(sender, e);
    }

    /// <summary>
    /// Called when a pinch gesture ends.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPinchEnded(object? sender, PinchEndedEventArgs e)
    {
    }
}
