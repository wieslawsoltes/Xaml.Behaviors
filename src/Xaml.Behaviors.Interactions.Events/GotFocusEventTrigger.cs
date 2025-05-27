using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.GotFocusEvent"/>.
/// </summary>
public class GotFocusEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.GotFocusEvent, OnGotFocus);
    }

    private void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        Execute(e);
    }
}
