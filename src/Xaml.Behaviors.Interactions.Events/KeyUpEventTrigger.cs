using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.KeyUpEvent"/>.
/// </summary>
public class KeyUpEventTrigger : InteractiveTriggerBase
{
    static KeyUpEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<KeyUpEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.KeyUpEvent, OnKeyUp, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyUpEvent, OnKeyUp);
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        Execute(e);
    }
}
