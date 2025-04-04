using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class HoldingGestureTrigger : RoutedEventTriggerBase<HoldingRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<HoldingRoutedEventArgs> RoutedEvent 
        => Gestures.HoldingEvent;
 
    static HoldingGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<HoldingGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
