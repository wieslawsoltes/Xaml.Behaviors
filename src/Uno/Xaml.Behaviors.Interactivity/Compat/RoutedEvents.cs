// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace Xaml.Interactivity;

/// <summary>
/// Identifies an Avalonia routed event that WinUI only exposes as a CLR event (no <see cref="RoutedEvent"/>).
/// </summary>
internal sealed class ClrRoutedEvent(string name)
{
    /// <summary>Gets the event name.</summary>
    public string Name { get; } = name;

    public static ClrRoutedEvent GotFocus { get; } = new("GotFocus");

    public static ClrRoutedEvent LostFocus { get; } = new("LostFocus");

    /// <summary>Converts the event to the typed identifier used by the shared sources (Avalonia <c>RoutedEvent&lt;T&gt;</c>).</summary>
    public static implicit operator RoutedEvent<RoutedEventArgs>(ClrRoutedEvent clrEvent)
        => RoutedEvent<RoutedEventArgs>.FromClrEvent(clrEvent);
}

/// <summary>
/// Avalonia routed event identifiers that have no identically named WinUI counterpart.
/// </summary>
internal static class UIElementRoutedEventCompat
{
    extension(UIElement)
    {
        /// <summary>Gets the event raised when the element receives focus.</summary>
        public static ClrRoutedEvent GotFocusEvent => ClrRoutedEvent.GotFocus;

        /// <summary>Gets the event raised when the element loses focus.</summary>
        public static ClrRoutedEvent LostFocusEvent => ClrRoutedEvent.LostFocus;

