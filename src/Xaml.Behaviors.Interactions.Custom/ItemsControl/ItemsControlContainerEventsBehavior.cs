// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base class that exposes container related events from an <see cref="ItemsControl"/>.
/// </summary>
public abstract class ItemsControlContainerEventsBehavior : DisposingBehavior<ItemsControl>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not { } itemsControl)
        {
            return DisposableAction.Empty;
        }

        itemsControl.PreparingContainer += ItemsControlOnPreparingContainer;
        itemsControl.ContainerPrepared += ItemsControlOnContainerPrepared;
        itemsControl.ContainerIndexChanged += ItemsControlOnContainerIndexChanged;
        itemsControl.ContainerClearing += ItemsControlOnContainerClearing;

        return DisposableAction.Create(() =>
        {
            itemsControl.PreparingContainer += ItemsControlOnPreparingContainer;
            itemsControl.ContainerPrepared -= ItemsControlOnContainerPrepared;
            itemsControl.ContainerIndexChanged -= ItemsControlOnContainerIndexChanged;
            itemsControl.ContainerClearing -= ItemsControlOnContainerClearing;
        });
    }

    private void ItemsControlOnPreparingContainer(object? sender, ContainerPreparedEventArgs e)
    {
        OnPreparingContainer(sender, e);
    }

    private void ItemsControlOnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        OnContainerPrepared(sender, e);
    }
    
    private void ItemsControlOnContainerIndexChanged(object? sender, ContainerIndexChangedEventArgs e)
    {
        OnContainerIndexChanged(sender, e);
    }

    private void ItemsControlOnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        OnContainerClearing(sender, e);
    }

    /// <summary>
    /// Called before the container is prepared.
    /// </summary>
    /// <param name="sender">The items control raising the event.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnPreparingContainer(object? sender, ContainerPreparedEventArgs e)
    {
    }

    /// <summary>
    /// Called after the container has been prepared.
    /// </summary>
    /// <param name="sender">The items control raising the event.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
    }

    /// <summary>
    /// Called when the index of a container has changed.
    /// </summary>
    /// <param name="sender">The items control raising the event.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnContainerIndexChanged(object? sender, ContainerIndexChangedEventArgs e)
    {
    }

    /// <summary>
    /// Called when a container is being cleared.
    /// </summary>
    /// <param name="sender">The items control raising the event.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
    }
}
