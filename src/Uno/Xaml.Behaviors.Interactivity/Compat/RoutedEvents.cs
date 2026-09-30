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
/// Avalonia tunnel handler observes before the controls handle the event.
/// </remarks>
internal static class RoutedEventCompatExtensions
{
    private static readonly ConditionalWeakTable<UIElement, List<Subscription>> s_subscriptions = new();

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<PointerRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new PointerEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<KeyRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new KeyEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<TappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new TappedEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DoubleTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new DoubleTappedEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RightTappedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new RightTappedEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<HoldingRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new HoldingEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<DragEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new DragEventHandler((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<CharacterReceivedRoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
        => Add(element, routedEvent, handler, new TypedEventHandler<UIElement, CharacterReceivedRoutedEventArgs>((s, e) => handler(s, e)), routes, handledEventsToo);

    public static void AddHandler(this UIElement element, RoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        // Avalonia raises gesture events with plain routed event arguments; WinUI needs the typed delegate.
        Delegate wrapper = routedEvent == UIElement.TappedEvent ? new TappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.DoubleTappedEvent ? new DoubleTappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.RightTappedEvent ? new RightTappedEventHandler((s, e) => handler(s, e))
            : routedEvent == UIElement.HoldingEvent ? new HoldingEventHandler((s, e) => handler(s, e))
            : new RoutedEventHandler((s, e) => handler(s, e));
        Add(element, routedEvent, handler, wrapper, routes, handledEventsToo);
    }

    public static void AddHandler(this UIElement element, ClrRoutedEvent routedEvent, EventHandler<RoutedEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Bubble, bool handledEventsToo = false)
    {
        _ = routes;
        _ = handledEventsToo;
        RoutedEventHandler wrapper = (s, e) => handler(s, e);
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

    private static void Add(UIElement element, RoutedEvent routedEvent, Delegate handler, Delegate wrapper, RoutingStrategies routes, bool handledEventsToo)
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

        element.AddHandler(actualEvent, wrapper, handledEventsToo);
        GetSubscriptions(element).Add(new Subscription(actualEvent, null, handler, wrapper) { Requested = routedEvent });
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
                subscriptions.RemoveAt(i);
                return;
            }
        }
    }

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
    }
}
