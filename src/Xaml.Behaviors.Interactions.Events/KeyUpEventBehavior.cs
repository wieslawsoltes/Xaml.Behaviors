// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that listens for the <see cref="InputElement.KeyUpEvent"/>.
/// </summary>
public abstract class KeyUpEventBehavior : InteractiveBehaviorBase
{
    static KeyUpEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<KeyUpEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.KeyUpEvent, KeyUp, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyUpEvent, KeyUp);
    }

    private void KeyUp(object? sender, KeyEventArgs e)
    {
        OnKeyUp(sender, e);
    }

    /// <summary>
    /// Called when a key is released while the associated control has focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnKeyUp(object? sender, KeyEventArgs e)
    {
    }
}
