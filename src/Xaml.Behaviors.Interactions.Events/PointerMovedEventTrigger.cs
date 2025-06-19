// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerMovedEvent"/>.
/// </summary>
public class PointerMovedEventTrigger : InteractiveTriggerBase
{
    static PointerMovedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerMovedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Execute(e);
    }
}
