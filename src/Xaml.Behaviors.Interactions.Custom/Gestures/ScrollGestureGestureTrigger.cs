using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ScrollGestureGestureTrigger : RoutedEventTriggerBase<ScrollGestureEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<ScrollGestureEventArgs> RoutedEvent
        => Gestures.ScrollGestureEvent;

    static ScrollGestureGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollGestureGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
