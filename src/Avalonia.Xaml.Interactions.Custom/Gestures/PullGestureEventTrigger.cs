using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions during a pull gesture.
/// </summary>
public class PullGestureEventTrigger : RoutedEventTriggerBase<PullGestureEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PullGestureEventArgs> RoutedEvent
        => Gestures.PullGestureEvent;

    static PullGestureEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PullGestureEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
