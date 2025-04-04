using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ScrollGestureInertiaStartingGestureTrigger : RoutedEventTriggerBase<ScrollGestureInertiaStartingEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<ScrollGestureInertiaStartingEventArgs> RoutedEvent
        => Gestures.ScrollGestureInertiaStartingEvent;

    static ScrollGestureInertiaStartingGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollGestureInertiaStartingGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
