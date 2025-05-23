using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerPressedTrigger : RoutedEventTriggerBase<PointerPressedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerPressedEventArgs> RoutedEvent 
        => InputElement.PointerPressedEvent;

    static PointerPressedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerPressedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
