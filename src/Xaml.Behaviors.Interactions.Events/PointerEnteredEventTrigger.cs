// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerEnteredEvent"/>.
/// </summary>
public class PointerEnteredEventTrigger : InteractiveTriggerBase
{
    static PointerEnteredEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEnteredEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerEnteredEvent, OnPointerEnter, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerEnteredEvent, OnPointerEnter);
    }

    private void OnPointerEnter(object? sender, PointerEventArgs e)
    {
        Execute(e);
    }
}
