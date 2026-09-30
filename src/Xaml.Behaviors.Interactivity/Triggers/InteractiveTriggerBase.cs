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
/// Base class for triggers that listen for routed events.
/// </summary>
public abstract partial class InteractiveTriggerBase : StyledElementTrigger<Interactive>
{

    /// <summary>
    /// Gets or sets the routing strategies used when subscribing to events.
    /// </summary>
    [StyledProperty(DefaultValue = RoutingStrategies.Bubble)]
    public partial RoutingStrategies RoutingStrategies { get; set; }

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
