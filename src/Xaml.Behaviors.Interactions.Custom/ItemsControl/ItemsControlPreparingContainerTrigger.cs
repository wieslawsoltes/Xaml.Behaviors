// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that listens for a <see cref="ItemsControl.PreparingContainer"/> event on its source and executes its actions when that event is fired.
/// </summary>
public class ItemsControlPreparingContainerTrigger : StyledElementTrigger<ItemsControl>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PreparingContainer += ItemsControlOnPreparingContainer;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PreparingContainer -= ItemsControlOnPreparingContainer;
        }
    }

    private void ItemsControlOnPreparingContainer(object? sender, ContainerPreparedEventArgs e)
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
