// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Subscribes the behaviors and triggers whose <see cref="RoutingStrategies"/> are set by the user to a routed event,
/// so that the handler is invoked whatever the routing strategies of the event.
/// </summary>
/// <remarks>
/// <para>
/// Avalonia only invokes a handler when its routes include the route the event is raised on: a handler without
/// <see cref="RoutingStrategies.Direct"/> is never invoked for a direct event, and a handler with only
/// <see cref="RoutingStrategies.Direct"/> is never invoked for a tunneling or bubbling event, even on the element that
/// raises it. The adapter maps these subscriptions to routes the event is raised on:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="RoutingStrategies.Direct"/> without <see cref="RoutingStrategies.Tunnel"/> or
/// <see cref="RoutingStrategies.Bubble"/> on a tunneling or bubbling event subscribes on the bubble route (the tunnel
/// route for a tunnel-only event) and only forwards the events whose source is the element, that is the documented
/// meaning of <see cref="RoutingStrategies.Direct"/>: the events raised by the element itself.
/// </description></item>
/// <item><description>
/// <see cref="RoutingStrategies.Tunnel"/> or <see cref="RoutingStrategies.Bubble"/> without
/// <see cref="RoutingStrategies.Direct"/> on a direct event subscribes with <see cref="RoutingStrategies.Direct"/>.
/// </description></item>
/// <item><description>Any other subscription uses the routing strategies unchanged.</description></item>
/// </list>
/// <para>
/// The routing strategies of the event are read from the public <see cref="RoutedEvent.RoutingStrategies"/> (no
/// reflection). The Uno Platform counterpart (<c>src/Uno/Xaml.Behaviors.Interactivity/Compat/RoutingStrategiesAdapter.cs</c>)
/// subscribes unchanged: its compat layer already emulates every route over the bubbling WinUI events.
/// </para>
/// </remarks>
internal static class RoutingStrategiesAdapter
{
    private const RoutingStrategies Routed = RoutingStrategies.Tunnel | RoutingStrategies.Bubble;

    /// <summary>
    /// Gets the routing strategies to subscribe to a routed event with.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="sourceOnly">
    /// <see langword="true"/> when the handler must only receive the events whose source is the element it is added to
    /// (a <see cref="RoutingStrategies.Direct"/>-only subscription to a tunneling or bubbling event).
    /// </param>
    /// <returns>The routing strategies of the subscription.</returns>
    public static RoutingStrategies GetSubscriptionRoutes(
        RoutedEvent routedEvent,
        RoutingStrategies routes,
        out bool sourceOnly)
    {
        var eventRoutes = routedEvent.RoutingStrategies;
        var isDirectEvent = (eventRoutes & Routed) == 0;

        if (!isDirectEvent && routes == RoutingStrategies.Direct)
        {
            sourceOnly = true;
            return (eventRoutes & RoutingStrategies.Bubble) != 0
                ? RoutingStrategies.Bubble
                : RoutingStrategies.Tunnel;
        }

        sourceOnly = false;

        if (isDirectEvent && (routes & RoutingStrategies.Direct) == 0 && (routes & Routed) != 0)
        {
            return RoutingStrategies.Direct;
        }

        return routes;
    }

    /// <summary>
    /// Adds a handler for a routed event with the routing strategies requested by the user, adapted to the routing
    /// strategies of the event.
    /// </summary>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler<TEventArgs>(
        this Interactive element,
        RoutedEvent<TEventArgs> routedEvent,
        EventHandler<TEventArgs> handler,
        RoutingStrategies routes,
        bool handledEventsToo = false)
        where TEventArgs : RoutedEventArgs
    {
        var subscriptionRoutes = GetSubscriptionRoutes(routedEvent, routes, out var sourceOnly);
        EventHandler<TEventArgs> subscribed = sourceOnly
            ? (sender, e) =>
            {
                if (ReferenceEquals(e.Source, element))
                {
                    handler(sender, e);
                }
            }
            : handler;

        element.AddHandler(routedEvent, subscribed, subscriptionRoutes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveHandler(routedEvent, subscribed));
    }

    /// <summary>
    /// Adds a handler for an untyped routed event with the routing strategies requested by the user, adapted to the
    /// routing strategies of the event.
    /// </summary>
    /// <remarks>
    /// A distinct name from <see cref="AddDisposableRoutedEventHandler{TEventArgs}"/>: an overload would make the calls
    /// with a typed event and a <see cref="RoutedEventArgs"/> handler ambiguous.
    /// </remarks>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableUntypedRoutedEventHandler(
        this Interactive element,
        RoutedEvent routedEvent,
        EventHandler<RoutedEventArgs> handler,
        RoutingStrategies routes,
        bool handledEventsToo = false)
    {
        var subscriptionRoutes = GetSubscriptionRoutes(routedEvent, routes, out var sourceOnly);
        EventHandler<RoutedEventArgs> subscribed = sourceOnly
            ? (sender, e) =>
            {
                if (ReferenceEquals(e.Source, element))
                {
                    handler(sender, e);
                }
            }
            : handler;

        element.AddHandler(routedEvent, subscribed, subscriptionRoutes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveHandler(routedEvent, subscribed));
    }
}
