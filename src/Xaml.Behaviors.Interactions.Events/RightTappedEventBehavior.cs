using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="Gestures.RightTappedEvent"/>.
/// </summary>
public abstract class RightTappedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.RightTappedEvent, RightTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.RightTappedEvent, RightTapped);
    }

    private void RightTapped(object? sender, RoutedEventArgs e)
    {
        OnRightTapped(sender, e);
    }

    /// <summary>
    /// Called when a right-tap gesture is raised on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnRightTapped(object? sender, RoutedEventArgs e)
    {
    }
}
