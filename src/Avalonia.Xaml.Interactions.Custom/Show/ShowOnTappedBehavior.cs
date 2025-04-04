using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows to show control on tapped event.
/// </summary>
public class ShowOnTappedBehavior : ShowBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
