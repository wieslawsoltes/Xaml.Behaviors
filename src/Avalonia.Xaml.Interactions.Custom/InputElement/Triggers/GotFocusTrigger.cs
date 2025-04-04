using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class GotFocusTrigger : RoutedEventTriggerBase<GotFocusEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<GotFocusEventArgs> RoutedEvent 
        => InputElement.GotFocusEvent;

    static GotFocusTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<GotFocusTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
