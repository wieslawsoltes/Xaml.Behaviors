// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Events;
#else
namespace Avalonia.Xaml.Interactions.Events;
#endif

/// <summary>
/// Behavior that listens for <see cref="InputElement.DoubleTappedEvent"/>.
/// </summary>
public abstract class DoubleTappedEventBehavior : InteractiveBehaviorBase
{
    private System.IDisposable? _subscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _subscription?.Dispose();
        _subscription = AssociatedObject?.AddDisposableRoutedEventHandler(InputElement.DoubleTappedEvent, DoubleTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void DoubleTapped(object? sender, RoutedEventArgs e)
    {
        OnDoubleTapped(sender, e);
    }

    /// <summary>
    /// Called when a double tap gesture is raised on the associated object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
    }
}
