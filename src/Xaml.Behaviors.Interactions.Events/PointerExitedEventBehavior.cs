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
/// Behavior that handles the <see cref="InputElement.PointerExitedEvent"/>.
/// </summary>
public abstract class PointerExitedEventBehavior : InteractiveBehaviorBase
{
    static PointerExitedEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerExitedEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }


    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerExitedEvent, PointerLeave, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerExitedEvent, PointerLeave);
    }

    private void PointerLeave(object? sender, PointerEventArgs e)
    {
        OnPointerLeave(sender, e);
    }

    /// <summary>
    /// Called when a pointer leaves the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPointerLeave(object? sender, PointerEventArgs e)
    {
    }
}
