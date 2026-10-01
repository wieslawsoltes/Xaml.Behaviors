// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Specialized;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes different sets of actions when the observed collection changes.
/// </summary>
public sealed partial class CollectionChangedBehavior : DisposingBehavior<AvaloniaObject>
{

    /// <summary>
    /// Gets or sets the collection to observe.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial INotifyCollectionChanged? Collection { get; set; }

    /// <summary>
    /// Actions invoked when items are added to the collection.
    /// </summary>
    [DirectProperty(Lazy = true)]
    public partial ActionCollection AddedActions { get; }

    /// <summary>
    /// Actions invoked when items are removed from the collection.
    /// </summary>
    [DirectProperty(Lazy = true)]
    public partial ActionCollection RemovedActions { get; }

    /// <summary>
    /// Actions invoked when the collection is reset.
    /// </summary>
    [DirectProperty(Lazy = true)]
    public partial ActionCollection ResetActions { get; }

    private INotifyCollectionChanged? _observedCollection;

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var actions = e.Action switch
        {
            NotifyCollectionChangedAction.Add => AddedActions,
            NotifyCollectionChangedAction.Remove => RemovedActions,
            NotifyCollectionChangedAction.Reset => ResetActions,
            _ => null
        };

        if (actions is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, actions, e);
        }
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedOverride()
    {
        Observe(Collection);

        return DisposableAction.Create(() => Observe(null));
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CollectionProperty && AssociatedObject is not null)
        {
            Observe(Collection);
        }
    }

    private void Observe(INotifyCollectionChanged? collection)
    {
        if (ReferenceEquals(_observedCollection, collection))
        {
            return;
        }

        if (_observedCollection is not null)
        {
            _observedCollection.CollectionChanged -= OnCollectionChanged;
        }

        _observedCollection = collection;

        if (_observedCollection is not null)
        {
            _observedCollection.CollectionChanged += OnCollectionChanged;
        }
    }
}
