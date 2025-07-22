// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DragLeaveEvent"/>.
/// </summary>
public sealed class DragLeaveEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave, RoutingStrategies, true);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
    }

    private void OnDragLeave(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
