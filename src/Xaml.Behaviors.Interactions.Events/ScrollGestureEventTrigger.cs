// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="Gestures.ScrollGestureEvent"/>.
/// </summary>
public class ScrollGestureEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.ScrollGestureEvent, OnScrollGesture, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.ScrollGestureEvent, OnScrollGesture);
    }

    private void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        Execute(e);
    }
}
