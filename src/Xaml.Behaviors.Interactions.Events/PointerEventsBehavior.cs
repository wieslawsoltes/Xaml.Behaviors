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
/// Behavior that listens for multiple pointer events.
/// </summary>
public abstract class PointerEventsBehavior : InteractiveBehaviorBase
{
    private System.IDisposable? _pressedSubscription;
    private System.IDisposable? _releasedSubscription;
    private System.IDisposable? _movedSubscription;

    static PointerEventsBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEventsBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        DisposeSubscriptions();

        if (AssociatedObject is not null)
        {
            _pressedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerPressedEvent, PointerPressed, RoutingStrategies);
            _releasedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerReleasedEvent, PointerReleased, RoutingStrategies);
            _movedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerMovedEvent, PointerMoved, RoutingStrategies);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DisposeSubscriptions();
    }

    private void DisposeSubscriptions()
    {
        _pressedSubscription?.Dispose();
        _pressedSubscription = null;
        _releasedSubscription?.Dispose();
        _releasedSubscription = null;
        _movedSubscription?.Dispose();
        _movedSubscription = null;
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