        /// <summary>Gets the text input event (<c>CharacterReceived</c> on WinUI).</summary>
        public static RoutedEvent TextInputEvent => UIElement.CharacterReceivedEvent;
    }
}

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>AddHandler</c> overloads and of <c>RemoveRoutedEventHandler</c>
/// used by the shared sources.
/// </summary>
/// <remarks>
/// Removal deliberately uses a distinct name: a method group passed to WinUI's instance
/// <c>UIElement.RemoveHandler(RoutedEvent, object)</c> binds to it (natural delegate type) and removes nothing.
/// </remarks>
/// <remarks>
/// WinUI routed events always bubble. <see cref="RoutingStrategies.Tunnel"/> subscribes to the <c>Preview*</c>
/// counterpart when WinUI has one (key events) and otherwise also receives handled events, which is what an
/// Avalonia tunnel handler observes before the controls handle the event. A <see cref="RoutingStrategies.Direct"/>
/// subscription without <see cref="RoutingStrategies.Bubble"/> and <see cref="RoutingStrategies.Tunnel"/> only
/// receives the events raised on the element itself (<see cref="DirectRouteFilter"/>).
/// </remarks>
internal static class RoutedEventCompatExtensions
{
    private static readonly ConditionalWeakTable<UIElement, List<Subscription>> s_subscriptions = new();

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<PointerRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new PointerEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<KeyRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new KeyEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<TappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new TappedEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DoubleTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new DoubleTappedEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RightTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new RightTappedEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<HoldingRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new HoldingEventHandler((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DragEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, h => WrapDragHandler(routedEvent, (s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<CharacterReceivedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, static h => new TypedEventHandler<UIElement, CharacterReceivedRoutedEventArgs>((s, e) => h(s, e)));

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, routes, handledEventsToo, h => CreateWrapper(routedEvent, h));

    public static void AddHandler(this UIElement element, ClrRoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        // WinUI focus events bubble but can be neither handled nor tunneled: only the Direct source filter applies.
        _ = handledEventsToo;
        var filter = IsDirectOnly(routes) ? DirectRouteFilter.Create(element, null) : null;
        RoutedEventHandler wrapper = filter is null
            ? (s, e) => handler(s, e)
            : (s, e) =>
            {
                if (filter.Accepts(e))
                {
                    handler(s, e);
                }
            };

        switch (routedEvent.Name)
        {
            case "GotFocus":
                element.GotFocus += wrapper;
                break;
            case "LostFocus":
                element.LostFocus += wrapper;
                break;
        }

        GetSubscriptions(element).Add(new Subscription(null, routedEvent, handler, wrapper));
    }

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<PointerRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<KeyRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<TappedRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DoubleTappedRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RightTappedRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<HoldingRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DragEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<CharacterReceivedRoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler) => Remove(element, routedEvent, handler);

    public static void RemoveRoutedEventHandler(this UIElement element, ClrRoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler)
    {
        var subscriptions = GetSubscriptions(element);
        for (var i = subscriptions.Count - 1; i >= 0; i--)
        {
            var subscription = subscriptions[i];
            if (ReferenceEquals(subscription.ClrEvent, routedEvent) && Equals(subscription.Handler, handler))
            {
                var wrapper = (RoutedEventHandler)subscription.Wrapper;
                if (routedEvent.Name == "GotFocus")
                {
                    element.GotFocus -= wrapper;
                }
                else
                {
                    element.LostFocus -= wrapper;
                }

                subscriptions.RemoveAt(i);
                return;
            }
        }
    }

    private static void Add<TEventArgs>(UIElement element, RoutedEvent routedEvent, EventHandler<TEventArgs> handler, RoutingStrategies routes, bool handledEventsToo, Func<EventHandler<TEventArgs>, Delegate> createWrapper)
        where TEventArgs : RoutedEventArgs
    {
        var tunnel = (routes & RoutingStrategies.Tunnel) != 0;
        var bubble = (routes & (RoutingStrategies.Bubble | RoutingStrategies.Direct)) != 0;
        var actualEvent = routedEvent;

        if (tunnel && !bubble && GetPreviewEvent(routedEvent) is { } previewEvent)
        {
            actualEvent = previewEvent;
        }
        else if (tunnel)
        {
            handledEventsToo = true;
        }

        var filter = IsDirectOnly(routes) ? DirectRouteFilter.Create(element, routedEvent) : null;
        var routedHandler = filter is null
            ? handler
            : (s, e) =>
            {
                if (filter.Accepts(e))
                {
                    handler(s, e);
                }
            };

        var wrapper = createWrapper(routedHandler);
        element.AddHandler(actualEvent, wrapper, handledEventsToo);
        GetSubscriptions(element).Add(new Subscription(actualEvent, null, handler, wrapper) { Requested = routedEvent, Filter = filter });
    }

    private static void Remove(UIElement element, RoutedEvent routedEvent, Delegate handler)
    {
        var subscriptions = GetSubscriptions(element);
        for (var i = subscriptions.Count - 1; i >= 0; i--)
        {
            var subscription = subscriptions[i];
            if (subscription.Requested == routedEvent && Equals(subscription.Handler, handler))
            {
                element.RemoveHandler(subscription.RoutedEvent!, subscription.Wrapper);
                subscription.Filter?.Detach();
                subscriptions.RemoveAt(i);
                return;
            }
        }
    }

    // Direct without Bubble or Tunnel: the handler only receives the events raised on the element itself. Combined
    // with Bubble (the Avalonia default) or Tunnel, the events of the descendants are delivered as well.
    private static bool IsDirectOnly(RoutingStrategies routes)
        => (routes & RoutingStrategies.Direct) != 0
           && (routes & (RoutingStrategies.Bubble | RoutingStrategies.Tunnel)) == 0;

    // Avalonia handlers may take plain routed event arguments; WinUI only invokes the typed delegate of the event.
    private static Delegate CreateWrapper(RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler)
        => routedEvent == UIElement.TappedEvent ? new TappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.DoubleTappedEvent ? new DoubleTappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.RightTappedEvent ? new RightTappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.HoldingEvent ? new HoldingEventHandler((s, e) => handler(s, e))
            : IsDragEvent(routedEvent) ? WrapDragHandler(routedEvent, (s, e) => handler(s, e))
            : IsKeyEvent(routedEvent) ? new KeyEventHandler((s, e) => handler(s, e))
            : IsPointerEvent(routedEvent) ? new PointerEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.CharacterReceivedEvent ? new TypedEventHandler<UIElement, CharacterReceivedRoutedEventArgs>((s, e) => handler(s, e))
            : new RoutedEventHandler((s, e) => handler(s, e));

    // Drag enter/over handlers observe the Avalonia default effects (accept what the source allows).
    private static DragEventHandler WrapDragHandler(RoutedEvent routedEvent, DragEventHandler handler)
    {
        if (routedEvent != UIElement.DragEnterEvent && routedEvent != UIElement.DragOverEvent)
        {
            return handler;
        }

        return (s, e) =>
        {
            DragEventArgsCompatExtensions.ApplyDefaultDragEffects(e);
            handler(s, e);
        };
    }

    private static bool IsKeyEvent(RoutedEvent routedEvent)
        => routedEvent == UIElement.KeyDownEvent || routedEvent == UIElement.KeyUpEvent
            || routedEvent == UIElement.PreviewKeyDownEvent || routedEvent == UIElement.PreviewKeyUpEvent;

    private static bool IsPointerEvent(RoutedEvent routedEvent)
        => routedEvent == UIElement.PointerPressedEvent || routedEvent == UIElement.PointerReleasedEvent
            || routedEvent == UIElement.PointerMovedEvent || routedEvent == UIElement.PointerEnteredEvent
            || routedEvent == UIElement.PointerExitedEvent || routedEvent == UIElement.PointerCanceledEvent
            || routedEvent == UIElement.PointerCaptureLostEvent || routedEvent == UIElement.PointerWheelChangedEvent;

    // Avalonia DragLeave handlers may take plain RoutedEventArgs; WinUI needs the DragEventHandler delegate type.
    private static bool IsDragEvent(RoutedEvent routedEvent)
        => routedEvent == UIElement.DragEnterEvent
           || routedEvent == UIElement.DragOverEvent
           || routedEvent == UIElement.DragLeaveEvent
           || routedEvent == UIElement.DropEvent;

    private static RoutedEvent? GetPreviewEvent(RoutedEvent routedEvent)
    {
        if (routedEvent == UIElement.KeyDownEvent)
        {
            return UIElement.PreviewKeyDownEvent;
        }

        return routedEvent == UIElement.KeyUpEvent ? UIElement.PreviewKeyUpEvent : null;
    }

    private static List<Subscription> GetSubscriptions(UIElement element) => s_subscriptions.GetOrCreateValue(element);

    private sealed record Subscription(RoutedEvent? RoutedEvent, ClrRoutedEvent? ClrEvent, Delegate Handler, Delegate Wrapper)
    {
        public RoutedEvent? Requested { get; init; }

        public DirectRouteFilter? Filter { get; init; }
    }
}
