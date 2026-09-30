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
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Moves an item within an <see cref="ItemsControl"/> from <see cref="FromIndex"/> to <see cref="ToIndex"/>.
/// </summary>
public sealed partial class MoveItemInItemsControlAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ItemsControl? ItemsControl { get; set; }

    /// <summary>
    /// Gets or sets source index.
    /// </summary>
    [StyledProperty]
    public partial int FromIndex { get; set; }

    /// <summary>
    /// Gets or sets target index.
    /// </summary>
    [StyledProperty]
    public partial int ToIndex { get; set; }

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
            if (sender is Control control)
            {
                itemsControl = control.GetSelfAndLogicalAncestors().OfType<ItemsControl>().FirstOrDefault();
            }
        }

        if (itemsControl?.ItemsSource is IList list && !list.IsReadOnly)
        {
            var targetIndex = ToIndex < 0 ? list.Count - 1 : ToIndex;

            if (FromIndex >= 0 && FromIndex < list.Count && targetIndex >= 0 && targetIndex < list.Count && FromIndex != targetIndex)
            {
                var item = list[FromIndex];
                list.RemoveAt(FromIndex);
                list.Insert(targetIndex, item);
                return true;
            }
        }

        return false;
    }
}
