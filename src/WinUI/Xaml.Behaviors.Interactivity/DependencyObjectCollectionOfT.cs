// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// A <see cref="DependencyObjectCollection"/> of <typeparamref name="T"/> items, the native WinUI counterpart of the
/// generic <c>DependencyObjectCollection&lt;T&gt;</c> of Uno Platform used by the shared sources.
/// </summary>
/// <typeparam name="T">The type of the items.</typeparam>
/// <remarks>
/// The items inherit the data context of the object that owns the collection, like the items of any
/// <see cref="DependencyObjectCollection"/>. XAML and code add items through the untyped collection (a second
/// collection interface would make the collection unusable in WinUI XAML); items of another type are kept but skipped
/// by the typed enumerator.
/// </remarks>
public partial class DependencyObjectCollection<T> : DependencyObjectCollection, IEnumerable<T>
    where T : DependencyObject
{
    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The item.</returns>
    public new T this[int index]
    {
        get => (T)base[index];
        set => base[index] = value;
    }

    /// <summary>
    /// Returns an enumerator over the <typeparamref name="T"/> items of the collection.
    /// </summary>
    /// <returns>The items, in order.</returns>
    public new IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            if (base[i] is T item)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}
