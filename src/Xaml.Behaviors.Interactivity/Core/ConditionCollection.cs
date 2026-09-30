// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if !UNO
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
/// data context of the owning behavior.
/// </remarks>
#if UNO
public class ConditionCollection : Microsoft.UI.Xaml.DependencyObjectCollection<Condition>, System.Collections.Specialized.INotifyCollectionChanged
{
    private readonly VectorChangeTranslator<Condition> _changes = new();

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
}
#else
public class ConditionCollection : AvaloniaList<Condition>
{
}
#endif
