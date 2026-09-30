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
public abstract class InteractiveBehaviorBase : StyledElementBehavior<Interactive>
{
    /// <summary>
    /// Identifies the <see cref="RoutingStrategies"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty RoutingStrategiesProperty =
#else
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
#endif
        AvaloniaProperty.Register<InteractiveBehaviorBase, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets the routing strategies used when subscribing to events.
    /// </summary>
    public RoutingStrategies RoutingStrategies
    {
        get => (RoutingStrategies)GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }
}
