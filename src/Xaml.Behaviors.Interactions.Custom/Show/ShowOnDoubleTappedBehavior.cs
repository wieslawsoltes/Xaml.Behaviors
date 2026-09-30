// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that allows to show control on double tapped event.
/// </summary>
public class ShowOnDoubleTappedBehavior : ShowBehaviorBase
{
    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        if (AssociatedObject is not { } element)
        {
            return DisposableAction.Empty;
        }

        element.AddHandler(InputElement.DoubleTappedEvent, AssociatedObject_DoubleTapped, EventRoutingStrategy);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(InputElement.DoubleTappedEvent, AssociatedObject_DoubleTapped));
#else
        var dispose = AssociatedObject?
            .AddDisposableHandler(
                InputElement.DoubleTappedEvent,
                AssociatedObject_DoubleTapped, 
                EventRoutingStrategy);

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
#endif
    }

    private void AssociatedObject_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        Show();
    }
}
