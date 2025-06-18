// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DropEvent"/>.
/// </summary>
public sealed class DropEventTrigger : InteractiveTriggerBase
{
    static DropEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<DropEventTrigger>(new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Direct | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DropEvent, OnDrop, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        Execute(e);
    }
}
