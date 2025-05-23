using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the associated <see cref="Carousel"/> changes selection.
/// </summary>
public class CarouselSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => SelectingItemsControl.SelectionChangedEvent;

    static CarouselSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<CarouselSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
