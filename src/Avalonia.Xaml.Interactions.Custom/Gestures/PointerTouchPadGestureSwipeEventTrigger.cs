using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions during a touchpad swipe gesture.
/// </summary>
public class PointerTouchPadGestureSwipeEventTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureSwipeEvent;

    static PointerTouchPadGestureSwipeEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureSwipeEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
