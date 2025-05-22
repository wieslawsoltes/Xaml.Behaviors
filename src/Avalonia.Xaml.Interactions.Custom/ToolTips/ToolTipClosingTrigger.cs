using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <see cref="ToolTip.ToolTipClosingEvent"/>.
/// </summary>
public class ToolTipClosingTrigger : RoutedEventTriggerBase<RoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<RoutedEventArgs> RoutedEvent
        => ToolTip.ToolTipClosingEvent;

    static ToolTipClosingTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipClosingTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
