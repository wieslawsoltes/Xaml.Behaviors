using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that listens for <see cref="Gestures.DoubleTappedEvent"/>.
/// </summary>
public abstract class DoubleTappedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.DoubleTappedEvent, DoubleTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.DoubleTappedEvent, DoubleTapped);
    }

    private void DoubleTapped(object? sender, RoutedEventArgs e)
    {
        OnDoubleTapped(sender, e);
    }

    /// <summary>
    /// Called when a double tap gesture is raised on the associated object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
    }
}
