// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RoutedEventTriggerBase<T> : RoutedEventTriggerBase where T : RoutedEventArgs
{
    /// <summary>
    /// 
    /// </summary>
    protected abstract RoutedEvent<T> RoutedEvent { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
            // The routing adapter subscribes on the routes the event is raised on, so a Direct-only subscription to a
            // tunneling or bubbling event handles the events raised by the element itself.
            return interactive.AddDisposableRoutedEventHandler(RoutedEvent, Handler, EventRoutingStrategy);
        }

        return DisposableAction.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Handler(object? sender, T e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Execute(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void Execute(T e)
    {
        // Only sets the flag: an event handled by another handler (received with handledEventsToo, as the emulated
        // tunnel route of Uno Platform does) must stay handled.
        if (MarkAsHandled)
        {
            e.Handled = true;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

}
