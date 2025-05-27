// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.PointerWheelChangedEvent"/>.
/// </summary>
public abstract class PointerWheelChangedEventBehavior : InteractiveBehaviorBase
{
    static PointerWheelChangedEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerWheelChangedEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerWheelChangedEvent, PointerWheelChanged, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerWheelChangedEvent, PointerWheelChanged);
    }

    private void PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        OnPointerWheelChanged(sender, e);
    }

    /// <summary>
    /// Called when the mouse wheel changes while over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
    }
}
