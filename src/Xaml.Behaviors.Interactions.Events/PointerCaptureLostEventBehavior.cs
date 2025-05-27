// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that listens for the <see cref="InputElement.PointerCaptureLostEvent"/>.
/// </summary>
public abstract class PointerCaptureLostEventBehavior : InteractiveBehaviorBase
{
    static PointerCaptureLostEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerCaptureLostEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }
    
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerCaptureLostEvent, PointerCaptureLost, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerCaptureLostEvent, PointerCaptureLost);
    }

    private void PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        OnPointerCaptureLost(sender, e);
    }

    /// <summary>
    /// Called when pointer capture is lost on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
    }
}
