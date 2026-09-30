// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// A behavior that listens for a <see cref="ItemsControl.ContainerPrepared"/> event on its source and executes its actions when that event is fired.
/// </summary>
/// <remarks>
/// On Uno Platform the container lifecycle events are raised by the WinUI <c>ItemsRepeater</c>
/// (<c>ElementPrepared</c>, <c>ElementIndexChanged</c> and <c>ElementClearing</c>).
/// </remarks>
public class ItemsControlContainerPreparedTrigger : StyledElementTrigger<ItemsControl>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
#if UNO
            AssociatedObject.ElementPrepared += ItemsControlOnContainerPrepared;
#else
            AssociatedObject.ContainerPrepared += ItemsControlOnContainerPrepared;
#endif
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
#if UNO
            AssociatedObject.ElementPrepared -= ItemsControlOnContainerPrepared;
#else
            AssociatedObject.ContainerPrepared -= ItemsControlOnContainerPrepared;
#endif
        }
    }

    private void ItemsControlOnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
