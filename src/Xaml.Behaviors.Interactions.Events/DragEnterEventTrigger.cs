// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Events;
#else
namespace Avalonia.Xaml.Interactions.Events;
#endif

/// <summary>
/// Trigger that listens for the <see cref="DragDrop.DragEnterEvent"/>.
/// </summary>
public sealed class DragEnterEventTrigger : InteractiveTriggerBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragEnterEvent, OnDragEnter, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(DragDrop.DragEnterEvent, OnDragEnter);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        Execute(e);
    }
}
