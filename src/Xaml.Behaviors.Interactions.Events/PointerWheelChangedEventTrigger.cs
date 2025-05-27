// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerWheelChangedEvent"/>.
/// </summary>
public class PointerWheelChangedEventTrigger : InteractiveTriggerBase
{
    static PointerWheelChangedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerWheelChangedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        Execute(e);
    }
}
