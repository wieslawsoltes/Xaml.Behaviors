// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Avalonia <c>ListBox.SelectAll</c>/<c>UnselectAll</c> for the WinUI <see cref="ListViewBase"/>.
/// </summary>
/// <remarks>
/// <see cref="ListViewBase.SelectAll"/> is not implemented by Uno Platform Skia: the items are added to
/// <see cref="ListViewBase.SelectedItems"/> instead.
/// </remarks>
internal static class ListViewBaseSelection
{
    /// <summary>
    /// Selects all items (multiple or extended selection modes); selects the first item in single selection mode.
    /// </summary>
    /// <param name="listView">The list view.</param>
    public static void SelectAll(ListViewBase? listView)
    {
        if (listView is null || listView.Items.Count == 0)
        {
            return;
        }

        switch (listView.SelectionMode)
        {
            case ListViewSelectionMode.None:
                return;
            case ListViewSelectionMode.Single:
                // A single selection list can only select one item.
                listView.SelectedIndex = 0;
                return;
        }

        var selected = listView.SelectedItems;
        foreach (var item in listView.Items)
        {
            if (!selected.Contains(item))
            {
                selected.Add(item);
            }
        }
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    /// <param name="listView">The list view.</param>
    public static void UnselectAll(ListViewBase? listView)
    {
        if (listView is null)
        {
            return;
        }

        if (listView.SelectionMode is ListViewSelectionMode.Multiple or ListViewSelectionMode.Extended)
        {
            listView.SelectedItems.Clear();
        }

        listView.SelectedIndex = -1;
    }
}
