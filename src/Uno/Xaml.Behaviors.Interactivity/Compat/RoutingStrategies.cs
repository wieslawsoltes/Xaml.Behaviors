// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactivity;

/// <summary>
/// Defines the routing strategies used when a behavior subscribes to a routed event.
/// </summary>
/// <remarks>
/// <para>
/// WinUI routed events always bubble. On Uno Platform <see cref="Tunnel"/> maps to the <c>Preview*</c>
/// counterpart of an event when one exists (for example <c>PreviewKeyDown</c>); otherwise (pointer events, or
/// <see cref="Tunnel"/> combined with <see cref="Bubble"/>) the handler also receives the events the controls
/// handled, after them. Like an Avalonia tunnel handler, which runs before the controls, it sees such an event as not
/// handled; the handled flag is restored when the handler returns, so the handler can only set it. The emulated
/// tunnel handlers run in bubbling order (from the source up), after the handlers of the controls.
/// </para>
/// <para>
/// <see cref="Direct"/> alone only reacts to events raised on the associated element itself: the handler runs
/// when the event's <c>OriginalSource</c> is the element, so events bubbling up from its descendants (including the
/// parts of its template) are ignored. <c>PointerEntered</c> and <c>PointerExited</c> are raised by Uno Platform on
/// every element the pointer enters or leaves and are not filtered; <c>PointerCaptureLost</c> is delivered when the
/// element itself loses a capture. Combined with <see cref="Bubble"/> or <see cref="Tunnel"/>, the events of the
/// descendants are delivered as well. Unlike Avalonia, a <see cref="Direct"/> subscription to an event that bubbles
/// or tunnels (for example <c>KeyDown</c>) still receives the events raised on the element itself.
/// </para>
/// </remarks>
[Flags]
public enum RoutingStrategies
{
    /// <summary>
    /// The event is raised only on the source element.
    /// </summary>
    Direct = 0x01,

    /// <summary>
    /// The event is raised on the source element's ancestors before the source (preview events).
    /// </summary>
    Tunnel = 0x02,

    /// <summary>
    /// The event bubbles from the source element to its ancestors.
    /// </summary>
    Bubble = 0x04,
}
