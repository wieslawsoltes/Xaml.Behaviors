﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that listens for multiple pointer events.
/// </summary>
public abstract class PointerEventsBehavior : InteractiveBehaviorBase
{
    static PointerEventsBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEventsBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, PointerPressed, RoutingStrategies);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, PointerReleased, RoutingStrategies);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, PointerMoved, RoutingStrategies);
        } 
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, PointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, PointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, PointerMoved);
        }
    }

    private void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        OnPointerPressed(sender, e);
    }

    private void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        OnPointerReleased(sender, e);
    }

    private void PointerMoved(object? sender, PointerEventArgs e)
    {
        OnPointerMoved(sender, e);
    }

    /// <summary>
    /// Called when a pointer is pressed over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
    }

    /// <summary>
    /// Called when a pointer is released over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void  OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
    }

    /// <summary>
    /// Called when a pointer moves over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerMoved(object? sender, PointerEventArgs e)
    {
    }
}
