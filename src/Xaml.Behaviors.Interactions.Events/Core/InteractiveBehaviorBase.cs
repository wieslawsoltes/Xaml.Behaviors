using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Base class for behaviors that listen for routed events.
/// </summary>
public abstract class InteractiveBehaviorBase : StyledElementBehavior<Interactive>
{
    /// <summary>
    /// Identifies the <see cref="RoutingStrategies"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<RoutingStrategies> RoutingStrategiesProperty =
        AvaloniaProperty.Register<InteractiveBehaviorBase, RoutingStrategies>(
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
}
