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
/// Trigger that listens for multiple pointer events.
/// </summary>
public class PointerEventsTrigger : InteractiveTriggerBase
{
    private System.IDisposable? _pressedSubscription;
    private System.IDisposable? _releasedSubscription;
    private System.IDisposable? _movedSubscription;

    static PointerEventsTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEventsTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        DisposeSubscriptions();

        if (AssociatedObject is not null)
        {
            _pressedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies);
            _releasedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies);
            _movedSubscription = AssociatedObject.AddDisposableRoutedEventHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies);
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

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Execute(e);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Execute(e);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Execute(e);
    }
}
