using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class RightTappedGestureTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent
        => Gestures.RightTappedEvent;

    static RightTappedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<RightTappedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
