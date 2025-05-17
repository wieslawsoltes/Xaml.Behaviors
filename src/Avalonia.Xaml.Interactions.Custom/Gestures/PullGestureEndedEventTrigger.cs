using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions when a pull gesture ends.
/// </summary>
public class PullGestureEndedEventTrigger : RoutedEventTriggerBase<PullGestureEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PullGestureEndedEventArgs> RoutedEvent
        => Gestures.PullGestureEndedEvent;

    static PullGestureEndedEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PullGestureEndedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
