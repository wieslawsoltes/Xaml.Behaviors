// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
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
/// Allows a user to add the item to <see cref="ItemsControl"/>.
/// </summary>
public sealed partial class AddItemToItemsControlAction : StyledElementAction
{
    
    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ItemsControl? ItemsControl { get; set; }
    
    /// <summary>
    /// Gets or sets item to add.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial object? Item { get; set; }

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
            listItemsSource.Add(item);
            return true;
        }

        return false;
    }
}
