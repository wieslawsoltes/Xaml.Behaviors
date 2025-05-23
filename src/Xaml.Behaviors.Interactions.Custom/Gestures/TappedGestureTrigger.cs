using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TappedGestureTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent
        => Gestures.TappedEvent;

    static TappedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TappedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
