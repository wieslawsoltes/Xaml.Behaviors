using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions when a pinch gesture ends.
/// </summary>
public class PinchEndedEventTrigger : RoutedEventTriggerBase<PinchEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEndedEventArgs> RoutedEvent
        => Gestures.PinchEndedEvent;

    static PinchEndedEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchEndedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
