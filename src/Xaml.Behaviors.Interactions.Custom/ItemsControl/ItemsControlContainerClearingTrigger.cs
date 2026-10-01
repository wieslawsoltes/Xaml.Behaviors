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
/// A behavior that listens for a <see cref="ItemsControl.ContainerClearing"/> event on its source and executes its actions when that event is fired.
/// </summary>
/// <remarks>
/// On Uno Platform the container lifecycle events are raised by the WinUI <c>ItemsRepeater</c>
/// (<c>ElementPrepared</c>, <c>ElementIndexChanged</c> and <c>ElementClearing</c>).
/// </remarks>
public class ItemsControlContainerClearingTrigger : StyledElementTrigger<ItemsControl>
{
#if UNO
    private ItemsControl? _subscribedItemsControl;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        // WinUI raises Loaded (the visual tree attachment on Uno) after the first layout pass, when the ItemsRepeater
        // already prepared the initially realized elements: subscribe as soon as the trigger is attached.
        Subscribe();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        Unsubscribe();
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        Subscribe();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_subscribedItemsControl is null && AssociatedObject is { } itemsControl)
        {
            itemsControl.ElementClearing += ItemsControlOnContainerClearing;
            _subscribedItemsControl = itemsControl;
        }
    }

    private void Unsubscribe()
    {
        if (_subscribedItemsControl is { } itemsControl)
        {
            itemsControl.ElementClearing -= ItemsControlOnContainerClearing;
            _subscribedItemsControl = null;
        }
    }
#else
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.ContainerClearing += ItemsControlOnContainerClearing;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.ContainerClearing -= ItemsControlOnContainerClearing;
        }
    }
#endif

    private void ItemsControlOnContainerClearing(object? sender, ContainerClearingEventArgs e)
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
