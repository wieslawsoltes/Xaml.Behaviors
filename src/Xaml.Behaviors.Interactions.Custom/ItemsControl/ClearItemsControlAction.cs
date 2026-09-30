using System.Collections;
// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
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
/// Allows a user to clear the items from a <see cref="ItemsControl"/>.
/// </summary>
public sealed class ClearItemsControlAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ItemsControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ItemsControl?> ItemsControlProperty =
        AvaloniaProperty.Register<InsertItemToItemsControlAction, ItemsControl?>(nameof(ItemsControl));
  
    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [ResolveByName]
    public ItemsControl? ItemsControl
    {
        get => (ItemsControl?)GetValue(ItemsControlProperty);
        set => SetValue(ItemsControlProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
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
            listItemsSource.Clear();
            return true;
        }

        return false;
    }
}
