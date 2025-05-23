using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <see cref="ToolTip.ToolTipClosingEvent"/>.
/// </summary>
public class ToolTipClosingTrigger : RoutedEventTrigger
{
    /// <inheritdoc />
    protected override RoutedEvent RoutedEvent
        => ToolTip.ToolTipClosingEvent;

    static ToolTipClosingTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<ToolTipClosingTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
