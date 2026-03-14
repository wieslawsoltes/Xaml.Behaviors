// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for <see cref="InputElement.DoubleTappedEvent"/>.
/// </summary>
public class DoubleTappedEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.DoubleTappedEvent, OnDoubleTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.DoubleTappedEvent, OnDoubleTapped);
    }

    private void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
