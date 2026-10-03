// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
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
/// Removes an item at a specified index from an <see cref="ItemsControl"/>.
/// </summary>
public sealed partial class RemoveItemAtAction : AvaloniaObject, IAction
{

    /// <summary>
    /// Gets or sets items control.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ItemsControl? ItemsControl { get; set; }

    /// <summary>
    /// Gets or sets index to remove.
    /// </summary>
    [StyledProperty]
    public partial int Index { get; set; }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        var itemsControl = ItemsControl ?? sender as ItemsControl;
        if (itemsControl?.ItemsSource is IList list && !list.IsReadOnly)
        {
            if (Index >= 0 && Index < list.Count)
            {
                list.RemoveAt(Index);
                return true;
            }
        }

        return false;
    }
}
