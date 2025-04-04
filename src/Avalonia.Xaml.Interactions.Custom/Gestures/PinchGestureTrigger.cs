using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PinchGestureTrigger : RoutedEventTriggerBase<PinchEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEventArgs> RoutedEvent
        => Gestures.PinchEvent;

    static PinchGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
