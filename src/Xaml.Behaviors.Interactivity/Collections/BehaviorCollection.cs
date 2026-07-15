// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Represents a collection of <see cref="IBehavior"/>'s with a shared <see cref="AssociatedObject"/>.
/// </summary>
public class BehaviorCollection : AvaloniaList<AvaloniaObject>
{
    // After a VectorChanged event we need to compare the current state of the collection
    // with the old collection so that we can call Detach on all removed items.
    private readonly List<IBehavior> _oldCollection = [];
    private readonly List<IBehavior> _pendingSynchronizations = [];
    private bool _isAttachingCollection;
    private bool _isSynchronizingCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="BehaviorCollection"/> class.
    /// </summary>
    public BehaviorCollection()
    {
        CollectionChanged += BehaviorCollection_CollectionChanged;
    }

    /// <summary>
    /// Gets the <see cref="AvaloniaObject"/> to which the <see cref="BehaviorCollection"/> is attached.
    /// </summary>
    public AvaloniaObject? AssociatedObject
    {
        get;
        private set;
    }

    /// <summary>
    /// Attaches the collection of behaviors to the specified <see cref="AvaloniaObject"/>.
    /// </summary>
    /// <param name="associatedObject">The <see cref="AvaloniaObject"/> to which to attach.</param>
    /// <exception cref="InvalidOperationException">The <see cref="BehaviorCollection"/> is already attached to a different <see cref="AvaloniaObject"/>.</exception>
    public void Attach(AvaloniaObject? associatedObject)
    {
        if (Equals(associatedObject, AssociatedObject))
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            throw new InvalidOperationException(
                "An instance of a behavior cannot be attached to more than one object at a time.");
        }

        Debug.Assert(associatedObject is not null, "The previous checks should keep us from ever setting null here.");
        AssociatedObject = associatedObject;
        var behaviors = this.OfType<IBehavior>().ToList();
        _isAttachingCollection = true;
        try
        {
            foreach (var behavior in behaviors)
            {
                var associatedObjectForAttach = AssociatedObject;
                if (associatedObjectForAttach is null)
                {
                    break;
                }

                if (behavior is not AvaloniaObject behaviorObject || !Contains(behaviorObject))
                {
                    continue;
                }

                behavior.Attach(associatedObjectForAttach);
            }
        }
        finally
        {
            _isAttachingCollection = false;
        }

        SynchronizeBehaviorEvents(this.OfType<IBehavior>().ToList());
    }

    /// <summary>
    /// Detaches the collection of behaviors from the <see cref="BehaviorCollection.AssociatedObject"/>.
    /// </summary>
    public void Detach()
    {
        foreach (var item in this)
        {
            if (item is IBehavior { AssociatedObject: not null } behaviorItem)
            {
                behaviorItem.Detach();
            }
        }

        AssociatedObject = null;
        _oldCollection.Clear();
    }

    internal void AttachedToVisualTree()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.AttachedToVisualTreeEventHandler();
            }
        }
    }

    internal void DetachedFromVisualTree()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.DetachedFromVisualTreeEventHandler();
            }
        }
    }

    internal void AttachedToLogicalTree()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.AttachedToLogicalTreeEventHandler();
            }
        }
    }

    internal void DetachedFromLogicalTree()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.DetachedFromLogicalTreeEventHandler();
            }
        }
    }

    internal void Loaded()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.LoadedEventHandler();
            }
        }
    }

    internal void Unloaded()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.UnloadedEventHandler();
            }
        }
    }

    internal void Initialized()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.InitializedEventHandler();
            }
        }
    }

    internal void DataContextChanged()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.DataContextChangedEventHandler();
            }
        }
    }

    internal void ResourcesChanged()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.ResourcesChangedEventHandler();
            }
        }
    }

    internal void ActualThemeVariantChanged()
    {
        foreach (var item in this.ToList())
        {
            if (item is IBehaviorEventsHandler behaviorEventsHandler and IBehavior { AssociatedObject: not null })
            {
                behaviorEventsHandler.ActualThemeVariantChangedEventHandler();
            }
        }
    }
    
    private void BehaviorCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var behavior in _oldCollection)
            {
                if (behavior.AssociatedObject is not null)
                {
                    behavior.Detach();
                }
            }

            _oldCollection.Clear();

            var attachedBehaviors = new List<IBehavior>(Count);
            foreach (var newItem in this.ToList())
            {
                var behavior = VerifiedAttach(newItem);
                _oldCollection.Add(behavior);
                attachedBehaviors.Add(behavior);
            }

            if (!_isAttachingCollection)
            {
                QueueOrSynchronizeBehaviorEvents(attachedBehaviors);
            }
#if DEBUG
            VerifyOldCollectionIntegrity();
