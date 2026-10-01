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
}
