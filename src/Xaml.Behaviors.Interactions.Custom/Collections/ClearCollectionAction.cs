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
/// Clears all items from a target <see cref="IList"/> when invoked.
/// </summary>
public sealed partial class ClearCollectionAction : AvaloniaObject, IAction
{

    /// <summary>
    /// Gets or sets the collection to clear.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial IList? Target { get; set; }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        if (Target is not { } list)
        {
            return false;
        }

        list.Clear();
        return true;
    }
}
