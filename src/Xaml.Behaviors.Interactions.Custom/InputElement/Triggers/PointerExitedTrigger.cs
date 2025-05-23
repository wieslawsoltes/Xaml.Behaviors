using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerExitedTrigger : RoutedEventTriggerBase<PointerEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerEventArgs> RoutedEvent 
        => InputElement.PointerExitedEvent;
}
