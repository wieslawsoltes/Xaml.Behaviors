// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Allows a user to insert the item to a <see cref="ItemsControl"/>.
/// </summary>
public sealed partial class InsertItemToItemsControlAction : StyledElementAction
{
  
    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ItemsControl? ItemsControl { get; set; }

    /// <summary>
    /// Gets or sets item to insert.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial object? Item { get; set; }

#if UNO
    /// <summary>
    /// Gets or sets the factory that creates a new item on every execution. This is a dependency property.
    /// </summary>
    /// <remarks>
    /// Uno Platform counterpart of an Avalonia <c>ObjectTemplate</c> assigned to <see cref="Item"/>: WinUI templates only
    /// create UI elements. When set, the created item is used instead of <see cref="Item"/>.
    /// </remarks>
    [StyledProperty]
    public partial IItemFactory? ItemFactory { get; set; }
#endif

    /// <summary>
    /// Gets or sets item index to insert.
    /// </summary>
    [StyledProperty]
    public partial int Index { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

#if UNO
        var item = ItemFactory is { } itemFactory ? itemFactory.CreateItem() : Item;
#else
        var item = Item;
#endif
        if (item is null)
        {
            return false;
        }

#if UNO
        if (item is DataTemplate template)
        {
            item = template.LoadContent();
        }
#else
        if (item is ITemplate template)
        {
            item = template.Build();
        }
#endif

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
