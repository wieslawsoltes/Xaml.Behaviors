// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Adds file paths from a drag event to an <see cref="ItemsControl"/>.
/// </summary>
/// <remarks>
/// The files replace the items of a modifiable <c>ItemsSource</c> list or, when the items control has no
/// <c>ItemsSource</c>, its <c>Items</c>.
/// </remarks>
public sealed partial class AddPreviewFilesAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the items control used to display preview. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ItemsControl? ItemsControl { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled || parameter is not DragEventArgs e)
        {
            return false;
        }

        var itemsControl = ItemsControl ?? sender as ItemsControl;
        if (itemsControl is null)
        {
            return false;
        }

        if (!e.DataTransfer.Contains(DataFormat.File))
        {
            return false;
        }

        var files = e.DataTransfer.TryGetFiles();
        if (files is null || files.Length == 0)
        {
            return false;
        }

        return TryReplaceItems(itemsControl, files);
    }

    /// <summary>
    /// Replaces the items of a modifiable <c>ItemsSource</c> list or, when there is no <c>ItemsSource</c>, the
    /// <c>Items</c> of an items control.
    /// </summary>
    /// <param name="itemsControl">The items control.</param>
    /// <param name="items">The new items.</param>
    /// <returns><c>true</c> when the items were replaced; <c>false</c> when the items source cannot be modified.</returns>
    internal static bool TryReplaceItems(ItemsControl itemsControl, IReadOnlyList<object> items)
    {
        if (itemsControl.ItemsSource is null)
        {
            var itemCollection = itemsControl.Items;
            itemCollection.Clear();
            for (var i = 0; i < items.Count; i++)
            {
                itemCollection.Add(items[i]);
            }

            return true;
        }

        if (itemsControl.ItemsSource is IList list && !list.IsReadOnly)
        {
            list.Clear();
            for (var i = 0; i < items.Count; i++)
            {
                list.Add(items[i]);
            }

            return true;
        }

        return false;
    }
}
