using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class DoubleTappedTrigger : RoutedEventTriggerBase<TappedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TappedEventArgs> RoutedEvent 
        => InputElement.DoubleTappedEvent;

    static DoubleTappedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<DoubleTappedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
