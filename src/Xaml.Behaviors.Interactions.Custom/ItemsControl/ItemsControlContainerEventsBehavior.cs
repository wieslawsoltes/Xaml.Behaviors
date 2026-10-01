// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using ItemsControl = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using ContainerPreparedEventArgs = Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs;
using ContainerClearingEventArgs = Microsoft.UI.Xaml.Controls.ItemsRepeaterElementClearingEventArgs;
using ContainerIndexChangedEventArgs = Microsoft.UI.Xaml.Controls.ItemsRepeaterElementIndexChangedEventArgs;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Base class that exposes container related events from an <see cref="ItemsControl"/>.
/// </summary>
/// <remarks>
/// On Uno Platform the container lifecycle events are raised by the WinUI <c>ItemsRepeater</c>
/// (<c>ElementPrepared</c>, <c>ElementIndexChanged</c> and <c>ElementClearing</c>).
/// </remarks>
public abstract class ItemsControlContainerEventsBehavior : DisposingBehavior<ItemsControl>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is not { } itemsControl)
        {
            return DisposableAction.Empty;
        }

#if UNO
        itemsControl.ElementPrepared += ItemsControlOnContainerPrepared;
        itemsControl.ElementIndexChanged += ItemsControlOnContainerIndexChanged;
        itemsControl.ElementClearing += ItemsControlOnContainerClearing;

        return DisposableAction.Create(() =>
        {
            itemsControl.ElementPrepared -= ItemsControlOnContainerPrepared;
            itemsControl.ElementIndexChanged -= ItemsControlOnContainerIndexChanged;
            itemsControl.ElementClearing -= ItemsControlOnContainerClearing;
        });
#else
        itemsControl.PreparingContainer += ItemsControlOnPreparingContainer;
        itemsControl.ContainerPrepared += ItemsControlOnContainerPrepared;
        itemsControl.ContainerIndexChanged += ItemsControlOnContainerIndexChanged;
        itemsControl.ContainerClearing += ItemsControlOnContainerClearing;

        return DisposableAction.Create(() =>
        {
            itemsControl.PreparingContainer -= ItemsControlOnPreparingContainer;
            itemsControl.ContainerPrepared -=ItemsControlOnContainerPrepared;
            itemsControl.ContainerIndexChanged -= ItemsControlOnContainerIndexChanged;
            itemsControl.ContainerClearing -= ItemsControlOnContainerClearing;
        });
#endif
    }

#if !UNO
    private void ItemsControlOnPreparingContainer(object? sender, ContainerPreparedEventArgs e)
    {
        OnPreparingContainer(sender, e);
    }
#endif

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
    /// <remarks>
    /// Not raised on Uno Platform: <c>ItemsRepeater</c> has no event before an element is prepared.
    /// </remarks>
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
