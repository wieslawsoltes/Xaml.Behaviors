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
public abstract partial class ExecuteCommandRoutedEventBehaviorBase : ExecuteCommandBehaviorBase
{

    /// <summary>
    /// Gets or sets the routing strategies used to subscribe to the routed event. This is an avalonia property.
    /// </summary>
    /// <remarks>
    /// The handler is invoked whatever the routing strategies of the event: <see cref="RoutingStrategies.Bubble"/> (the
    /// default) or <see cref="RoutingStrategies.Tunnel"/> also handle a direct event (for example
    /// <c>PointerEntered</c>), and <see cref="RoutingStrategies.Direct"/> alone only handles the events raised by the
    /// element itself, not the ones raised by its descendants.
    /// </remarks>
    [StyledProperty(DefaultValue = RoutingStrategies.Bubble)]
    public partial RoutingStrategies EventRoutingStrategy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool MarkAsHandled { get; set; } = true;
}
