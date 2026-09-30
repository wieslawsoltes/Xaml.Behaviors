// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Events;
#else
namespace Avalonia.Xaml.Interactions.Events;
#endif

/// <summary>
/// Behavior that handles the <see cref="InputElement.ScrollGestureEvent"/>.
/// </summary>
public abstract class ScrollGestureEventBehavior : InteractiveBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.ScrollGestureEvent, ScrollGesture, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.ScrollGestureEvent, ScrollGesture);
    }

    private void ScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        OnScrollGesture(sender, e);
    }

    /// <summary>
    /// Called when a scroll gesture occurs on the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
    }
}
