using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class DoubleTappedGestureTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent 
        => Gestures.DoubleTappedEvent;
 
    static DoubleTappedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<DoubleTappedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
