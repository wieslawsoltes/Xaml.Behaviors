// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes different sets of actions when the observed collection changes.
/// </summary>
public sealed class CollectionChangedBehavior : DisposingBehavior<AvaloniaObject>
{
    /// <summary>
    /// Identifies the <see cref="Collection"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotifyCollectionChanged?> CollectionProperty =
        AvaloniaProperty.Register<CollectionChangedBehavior, INotifyCollectionChanged?>(nameof(Collection));

    /// <summary>
    /// Identifies the <see cref="AddedActions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<CollectionChangedBehavior, ActionCollection> AddedActionsProperty =
        AvaloniaProperty.RegisterDirect<CollectionChangedBehavior, ActionCollection>(nameof(AddedActions), b => b.AddedActions);

    /// <summary>
    /// Identifies the <see cref="RemovedActions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<CollectionChangedBehavior, ActionCollection> RemovedActionsProperty =
        AvaloniaProperty.RegisterDirect<CollectionChangedBehavior, ActionCollection>(nameof(RemovedActions), b => b.RemovedActions);

    /// <summary>
    /// Identifies the <see cref="ResetActions"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<CollectionChangedBehavior, ActionCollection> ResetActionsProperty =
        AvaloniaProperty.RegisterDirect<CollectionChangedBehavior, ActionCollection>(nameof(ResetActions), b => b.ResetActions);

    private ActionCollection? _addedActions;
    private ActionCollection? _removedActions;
    private ActionCollection? _resetActions;

    /// <summary>
    /// Gets or sets the collection to observe.
    /// </summary>
    [ResolveByName]
    public INotifyCollectionChanged? Collection
    {
        get => GetValue(CollectionProperty);
        set => SetValue(CollectionProperty, value);
    }

    /// <summary>
    /// Actions invoked when items are added to the collection.
    /// </summary>
    [Content]
    public ActionCollection AddedActions => _addedActions ??= [];

    /// <summary>
    /// Actions invoked when items are removed from the collection.
    /// </summary>
    [Content]
    public ActionCollection RemovedActions => _removedActions ??= [];

    /// <summary>
    /// Actions invoked when the collection is reset.
    /// </summary>
    [Content]
    public ActionCollection ResetActions => _resetActions ??= [];

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
        if (Collection is null)
        {
            return DisposableAction.Empty;
        }

        Collection.CollectionChanged += OnCollectionChanged;
        return DisposableAction.Create(() => Collection.CollectionChanged -= OnCollectionChanged);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CollectionProperty)
        {
            if (change.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged;
            }
            if (change.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }
        }
    }
}
