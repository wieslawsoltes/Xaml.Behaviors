// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Xaml.Interactivity;

/// <summary>
/// Defines the routing strategies used when a behavior subscribes to a routed event.
/// </summary>
/// <remarks>
/// WinUI routed events always bubble. On Uno Platform <see cref="Tunnel"/> maps to the <c>Preview*</c>
/// counterpart of an event when one exists (for example <c>PreviewKeyDown</c>) and <see cref="Direct"/>
/// only reacts to events raised by the associated element itself.
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
