using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="Gestures.ScrollGestureEvent"/>.
/// </summary>
public abstract class ScrollGestureEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.ScrollGestureEvent, ScrollGesture, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.ScrollGestureEvent, ScrollGesture);
    }

    private void ScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        OnScrollGesture(sender, e);
    }

    /// <summary>
    /// Called when a scroll gesture occurs on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
    }
}
