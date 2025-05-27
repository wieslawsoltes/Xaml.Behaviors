using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.TextInputMethodClientRequestedEvent"/>.
/// </summary>
public class TextInputMethodClientRequestedEventTrigger : InteractiveTriggerBase
{
    static TextInputMethodClientRequestedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<TextInputMethodClientRequestedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.TextInputMethodClientRequestedEvent, OnClientRequested, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.TextInputMethodClientRequestedEvent, OnClientRequested);
    }

    private void OnClientRequested(object? sender, TextInputMethodClientRequestedEventArgs e)
    {
        Execute(e);
    }
}
