// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Allows a user to insert the item to a <see cref="ItemsControl"/>.
/// </summary>
public sealed class InsertItemToItemsControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<InsertItemToItemsControlAction, ItemsControl?>(nameof(ItemsControl));

    /// <summary>
    /// Identifies the <see cref="Item"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ItemProperty =
        AvaloniaProperty.Register<InsertItemToItemsControlAction, object?>(nameof(Item));
    
    /// <summary>
    /// Identifies the <see cref="Index"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<InsertItemToItemsControlAction, int>(nameof(Index));
  
    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [ResolveByName]
    public ItemsControl? ItemsControl
    {
        get => GetValue(ItemsControlProperty);
        set => SetValue(ItemsControlProperty, value);
    }

    /// <summary>
    /// Gets or sets item to insert.
    /// </summary>
    public object? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    /// <summary>
    /// Gets or sets item index to insert.
    /// </summary>
    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var item = Item;
        if (item is null)
        {
            return false;
        }

        var itemsControl = ItemsControl;
        if (itemsControl is null)
        {
            return false;
        }

        if (itemsControl.ItemsSource is IList listItemsSource && !listItemsSource.IsReadOnly)
        {
            listItemsSource.Insert(Index, item);
            return true;
        }

        return false;
    }
}
