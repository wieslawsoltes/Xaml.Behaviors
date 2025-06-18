﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.GotFocusEvent"/>.
/// </summary>
public abstract class GotFocusEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.GotFocusEvent, GotFocus, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.GotFocusEvent, GotFocus);
    }

    private void GotFocus(object? sender, GotFocusEventArgs e)
    {
        OnGotFocus(sender, e);
    }

    /// <summary>
    /// Called when the associated control receives focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
    }
}
