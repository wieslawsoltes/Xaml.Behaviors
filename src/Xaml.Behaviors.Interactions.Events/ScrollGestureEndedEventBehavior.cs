// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="Gestures.ScrollGestureEndedEvent"/>.
/// </summary>
public abstract class ScrollGestureEndedEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollGestureEnded, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.ScrollGestureEndedEvent, ScrollGestureEnded);
    }

    private void ScrollGestureEnded(object? sender, ScrollGestureEventArgs e)
    {
        OnScrollGestureEnded(sender, e);
    }

    /// <summary>
    /// Called when a scroll gesture ends on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnScrollGestureEnded(object? sender, ScrollGestureEventArgs e)
    {
    }
}
