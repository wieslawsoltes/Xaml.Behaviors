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
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Adds a range of items to a target <see cref="IList"/> when invoked.
/// </summary>
public sealed partial class AddRangeAction : AvaloniaObject, IAction
{

    /// <summary>
    /// Gets or sets the items to add.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial IEnumerable? Items { get; set; }

    /// <summary>
    /// Gets or sets the collection to add items to.
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

        foreach (var item in Items)
        {
            list.Add(item);
        }

        return true;
    }
}
