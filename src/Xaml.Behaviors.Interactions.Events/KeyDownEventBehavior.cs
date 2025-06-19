﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that listens for the <see cref="InputElement.KeyDownEvent"/>.
/// </summary>
public abstract class KeyDownEventBehavior : InteractiveBehaviorBase
{
    static KeyDownEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<KeyDownEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, KeyDown, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, KeyDown);
    }

    private void KeyDown(object? sender, KeyEventArgs e)
    {
        OnKeyDown(sender, e);
    }

    /// <summary>
    /// Called when a key is pressed while the associated control has focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnKeyDown(object? sender, KeyEventArgs e)
    {
    }
}
