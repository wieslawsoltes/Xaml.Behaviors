// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.Foundation.Collections;

namespace Xaml.Interactivity;

/// <summary>
/// Translates WinUI <see cref="IObservableVector{T}"/> notifications into
/// <see cref="NotifyCollectionChangedEventArgs"/> so collections expose the <see cref="INotifyCollectionChanged"/>
/// contract used by the shared (Avalonia first) sources.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class VectorChangeTranslator<T>
{
    private readonly List<T> _snapshot = [];

    /// <summary>
    /// Updates the snapshot and creates the equivalent collection change notification.
    /// </summary>
    /// <param name="items">The collection after the change.</param>
    /// <param name="e">The vector change.</param>
    /// <returns>The collection change notification.</returns>
    public NotifyCollectionChangedEventArgs Translate(IList<T> items, IVectorChangedEventArgs e)
    {
        var index = (int)e.Index;
        switch (e.CollectionChange)
        {
            case CollectionChange.ItemInserted:
            {
                var item = items[index];
                _snapshot.Insert(index, item);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            }
            case CollectionChange.ItemRemoved:
            {
                var item = _snapshot[index];
                _snapshot.RemoveAt(index);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            }
            case CollectionChange.ItemChanged:
            {
                var oldItem = _snapshot[index];
                var newItem = items[index];
                _snapshot[index] = newItem;
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
            }
            default:
                _snapshot.Clear();
                _snapshot.AddRange(items);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        }
    }
}
