using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for pull gestures and triggers its actions when detected.
/// </summary>
public abstract class PullGestureEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PullGestureEvent, PullGesture, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PullGestureEvent, PullGesture);
    }

    private void PullGesture(object? sender, PullGestureEventArgs e)
    {
        OnPullGesture(sender, e);
    }

    /// <summary>
    /// Called when a pull gesture occurs.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPullGesture(object? sender, PullGestureEventArgs e)
    {
    }
}
