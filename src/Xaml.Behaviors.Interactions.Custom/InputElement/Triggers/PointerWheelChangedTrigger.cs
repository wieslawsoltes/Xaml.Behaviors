using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerWheelChangedTrigger : RoutedEventTriggerBase<PointerWheelEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerWheelEventArgs> RoutedEvent 
        => InputElement.PointerWheelChangedEvent;

    static PointerWheelChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerWheelChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
