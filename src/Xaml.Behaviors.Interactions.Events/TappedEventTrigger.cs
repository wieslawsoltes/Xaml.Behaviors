// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="Gestures.TappedEvent"/>.
/// </summary>
public class TappedEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.TappedEvent, OnTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.TappedEvent, OnTapped);
    }

    private void OnTapped(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
