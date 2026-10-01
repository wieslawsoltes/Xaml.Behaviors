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
public abstract class RoutedEventTrigger : RoutedEventTriggerBase
{
    /// <summary>
    /// 
    /// </summary>
    protected abstract RoutedEvent RoutedEvent { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A disposable resource to be disposed when the behavior is detached.</returns>
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is Interactive interactive)
        {
            var routedEvent = RoutedEvent;
            var routes = EventRoutingStrategy;
#if !UNO
            if (TryGetEmulatedDirectRoutes(routedEvent, routes, out var emulatedRoutes))
            {
                return AddDisposableHandler(interactive, routedEvent, DirectHandler, emulatedRoutes);
            }
#endif
            // The Uno Platform compat layer filters a Direct-only subscription by the original source itself.
            var disposable = AddDisposableHandler(
                interactive,
                routedEvent,
                Handler,
                routes);
            return disposable;
        }

        return DisposableAction.Empty;
    }

#if !UNO
    /// <summary>
    /// Forwards to <see cref="Handler"/> the events raised by the element the handler is subscribed to, which
    /// emulates a <see cref="RoutingStrategies.Direct"/>-only subscription to a tunneling or bubbling event.
    /// </summary>
    /// <param name="sender">The element the handler is subscribed to.</param>
    /// <param name="e">The event arguments.</param>
    private void DirectHandler(object? sender, RoutedEventArgs e)
    {
        if (ReferenceEquals(e.Source, sender))
        {
            Handler(sender, e);
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Handler(object? sender, RoutedEventArgs e)
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
    protected void Execute(RoutedEventArgs e)
    {
        e.Handled = MarkAsHandled;
        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

    private static IDisposable AddDisposableHandler(
        Interactive o, 
        RoutedEvent routedEvent,
        EventHandler<RoutedEventArgs> handler,
        RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble,
        bool handledEventsToo = false)
    {
        o.AddHandler(routedEvent, handler, routes, handledEventsToo);

        return DisposableAction.Create(() => o.RemoveRoutedEventHandler(routedEvent, handler));
    }
}
