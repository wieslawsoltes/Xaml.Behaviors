// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Removes routed event handlers through a platform neutral entry point.
/// </summary>
/// <remarks>
/// Shared behaviors call <c>RemoveRoutedEventHandler</c> instead of <c>RemoveHandler</c>: on Uno Platform (WinUI)
/// a method group passed to <c>UIElement.RemoveHandler(RoutedEvent, object)</c> binds to that instance method
/// through its natural delegate type and silently fails to remove the handler. The Uno Platform counterpart lives in
/// <c>src/Uno/Xaml.Behaviors.Interactivity/Compat/RoutedEvents.cs</c>.
/// </remarks>
internal static class RoutedEventHandlerExtensions
{
    /// <summary>
    /// Removes a routed event handler.
    /// </summary>
    /// <typeparam name="TEventArgs">The event arguments type.</typeparam>
    /// <param name="element">The element.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler to remove.</param>
    public static void RemoveRoutedEventHandler<TEventArgs>(this Interactive element, RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler)
        where TEventArgs : RoutedEventArgs
        => element.RemoveHandler(routedEvent, handler);

    /// <summary>
    /// Removes a routed event handler whose arguments type differs from the event arguments type.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="handler">The handler to remove.</param>
    public static void RemoveRoutedEventHandler(this Interactive element, RoutedEvent routedEvent, Delegate handler)
        => element.RemoveHandler(routedEvent, handler);
}
