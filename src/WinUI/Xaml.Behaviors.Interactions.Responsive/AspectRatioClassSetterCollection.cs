// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Responsive;

/// <summary>
/// The collection of <see cref="AspectRatioClassSetter"/> items of <see cref="AspectRatioBehavior.Setters"/> on native WinUI.
/// </summary>
/// <remarks>
/// Uno Platform uses its generic <c>DependencyObjectCollection&lt;T&gt;</c>; native WinUI has none and WinUI XAML does
/// not support generic collection types. The items inherit the data context of the behavior, like the items of any
/// <see cref="DependencyObjectCollection"/>.
/// </remarks>
public partial class AspectRatioClassSetterCollection : DependencyObjectCollection, IEnumerable<AspectRatioClassSetter>
{
    /// <summary>
    /// Returns an enumerator over the <see cref="AspectRatioClassSetter"/> items of the collection.
    /// </summary>
    /// <returns>The items, in order.</returns>
    public new IEnumerator<AspectRatioClassSetter> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            if (this[i] is AspectRatioClassSetter item)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc />
    IEnumerator<AspectRatioClassSetter> IEnumerable<AspectRatioClassSetter>.GetEnumerator() => GetEnumerator();
}
