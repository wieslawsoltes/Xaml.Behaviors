// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Adds file paths from a drag event to an <see cref="ItemsControl"/>.
/// </summary>
public sealed class AddPreviewFilesAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<AddPreviewFilesAction, ItemsControl?>(nameof(ItemsControl));

    /// <summary>
    /// Gets or sets the items control used to display preview. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ItemsControl? ItemsControl
    {
        get => GetValue(ItemsControlProperty);
        set => SetValue(ItemsControlProperty, value);
    }

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

        if (itemsControl.ItemsSource is IList list && !list.IsReadOnly)
        {
            list.Clear();
            foreach (var file in files)
            {
                list.Add(file);
            }

            return true;
        }

        return false;
    }
}
