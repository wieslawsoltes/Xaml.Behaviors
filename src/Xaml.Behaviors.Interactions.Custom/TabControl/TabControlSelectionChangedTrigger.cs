using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the associated <see cref="TabControl"/> changes selection.
/// </summary>
public class TabControlSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => SelectingItemsControl.SelectionChangedEvent;

    static TabControlSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TabControlSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
