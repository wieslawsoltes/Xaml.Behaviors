// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DragOverEvent"/>.
/// </summary>
public sealed class DragOverEventTrigger : InteractiveTriggerBase
{
    static DragOverEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<DragOverEventTrigger>(new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Direct | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        Execute(e);
    }
}
