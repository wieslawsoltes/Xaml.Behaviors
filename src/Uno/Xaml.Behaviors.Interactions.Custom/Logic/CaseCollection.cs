// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Windows.Foundation.Collections;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Represents the collection of <see cref="Case"/> items of a <see cref="SwitchCaseAction"/> or
/// <see cref="SwitchCaseBehavior"/> (Uno Platform counterpart of the Avalonia <c>AvaloniaList&lt;Case&gt;</c>).
/// </summary>
/// <remarks>
/// A dependency object collection, so the cases inherit the data context of their switch like the actions of an
/// <see cref="ActionCollection"/>. Items that are not <see cref="Case"/> instances are ignored.
/// </remarks>
public class CaseCollection : DependencyObjectCollection, INotifyCollectionChanged
{
    private readonly VectorChangeTranslator<DependencyObject> _changes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CaseCollection"/> class.
    /// </summary>
    public CaseCollection()
    {
        VectorChanged += CaseCollection_VectorChanged;
    }

    /// <summary>
    /// Occurs when the collection changes (translated from <c>VectorChanged</c>).
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Returns an enumerator over the cases of the collection.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public new IEnumerator<Case> GetEnumerator()
    {
        foreach (var item in (IEnumerable<DependencyObject>)this)
        {
            if (item is Case caseItem)
            {
                yield return caseItem;
            }
        }
    }

    private void CaseCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
        => CollectionChanged?.Invoke(this, _changes.Translate(this, eventArgs));
}
