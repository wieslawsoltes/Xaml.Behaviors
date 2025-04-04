using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerTouchPadGestureSwipeGestureTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureSwipeEvent;

    static PointerTouchPadGestureSwipeGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureSwipeGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
