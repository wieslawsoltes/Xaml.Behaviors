// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DragEnterEvent"/>.
/// </summary>
public sealed class DragEnterEventTrigger : InteractiveTriggerBase
{
    static DragEnterEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<DragEnterEventTrigger>(new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Direct | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragEnterEvent, OnDragEnter, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        Execute(e);
    }
}
