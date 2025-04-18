﻿using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows to show control on double tapped event.
/// </summary>
public class ShowOnDoubleTappedBehavior : ShowBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var dispose = AssociatedObject?
            .AddDisposableHandler(
                Gestures.DoubleTappedEvent, 
                AssociatedObject_DoubleTapped, 
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
    }

    private void AssociatedObject_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        Show();
    }
}
