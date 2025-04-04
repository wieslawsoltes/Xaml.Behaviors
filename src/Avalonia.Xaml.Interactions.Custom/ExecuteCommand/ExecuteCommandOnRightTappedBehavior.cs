using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ExecuteCommandOnRightTappedBehavior : ExecuteCommandRoutedEventBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var control = SourceControl ?? AssociatedObject;
        var dispose = control?
            .AddDisposableHandler(
                Gestures.RightTappedEvent,
                OnRightTapped,
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
    }

    private void OnRightTapped(object? sender, RoutedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (ExecuteCommand())
        {
            e.Handled = MarkAsHandled;
        }
    }
}
