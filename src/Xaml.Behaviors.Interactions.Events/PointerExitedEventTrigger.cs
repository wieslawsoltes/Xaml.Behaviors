// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerExitedEvent"/>.
/// </summary>
public class PointerExitedEventTrigger : InteractiveTriggerBase
{
    static PointerExitedEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerExitedEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerExitedEvent, OnPointerExit, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerExitedEvent, OnPointerExit);
    }

    private void OnPointerExit(object? sender, PointerEventArgs e)
    {
        Execute(e);
    }
}
