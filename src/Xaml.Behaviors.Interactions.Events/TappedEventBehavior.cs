using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="Gestures.TappedEvent"/>.
/// </summary>
public abstract class TappedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.TappedEvent, Tapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.TappedEvent, Tapped);
    }

    private void Tapped(object? sender, RoutedEventArgs e)
    {
        OnTapped(sender, e);
    }

    /// <summary>
    /// Called when a tap gesture is raised on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnTapped(object? sender, RoutedEventArgs e)
    {
    }
}
