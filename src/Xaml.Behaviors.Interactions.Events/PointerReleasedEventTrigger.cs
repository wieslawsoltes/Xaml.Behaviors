// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerReleasedEvent"/>.
/// </summary>
public class PointerReleasedEventTrigger : InteractiveTriggerBase
{
    static PointerReleasedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerReleasedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Execute(e);
    }
}
