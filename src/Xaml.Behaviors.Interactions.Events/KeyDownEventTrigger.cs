// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.KeyDownEvent"/>.
/// </summary>
public class KeyDownEventTrigger : InteractiveTriggerBase
{
    static KeyDownEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<KeyDownEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        Execute(e);
    }
}
