// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
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
/// Trigger that listens for the <see cref="DragDrop.DragLeaveEvent"/>.
/// </summary>
/// <remarks>
/// The trigger also receives handled events, so it fires when a drop handler on the same element handles the drag.
/// </remarks>
public sealed class DragLeaveEventTrigger : InteractiveTriggerBase
{
    private System.IDisposable? _subscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _subscription?.Dispose();
        _subscription = AssociatedObject?.AddDisposableRoutedEventHandler(DragDrop.DragLeaveEvent, OnDragLeave, RoutingStrategies, handledEventsToo: true);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnDragLeave(object? sender, RoutedEventArgs e)
    {
        Execute(e);
    }
}
