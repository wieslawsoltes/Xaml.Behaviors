using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions during a touchpad rotation gesture.
/// </summary>
public class PointerTouchPadGestureRotateEventTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureRotateEvent;

    static PointerTouchPadGestureRotateEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureRotateEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
