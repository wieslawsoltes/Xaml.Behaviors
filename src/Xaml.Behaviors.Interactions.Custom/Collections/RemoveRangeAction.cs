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
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Removes a range of items from a target <see cref="IList"/> when invoked.
/// </summary>
public sealed partial class RemoveRangeAction : AvaloniaObject, IAction
{

    /// <summary>
    /// Gets or sets the items to remove.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial IEnumerable? Items { get; set; }

    /// <summary>
    /// Gets or sets the collection to remove items from.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial IList? Target { get; set; }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        if (Target is not { } list || Items is null)
        {
            return false;
        }

        var items = Items.Cast<object>().ToList();
  
        foreach (var item in items)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }

        return true;
    }
}
