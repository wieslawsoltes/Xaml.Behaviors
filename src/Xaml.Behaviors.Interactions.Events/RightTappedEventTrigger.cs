// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="Gestures.RightTappedEvent"/>.
/// </summary>
public class RightTappedEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(Gestures.RightTappedEvent, OnRightTapped, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(Gestures.RightTappedEvent, OnRightTapped);
    }

    private void OnRightTapped(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
