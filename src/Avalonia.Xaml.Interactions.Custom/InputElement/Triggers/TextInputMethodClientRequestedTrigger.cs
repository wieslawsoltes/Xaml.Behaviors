using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TextInputMethodClientRequestedTrigger : RoutedEventTriggerBase<TextInputMethodClientRequestedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TextInputMethodClientRequestedEventArgs> RoutedEvent 
        => InputElement.TextInputMethodClientRequestedEvent;

    static TextInputMethodClientRequestedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TextInputMethodClientRequestedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
