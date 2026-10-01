// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
///
/// </summary>
public abstract partial class RoutedEventTriggerBase : AttachedToVisualTreeTriggerBase<Visual>
{

    /// <summary>
    /// Gets or sets the routing strategies used to subscribe to the routed event.
    /// </summary>
    /// <remarks>
    /// <see cref="RoutingStrategies.Direct"/> alone (the default) only handles the events raised by the associated
    /// element itself, not the ones raised by its descendants, whatever the routing strategies of the event.
    /// Include <see cref="RoutingStrategies.Bubble"/> or <see cref="RoutingStrategies.Tunnel"/> to also handle the
    /// events raised by the descendants.
    /// </remarks>
    [StyledProperty(DefaultValue = RoutingStrategies.Direct)]
    public partial RoutingStrategies EventRoutingStrategy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the trigger marks the event as handled when it executes its actions.
    /// When <see langword="false"/> (the default), the handled flag of the event is left unchanged.
    /// </summary>
    public bool MarkAsHandled { get; set; }

#if !UNO
    /// <summary>
    /// Gets the routes of the subscription that emulates a <see cref="RoutingStrategies.Direct"/>-only subscription
    /// to a tunneling or bubbling event.
    /// </summary>
    /// <remarks>
    /// Avalonia only invokes the handlers subscribed with <see cref="RoutingStrategies.Direct"/> for the events
    /// registered as direct events: the route of a tunneling or bubbling event only invokes the handlers subscribed
    /// with <see cref="RoutingStrategies.Tunnel"/> or <see cref="RoutingStrategies.Bubble"/>, even on the element that
    /// raises it. Such a subscription is made on the bubble route (or the tunnel route of a tunnel-only event) and
    /// filtered by the source of the event, so the handler receives the events raised by the element itself, as on
    /// Uno Platform.
    /// </remarks>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="routes">The routing strategies of the subscription.</param>
    /// <param name="emulatedRoutes">The routes to subscribe with when the method returns <see langword="true"/>.</param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="routes"/> is <see cref="RoutingStrategies.Direct"/> without
    /// <see cref="RoutingStrategies.Tunnel"/> or <see cref="RoutingStrategies.Bubble"/> and the event is not a direct
    /// event; otherwise <see langword="false"/> and the subscription uses <paramref name="routes"/> unchanged.
    /// </returns>
    internal static bool TryGetEmulatedDirectRoutes(
        RoutedEvent routedEvent,
        RoutingStrategies routes,
        out RoutingStrategies emulatedRoutes)
    {
        const RoutingStrategies routed = RoutingStrategies.Tunnel | RoutingStrategies.Bubble;
        var eventRoutes = routedEvent.RoutingStrategies;

        if ((routes & RoutingStrategies.Direct) == 0
            || (routes & routed) != 0
            || eventRoutes == RoutingStrategies.Direct
            || (eventRoutes & routed) == 0)
        {
            emulatedRoutes = routes;
            return false;
        }

        emulatedRoutes = (eventRoutes & RoutingStrategies.Bubble) != 0
            ? RoutingStrategies.Bubble
            : RoutingStrategies.Tunnel;
        return true;
    }
#endif
}