#endif
            return;
        }

        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                var eventIndex = eventArgs.NewStartingIndex;
                var changedItem = eventArgs.NewItems?[0] as AvaloniaObject;
                var behavior = VerifiedAttach(changedItem);
                _oldCollection.Insert(eventIndex, behavior);
                if (!_isAttachingCollection)
                {
                    QueueOrSynchronizeBehaviorEvents(behavior);
                }
                break;
            }

            case NotifyCollectionChangedAction.Replace:
            {
                var eventIndex = eventArgs.OldStartingIndex;
                eventIndex = eventIndex == -1 ? 0 : eventIndex;

                var changedItem = eventArgs.NewItems?[0] as AvaloniaObject;

                var oldItem = _oldCollection[eventIndex];
                if (oldItem.AssociatedObject is not null)
                {
                    oldItem.Detach();
                }

                var behavior = VerifiedAttach(changedItem);
                _oldCollection[eventIndex] = behavior;
                if (!_isAttachingCollection)
                {
                    QueueOrSynchronizeBehaviorEvents(behavior);
                }
                break;
            }

            case NotifyCollectionChangedAction.Remove:
            {
                var eventIndex = eventArgs.OldStartingIndex;

                var oldItem = _oldCollection[eventIndex];
                if (oldItem.AssociatedObject is not null)
                {
                    oldItem.Detach();
                }

                _oldCollection.RemoveAt(eventIndex);
                break;
            }

            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
            default:
            {
                Debug.Assert(false, "Unsupported collection operation attempted.");
                break;
            }
        }
#if DEBUG
        VerifyOldCollectionIntegrity();
#endif
    }

    private IBehavior VerifiedAttach(AvaloniaObject? item)
    {
        if (item is not IBehavior behavior)
        {
            throw new InvalidOperationException(
                $"Only {nameof(IBehavior)} types are supported in a {nameof(BehaviorCollection)}.");
        }

        if (_oldCollection.Contains(behavior))
        {
            throw new InvalidOperationException(
                $"Cannot add an instance of a behavior to a {nameof(BehaviorCollection)} more than once.");
        }

        if (AssociatedObject is not null)
        {
            behavior.Attach(AssociatedObject);
        }

        return behavior;
    }

    private void QueueOrSynchronizeBehaviorEvents(IBehavior behavior)
    {
        if (_isSynchronizingCollection)
        {
            QueuePendingSynchronization(behavior);
            return;
        }

        SynchronizeBehaviorEvents([behavior]);
    }

    private void QueueOrSynchronizeBehaviorEvents(IReadOnlyList<IBehavior> behaviors)
    {
        if (_isSynchronizingCollection)
        {
            foreach (var behavior in behaviors)
            {
                QueuePendingSynchronization(behavior);
            }

            return;
        }

        SynchronizeBehaviorEvents(behaviors);
    }

    private void QueuePendingSynchronization(IBehavior behavior)
    {
        if (!_pendingSynchronizations.Contains(behavior))
        {
            _pendingSynchronizations.Add(behavior);
        }
    }

    private void SynchronizeBehaviorEvents(IReadOnlyList<IBehavior> behaviors)
    {
        _isSynchronizingCollection = true;
        try
        {
            var currentBatch = behaviors;
            while (currentBatch.Count > 0)
            {
                foreach (var behavior in currentBatch)
                {
                    SynchronizeInitialized(behavior);
                }

                foreach (var behavior in currentBatch)
                {
                    SynchronizeLogicalAttachment(behavior);
                }

                foreach (var behavior in currentBatch)
                {
                    SynchronizeVisualAttachment(behavior);
                }

                foreach (var behavior in currentBatch)
                {
                    SynchronizeLoaded(behavior);
                }

                if (_pendingSynchronizations.Count == 0)
                {
                    break;
                }

                currentBatch = _pendingSynchronizations.ToList();
                _pendingSynchronizations.Clear();
            }
        }
        finally
        {
            _isSynchronizingCollection = false;
            _pendingSynchronizations.Clear();
        }
    }

    private static void SynchronizeInitialized(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            behavior.AssociatedObject is StyledElement { IsInitialized: true })
        {
            eventsHandler.InitializedEventHandler();
        }
    }

    private static void SynchronizeLogicalAttachment(IBehavior behavior)
    {
        var associatedObject = behavior.AssociatedObject;
        var isOpenTopLevel = associatedObject is TopLevel { IsLoaded: true };
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            associatedObject is StyledElement styledElement &&
            (((ILogical)styledElement).IsAttachedToLogicalTree || isOpenTopLevel))
        {
            eventsHandler.AttachedToLogicalTreeEventHandler();
        }
    }

    private static void SynchronizeVisualAttachment(IBehavior behavior)
    {
        var associatedObject = behavior.AssociatedObject;
        var isOpenTopLevel = associatedObject is TopLevel { IsLoaded: true };
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            associatedObject is Visual visual &&
            (visual.IsAttachedToVisualTree() || isOpenTopLevel))
        {
            eventsHandler.AttachedToVisualTreeEventHandler();
        }
    }

    private static void SynchronizeLoaded(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            behavior.AssociatedObject is Control { IsLoaded: true })
        {
            eventsHandler.LoadedEventHandler();
        }
    }

    [Conditional("DEBUG")]
    private void VerifyOldCollectionIntegrity()
    {
        var isValid = Count == _oldCollection.Count;
        if (isValid)
        {
            for (var i = 0; i < Count; i++)
            {
                if (!Equals(this[i], _oldCollection[i]))
                {
                    isValid = false;
                    break;
                }
            }
        }

        Debug.Assert(isValid, "Referential integrity of the collection has been compromised.");
    }
}
