// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia.Collections;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Represents a collection of <see cref="Condition"/> objects.
/// </summary>
/// <remarks>
/// On Uno Platform the collection is a <c>DependencyObjectCollection&lt;Condition&gt;</c> so the conditions inherit the
/// data context of the owning behavior. Native WinUI has no generic <c>DependencyObjectCollection</c> (and WinUI XAML
/// does not support generic types): the collection derives from <c>DependencyObjectCollection</c> and enumerates its
/// conditions.
/// </remarks>
#if WINUI
public partial class ConditionCollection : DependencyObjectCollection, System.Collections.Generic.IEnumerable<Condition>, System.Collections.Specialized.INotifyCollectionChanged
#elif UNO
public partial class ConditionCollection : DependencyObjectCollection<Condition>, System.Collections.Specialized.INotifyCollectionChanged
#endif
#if UNO
{
#if WINUI
    private readonly VectorChangeTranslator<DependencyObject> _changes = new();
#else
    private readonly VectorChangeTranslator<Condition> _changes = new();
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
    /// </summary>
    public ConditionCollection()
    {
        VectorChanged += (_, e) => CollectionChanged?.Invoke(this, _changes.Translate(this, e));
    }

    /// <summary>
    /// Occurs when the collection changes (translated from <c>VectorChanged</c>).
    /// </summary>
    public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;
#if WINUI

    /// <summary>
    /// Returns an enumerator over the conditions of the collection.
    /// </summary>
    /// <returns>The conditions, in order.</returns>
    public new System.Collections.Generic.IEnumerator<Condition> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            if (this[i] is Condition condition)
            {
                yield return condition;
            }
        }
    }

    /// <inheritdoc />
    System.Collections.Generic.IEnumerator<Condition> System.Collections.Generic.IEnumerable<Condition>.GetEnumerator() => GetEnumerator();
#endif
}
#else
public partial class ConditionCollection : AvaloniaList<Condition>
{
}
#endif
