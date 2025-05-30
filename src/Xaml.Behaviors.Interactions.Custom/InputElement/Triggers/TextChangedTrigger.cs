using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <see cref="TextBox.TextChangedEvent"/>.
/// </summary>
public class TextChangedTrigger : RoutedEventTriggerBase<RoutedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<RoutedEventArgs> RoutedEvent
        => TextBox.TextChangedEvent;

    static TextChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TextChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
