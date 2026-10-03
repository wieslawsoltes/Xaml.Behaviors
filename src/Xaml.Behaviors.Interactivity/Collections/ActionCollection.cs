// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Specialized;
#if UNO
using Microsoft.UI.Xaml;
using Windows.Foundation.Collections;
#else
using Avalonia.Collections;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Represents a collection of <see cref="IAction"/>'s.
/// </summary>
#if UNO
public partial class ActionCollection : DependencyObjectCollection, INotifyCollectionChanged
{
    private readonly System.Collections.Generic.List<DependencyObject> _items = [];
    private readonly VectorChangeTranslator<DependencyObject> _changes = new();

    /// <summary>
    /// Occurs when the collection changes (translated from <c>VectorChanged</c>).
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    private DependencyObject? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCollection"/> class.
    /// </summary>
    public ActionCollection()
    {
        VectorChanged += ActionCollection_VectorChanged;
    }

    /// <summary>
    /// Gets the object that hosts the trigger owning this collection.
    /// </summary>
    /// <remarks>
    /// WinUI has no logical tree, so the owning trigger publishes its associated object here and the
    /// actions implementing <see cref="IActionLogicalTreeLifecycle"/> are notified.
    /// </remarks>
    internal DependencyObject? Host => _host;

    /// <summary>
    /// Sets the object that hosts the trigger owning this collection.
    /// </summary>
    /// <param name="host">The host, or <c>null</c> when the trigger is detached.</param>
    internal void SetHost(DependencyObject? host)
    {
        if (ReferenceEquals(_host, host))
        {
            return;
        }

        if (_host is not null)
        {
            foreach (var item in this)
            {
                DetachFromHost(item);
            }
        }

        _host = host;

        if (_host is not null)
        {
            foreach (var item in this)
            {
                AttachToHost(item);
            }
        }
    }

    private void ActionCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
    {
        OnVectorChanged(eventArgs);
        CollectionChanged?.Invoke(this, _changes.Translate(this, eventArgs));
    }

    private void OnVectorChanged(IVectorChangedEventArgs eventArgs)
    {
        var index = (int)eventArgs.Index;

        switch (eventArgs.CollectionChange)
        {
            case CollectionChange.Reset:
            {
                foreach (var item in _items)
                {
                    DetachFromHost(item);
                }

                _items.Clear();

                foreach (var item in this)
                {
                    VerifyType(item);
                    _items.Add(item);
                    AttachToHost(item);
                }

                break;
            }
            case CollectionChange.ItemInserted:
            {
                var changedItem = this[index];
                VerifyType(changedItem);
                _items.Insert(index, changedItem);
                AttachToHost(changedItem);
                break;
            }
            case CollectionChange.ItemChanged:
            {
                var changedItem = this[index];
                VerifyType(changedItem);
                DetachFromHost(_items[index]);
                _items[index] = changedItem;
                AttachToHost(changedItem);
                break;
            }
            case CollectionChange.ItemRemoved:
            {
                DetachFromHost(_items[index]);
                _items.RemoveAt(index);
                break;
            }
        }
    }

    private void AttachToHost(DependencyObject? item)
    {
        if (_host is not null && item is Action action)
        {
            action.AttachToHost(_host);
        }
    }

    private static void DetachFromHost(DependencyObject? item)
    {
        if (item is Action action)
        {
            action.DetachFromHost();
        }
    }
#else
public partial class ActionCollection : AvaloniaList<AvaloniaObject>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCollection"/> class.
    /// </summary>
    public ActionCollection()
    {
        CollectionChanged += ActionCollection_CollectionChanged;
    }

    private void ActionCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        var collectionChangedAction = eventArgs.Action;

        switch (collectionChangedAction)
        {
            case NotifyCollectionChangedAction.Reset:
            {
                foreach (var item in this)
                {
                    VerifyType(item);
                }

                break;
            }
            case NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace:
            {
                var changedItem = eventArgs.NewItems?[0] as AvaloniaObject;
                VerifyType(changedItem);
                break;
            }
        }
    }

#endif

    private static void VerifyType(AvaloniaObject? item)
    {
        if (item is null)
        {
            return;
        }

        if (item is not IAction)
        {
            throw new InvalidOperationException(
                $"Only {nameof(IAction)} types are supported in an {nameof(ActionCollection)}.");
        }
    }
}
