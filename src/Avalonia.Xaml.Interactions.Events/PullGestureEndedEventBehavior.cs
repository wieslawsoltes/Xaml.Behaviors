using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for pull gesture end events and triggers its actions when detected.
/// </summary>
public abstract class PullGestureEndedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.PullGestureEndedEvent, PullGestureEnded, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.PullGestureEndedEvent, PullGestureEnded);
    }

    private void PullGestureEnded(object? sender, PullGestureEndedEventArgs e)
    {
        OnPullGestureEnded(sender, e);
    }

    /// <summary>
    /// Called when a pull gesture ends.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnPullGestureEnded(object? sender, PullGestureEndedEventArgs e)
    {
    }
}
