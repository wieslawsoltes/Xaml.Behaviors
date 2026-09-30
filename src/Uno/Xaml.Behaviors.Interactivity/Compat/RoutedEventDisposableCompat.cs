// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>InteractiveExtensions.AddDisposableHandler</c> overloads used by the
/// shared sources.
/// </summary>
/// <remarks>
/// Each overload subscribes through <see cref="RoutedEventCompatExtensions"/> (same routing strategy mapping) and
/// returns a disposable that removes the handler with <c>RemoveRoutedEventHandler</c>.
/// </remarks>
internal static class RoutedEventDisposableCompatExtensions
{
    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<PointerRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<KeyRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<TappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DoubleTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RightTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<HoldingRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DragEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<CharacterReceivedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }

    public static IDisposable AddDisposableHandler(this UIElement element, ClrRoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        element.AddHandler(routedEvent, handler, routes, handledEventsToo);
        return DisposableAction.Create(() => element.RemoveRoutedEventHandler(routedEvent, handler));
    }
}
