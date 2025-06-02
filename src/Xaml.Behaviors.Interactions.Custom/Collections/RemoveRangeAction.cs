// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Removes a range of items from a target <see cref="IList"/> when invoked.
/// </summary>
public sealed class RemoveRangeAction : AvaloniaObject, IAction
{
    /// <summary>
    /// Identifies the <see cref="Items"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsProperty =
        AvaloniaProperty.Register<RemoveRangeAction, IEnumerable?>(nameof(Items));

    /// <summary>
    /// Identifies the <see cref="Target"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IList?> TargetProperty =
        AvaloniaProperty.Register<RemoveRangeAction, IList?>(nameof(Target));

    /// <summary>
    /// Gets or sets the items to remove.
    /// </summary>
    [Content]
    public IEnumerable? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection to remove items from.
    /// </summary>
    [ResolveByName]
    public IList? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <inheritdoc />
    public object Execute(object? sender, object? parameter)
    {
        if (Target is not { } list || Items is null)
        {
            return false;
        }

        foreach (var item in Items)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }

        return true;
    }
}
