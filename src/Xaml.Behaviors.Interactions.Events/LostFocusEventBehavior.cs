// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.LostFocusEvent"/>.
/// </summary>
public abstract class LostFocusEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.LostFocusEvent, LostFocus, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.LostFocusEvent, LostFocus);
    }

    private void LostFocus(object? sender, RoutedEventArgs e)
    {
        OnLostFocus(sender, e);
    }

    /// <summary>
    /// Called when the associated control loses focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnLostFocus(object? sender, RoutedEventArgs e)
    {
    }
}
