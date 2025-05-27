// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerPressedEvent"/>.
/// </summary>
public class PointerPressedEventTrigger : InteractiveTriggerBase
{
    static PointerPressedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerPressedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Execute(e);
    }
}
