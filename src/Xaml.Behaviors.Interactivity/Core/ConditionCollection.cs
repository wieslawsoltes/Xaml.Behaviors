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
/// data context of the owning behavior (on native WinUI, the <c>DependencyObjectCollection&lt;T&gt;</c> of the WinUI port).
/// </remarks>
#if UNO
public partial class ConditionCollection : DependencyObjectCollection<Condition>, System.Collections.Specialized.INotifyCollectionChanged
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
}
#else
public partial class ConditionCollection : AvaloniaList<Condition>
{
}
#endif
