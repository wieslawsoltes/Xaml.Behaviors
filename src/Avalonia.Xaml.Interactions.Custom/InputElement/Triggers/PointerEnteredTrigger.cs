using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerEnteredTrigger : RoutedEventTriggerBase<PointerEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerEventArgs> RoutedEvent 
        => InputElement.PointerEnteredEvent;
}
