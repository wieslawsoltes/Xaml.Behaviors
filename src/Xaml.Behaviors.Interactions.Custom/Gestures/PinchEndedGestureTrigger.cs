using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PinchEndedGestureTrigger : RoutedEventTriggerBase<PinchEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEndedEventArgs> RoutedEvent
        => Gestures.PinchEndedEvent;

    static PinchEndedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchEndedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
