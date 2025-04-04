using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TappedTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent 
        => InputElement.TappedEvent;
    
    static TappedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TappedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
