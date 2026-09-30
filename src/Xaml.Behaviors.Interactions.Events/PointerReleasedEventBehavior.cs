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
/// Behavior that handles the <see cref="InputElement.PointerReleasedEvent"/>.
/// </summary>
public abstract class PointerReleasedEventBehavior : InteractiveBehaviorBase
{
    static PointerReleasedEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerReleasedEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, PointerReleased, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, PointerReleased);
    }

    private void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        OnPointerReleased(sender, e);
    }

    /// <summary>
    /// Called when a pointer is released over the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
    }
}
