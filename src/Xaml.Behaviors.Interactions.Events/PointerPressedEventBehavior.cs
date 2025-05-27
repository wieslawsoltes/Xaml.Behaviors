// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.PointerPressedEvent"/>.
/// </summary>
public abstract class PointerPressedEventBehavior : InteractiveBehaviorBase
{
    static PointerPressedEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerPressedEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, PointerPressed, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerPressedEvent, PointerPressed);
    }

    private void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        OnPointerPressed(sender, e);
    }

    /// <summary>
    /// Called when a pointer is pressed over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
    }
}
