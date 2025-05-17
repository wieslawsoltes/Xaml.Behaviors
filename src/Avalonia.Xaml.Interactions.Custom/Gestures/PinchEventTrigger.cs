using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions during a pinch gesture.
/// </summary>
public class PinchEventTrigger : RoutedEventTriggerBase<PinchEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEventArgs> RoutedEvent
        => Gestures.PinchEvent;

    static PinchEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
