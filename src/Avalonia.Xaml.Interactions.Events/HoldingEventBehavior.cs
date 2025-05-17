using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Listens for holding gestures and triggers its actions when detected.
/// </summary>
public abstract class HoldingEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.HoldingEvent, Holding, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.HoldingEvent, Holding);
    }

    private void Holding(object? sender, HoldingRoutedEventArgs e)
    {
        OnHolding(sender, e);
    }

    /// <summary>
    /// Called when a holding gesture is detected.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event args.</param>
    protected virtual void OnHolding(object? sender, HoldingRoutedEventArgs e)
    {
    }
}
