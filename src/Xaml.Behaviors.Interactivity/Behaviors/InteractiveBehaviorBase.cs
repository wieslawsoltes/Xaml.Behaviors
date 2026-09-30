// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Base class for behaviors that listen for routed events.
/// </summary>
public abstract partial class InteractiveBehaviorBase : StyledElementBehavior<Interactive>
{

    /// <summary>
    /// Gets or sets the routing strategies used when subscribing to events.
    /// </summary>
    [StyledProperty(DefaultValue = RoutingStrategies.Bubble)]
    public partial RoutingStrategies RoutingStrategies { get; set; }
}
