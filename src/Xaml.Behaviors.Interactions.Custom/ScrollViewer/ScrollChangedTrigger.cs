using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the <see cref="ScrollViewer.ScrollChanged"/> event occurs.
/// </summary>
public class ScrollChangedTrigger : RoutedEventTriggerBase<ScrollChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<ScrollChangedEventArgs> RoutedEvent
        => ScrollViewer.ScrollChangedEvent;

    static ScrollChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ScrollChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
