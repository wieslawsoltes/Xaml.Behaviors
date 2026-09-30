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
    static PointerEventsTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerEventsTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, OnPointerMoved);
        }
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
