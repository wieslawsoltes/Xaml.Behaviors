// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Identifies a routed event together with the type of its event arguments (Uno Platform counterpart of the
/// Avalonia <c>RoutedEvent&lt;TEventArgs&gt;</c>).
/// </summary>
/// <remarks>
/// WinUI routed event identifiers are not generic. A <see cref="Microsoft.UI.Xaml.RoutedEvent"/> converts
/// implicitly, so members typed with this class accept the WinUI identifiers, for example
/// <c>UIElement.KeyDownEvent</c>.
/// </remarks>
/// <typeparam name="TEventArgs">The type of the event arguments raised by the event.</typeparam>
public sealed class RoutedEvent<TEventArgs>
    where TEventArgs : RoutedEventArgs
{
    private RoutedEvent(Microsoft.UI.Xaml.RoutedEvent? routedEvent, ClrRoutedEvent? clrEvent)
    {
        Event = routedEvent;
        ClrEvent = clrEvent;
    }

    /// <summary>
    /// Gets the WinUI routed event, or <c>null</c> for events WinUI only exposes as CLR events (focus events).
    /// </summary>
    public Microsoft.UI.Xaml.RoutedEvent? Event { get; }

    /// <summary>
    /// Gets the CLR event used when WinUI has no routed event identifier.
    /// </summary>
    internal ClrRoutedEvent? ClrEvent { get; }

    /// <summary>
    /// Converts a WinUI routed event identifier.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    public static implicit operator RoutedEvent<TEventArgs>(Microsoft.UI.Xaml.RoutedEvent routedEvent)
    {
        ArgumentNullException.ThrowIfNull(routedEvent);
        return new RoutedEvent<TEventArgs>(routedEvent, null);
    }

    /// <summary>
    /// Creates the identifier of an event WinUI only exposes as a CLR event.
    /// </summary>
    /// <param name="clrEvent">The CLR event.</param>
    /// <returns>The identifier.</returns>
    internal static RoutedEvent<TEventArgs> FromClrEvent(ClrRoutedEvent clrEvent) => new(null, clrEvent);

    /// <inheritdoc />
    public override string ToString() => ClrEvent?.Name ?? Event?.ToString() ?? string.Empty;
}

/// <summary>
/// Subscription helpers for <see cref="RoutedEvent{TEventArgs}"/>.
/// </summary>
internal static class RoutedEventOfTCompatExtensions
{
    /// <summary>
    /// Subscribes a typed handler and returns a disposable that removes it (Avalonia
    /// <c>AddDisposableHandler&lt;TEventArgs&gt;</c>).
    /// </summary>
    public static IDisposable AddDisposableHandler<TEventArgs>(this UIElement element, RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
        where TEventArgs : RoutedEventArgs
    {
        ArgumentNullException.ThrowIfNull(routedEvent);
        ArgumentNullException.ThrowIfNull(handler);

        EventHandler<RoutedEventArgs> wrapper = (sender, e) =>
        {
            if (e is TEventArgs args)
            {
                handler(sender, args);
            }
        };

        return routedEvent.ClrEvent is { } clrEvent
            ? element.AddDisposableHandler(clrEvent, wrapper, routes, handledEventsToo)
            : element.AddDisposableHandler(routedEvent.Event!, wrapper, routes, handledEventsToo);
    }
}
