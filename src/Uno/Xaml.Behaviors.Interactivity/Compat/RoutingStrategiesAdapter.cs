// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia routing adapter
/// (<c>src/Xaml.Behaviors.Interactivity/Events/RoutingStrategiesAdapter.cs</c>) used by the shared behaviors and
/// triggers whose <see cref="RoutingStrategies"/> are set by the user.
/// </summary>
/// <remarks>
/// A pass-through: every overload subscribes with <c>AddDisposableHandler</c> (<see cref="RoutedEventDisposableCompatExtensions"/>
/// and <see cref="RoutedEventOfTCompatExtensions"/>) with the requested routing strategies unchanged. The compat layer
/// already emulates every route over the bubbling WinUI events, including <see cref="RoutingStrategies.Direct"/>
/// (<see cref="DirectRouteFilter"/>), so the handlers are invoked whatever the routing strategies of the event.
/// </remarks>
internal static class RoutingStrategiesAdapter
{
    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<PointerRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<KeyRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<TappedRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DoubleTappedRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RightTappedRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<HoldingRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DragEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<CharacterReceivedRoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>
    /// Adds a handler for an untyped routed event (Avalonia <c>RoutedEvent</c>) with the requested routing strategies
    /// and returns a disposable that removes it.
    /// </summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableUntypedRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a handler of a focus event with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The focus event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler(this UIElement element, ClrRoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);

    /// <summary>Adds a typed handler with the requested routing strategies and returns a disposable that removes it.</summary>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="element">The element to add the handler to.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="routes">The routing strategies requested by the user.</param>
    /// <param name="handledEventsToo">Whether the handler also receives the events already marked as handled.</param>
    /// <returns>A disposable that removes the handler.</returns>
    public static IDisposable AddDisposableRoutedEventHandler<TEventArgs>(this UIElement element, RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler, RoutingStrategies routes, bool handledEventsToo = false)
        where TEventArgs : RoutedEventArgs
        => element.AddDisposableHandler(routedEvent, handler, routes, handledEventsToo);
}
