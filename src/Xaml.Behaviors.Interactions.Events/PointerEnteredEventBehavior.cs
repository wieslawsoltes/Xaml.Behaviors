﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.PointerEnteredEvent"/>.
/// </summary>
public abstract class PointerEnteredEventBehavior : InteractiveBehaviorBase
{
    static PointerEnteredEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEnteredEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerEnteredEvent, PointerEnter, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerEnteredEvent, PointerEnter);
    }

    private void PointerEnter(object? sender, PointerEventArgs e)
    {
        OnPointerEnter(sender, e);
    }

    /// <summary>
    /// Called when a pointer enters the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerEnter(object? sender, PointerEventArgs e)
    {
    }
}
