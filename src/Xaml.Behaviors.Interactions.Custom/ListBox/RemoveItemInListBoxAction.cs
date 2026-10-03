// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using ListBox = Microsoft.UI.Xaml.Controls.Primitives.Selector;
using ListBoxItem = Microsoft.UI.Xaml.Controls.Primitives.SelectorItem;
#else
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Allows a user to remove the item from a <see cref="ListBox"/> ItemTemplate.
/// </summary>
/// <remarks>
/// On Uno Platform any WinUI selector (<c>ListView</c>, <c>ListBox</c>, ...) with directly added items is supported.
/// </remarks>
public sealed class RemoveItemInListBoxAction : StyledElementAction
{
    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not Control control)
        {
            return false;
        }

        var itemsControl = control.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault();
        if (itemsControl is null)
        {
            return false;
        }

        if (itemsControl.ItemsSource is IList listItemsSource && !listItemsSource.IsReadOnly)
        {
            var data = control.DataContext;
            if (listItemsSource.Contains(data))
            {
                listItemsSource.Remove(data);
                return true;
            }
        }
        else if (itemsControl is ListBox listBox)
        {
            var listBoxItem = control.GetSelfAndLogicalAncestors().OfType<ListBoxItem>().FirstOrDefault();
            if (listBoxItem is not null)
            {
#if UNO
                // Native WinUI does not set the data context of the container of a directly added item.
                var item = listBox.ItemFromContainer(listBoxItem) ?? listBoxItem.DataContext;
                if (listBox.Items is System.Collections.Generic.IList<object> listItems && listItems.Contains(item))
                {
                    listItems.Remove(item);
                    return true;
                }
#else
                if (listBox.Items is IList listItems && listItems.Contains(listBoxItem.DataContext))
                {
                    listItems.Remove(listBoxItem.DataContext);
                    return true;
                }
#endif
            }
        }

        return false;
    }
}
