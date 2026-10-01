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
/// Behavior that handles the <see cref="InputElement.ScrollGestureEndedEvent"/>.
/// </summary>
public abstract class ScrollGestureEndedEventBehavior : InteractiveBehaviorBase
{
    private System.IDisposable? _subscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _subscription?.Dispose();
        _subscription = AssociatedObject?.AddDisposableRoutedEventHandler(InputElement.ScrollGestureEndedEvent, ScrollGestureEnded, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void ScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
    {
        OnScrollGestureEnded(sender, e);
    }

    /// <summary>
    /// Called when a scroll gesture ends on the associated control.
    /// </summary>
    /// <param name="sender">The element that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
    {
    }

    /// <summary>
    /// Never called: <see cref="InputElement.ScrollGestureEndedEvent"/> is raised with
    /// <see cref="ScrollGestureEndedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The element that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    [System.Obsolete("ScrollGestureEndedEvent is raised with ScrollGestureEndedEventArgs: override OnScrollGestureEnded(object?, ScrollGestureEndedEventArgs).")]
    protected virtual void OnScrollGestureEnded(object? sender, ScrollGestureEventArgs e)
    {
    }
}
