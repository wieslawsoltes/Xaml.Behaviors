// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Base class for triggers that listen for routed events.
/// </summary>
public abstract class InteractiveTriggerBase : StyledElementTrigger<Interactive>
{
    /// <summary>
    /// Identifies the <see cref="RoutingStrategies"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<InteractiveTriggerBase, RoutingStrategies>(
            nameof(RoutingStrategies),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets the routing strategies used when subscribing to events.
    /// </summary>
    public RoutingStrategies RoutingStrategies
    {
        get => GetValue(RoutingStrategiesProperty);
        set => SetValue(RoutingStrategiesProperty, value);
    }

    /// <summary>
    /// Executes the actions associated with this trigger.
    /// </summary>
    /// <param name="parameter">Event arguments passed to the actions.</param>
    protected void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
