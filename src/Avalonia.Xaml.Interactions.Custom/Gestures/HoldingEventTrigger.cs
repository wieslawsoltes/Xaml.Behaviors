using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers actions when a holding gesture is detected.
/// </summary>
public class HoldingEventTrigger : RoutedEventTriggerBase<HoldingRoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<HoldingRoutedEventArgs> RoutedEvent
        => Gestures.HoldingEvent;

    static HoldingEventTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<HoldingEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
