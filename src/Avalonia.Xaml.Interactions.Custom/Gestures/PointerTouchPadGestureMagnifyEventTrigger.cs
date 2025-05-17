using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions during a touchpad magnify gesture.
/// </summary>
public class PointerTouchPadGestureMagnifyEventTrigger : RoutedEventTriggerBase<PointerDeltaEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerDeltaEventArgs> RoutedEvent
        => Gestures.PointerTouchPadGestureMagnifyEvent;

    static PointerTouchPadGestureMagnifyEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerTouchPadGestureMagnifyEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
