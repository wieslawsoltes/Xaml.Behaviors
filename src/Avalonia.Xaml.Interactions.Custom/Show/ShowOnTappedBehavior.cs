using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows to show control on tapped event.
/// </summary>
public class ShowOnTappedBehavior : ShowBehaviorBase
{
    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var dispose = AssociatedObject?
            .AddDisposableHandler(
                Gestures.TappedEvent, 
                AssociatedObject_Tapped, 
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
    }

    private void AssociatedObject_Tapped(object? sender, RoutedEventArgs e)
    {
        Show();
    }
}
