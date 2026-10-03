// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Windows.Foundation.Collections;
#else
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Represents a collection of <see cref="IBehavior"/>'s with a shared <see cref="AssociatedObject"/>.
/// </summary>
#if UNO
public partial class BehaviorCollection : DependencyObjectCollection
#else
public partial class BehaviorCollection : AvaloniaList<AvaloniaObject>
#endif
{
    // After a VectorChanged event we need to compare the current state of the collection
    // with the old collection so that we can call Detach on all removed items.
    private readonly List<IBehavior> _oldCollection = [];
    private readonly List<IBehavior> _pendingSynchronizations = [];
    private bool _isAttachingCollection;
    private bool _isSynchronizingCollection;
    private bool _hasObservedInitialized;
    private bool _hasObservedLogicalAttachment;
    private bool _hasObservedVisualAttachment;
    private bool _hasObservedLoaded;
#if UNO
    private bool _hasPendingDataContextChange;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="BehaviorCollection"/> class.
    /// </summary>
    public BehaviorCollection()
    {
#if UNO
        VectorChanged += BehaviorCollection_VectorChanged;
#else
        CollectionChanged += BehaviorCollection_CollectionChanged;
#endif
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
#if WINUI
        // Behaviors are attached on the UI thread: the dispatcher compat learns it here (see Compat/Dispatcher.cs).
        UIThreadDispatcher.CaptureCurrentThread();
#endif
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
        CaptureCurrentLifecycleState();
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
        _pendingSynchronizations.Clear();
        _hasObservedInitialized = false;
        _hasObservedLogicalAttachment = false;
        _hasObservedVisualAttachment = false;
        _hasObservedLoaded = false;
#if UNO
        _hasPendingDataContextChange = false;
#endif
    }

    internal void AttachedToVisualTree()
    {
        _hasObservedVisualAttachment = true;
        DispatchBehaviorEvent(static handler => handler.AttachedToVisualTreeEventHandler());
    }

    internal void DetachedFromVisualTree()
    {
        _hasObservedVisualAttachment = false;
        DispatchBehaviorEvent(static handler => handler.DetachedFromVisualTreeEventHandler());
    }

    internal void AttachedToLogicalTree()
    {
        _hasObservedLogicalAttachment = true;
        DispatchBehaviorEvent(static handler => handler.AttachedToLogicalTreeEventHandler());
    }

    internal void DetachedFromLogicalTree()
    {
        _hasObservedLogicalAttachment = false;
        DispatchBehaviorEvent(static handler => handler.DetachedFromLogicalTreeEventHandler());
    }

    internal void Loaded()
    {
        _hasObservedLoaded = true;
        DispatchBehaviorEvent(static handler => handler.LoadedEventHandler());
    }

    internal void Unloaded()
    {
        _hasObservedLoaded = false;
        DispatchBehaviorEvent(static handler => handler.UnloadedEventHandler());
    }

    internal void Initialized()
    {
        _hasObservedInitialized = true;
        DispatchBehaviorEvent(static handler => handler.InitializedEventHandler());
    }

    internal void NotifyDataContextChanged()
    {
        DispatchBehaviorEvent(static handler => handler.DataContextChangedEventHandler());
    }

    internal void ResourcesChanged()
    {
        DispatchBehaviorEvent(static handler => handler.ResourcesChangedEventHandler());
    }

    internal void ActualThemeVariantChanged()
    {
        DispatchBehaviorEvent(static handler => handler.ActualThemeVariantChangedEventHandler());
    }

#if UNO
    /// <summary>
    /// Raises the data context notification of a WinUI <c>DataContextChanged</c> event.
    /// </summary>
    /// <remarks>
    /// WinUI raises <c>DataContextChanged</c> when the data context is assigned or inherited, often before the element
    /// loads, and applies the <c>x:Bind</c> values of a view when it loads. A change raised before the behaviors are
    /// loaded is therefore delivered once, after the <c>Loaded</c> lifecycle, so that the behaviors observe the data
    /// context with their <c>x:Bind</c> values set.
    /// </remarks>
    internal void DataContextChanged()
    {
        if (!_hasObservedLoaded)
        {
            _hasPendingDataContextChange = true;
            return;
        }

        NotifyDataContextChanged();
    }

    /// <summary>
    /// Raises the attach phases of a WinUI <c>Loaded</c> event (initialized, logical tree, visual tree and loaded) as
    /// a single host lifecycle event.
    /// </summary>
    /// <remarks>
    /// Avalonia raises the phases as separate host events and synchronizes the behaviors added by an earlier handler
    /// after the event being dispatched. WinUI raises them from one <c>Loaded</c> event, so the added behaviors are
    /// synchronized after the existing behaviors are loaded.
    /// </remarks>
    internal void AttachedToLiveTree()
    {
        var wasSynchronizingCollection = _isSynchronizingCollection;
        _isSynchronizingCollection = true;
        try
        {
            Initialized();
            AttachedToLogicalTree();
            AttachedToVisualTree();
            Loaded();
        }
        finally
        {
            _isSynchronizingCollection = wasSynchronizingCollection;
            if (!wasSynchronizingCollection && _pendingSynchronizations.Count > 0)
            {
                var pending = _pendingSynchronizations.ToList();
                _pendingSynchronizations.Clear();
                SynchronizeBehaviorEvents(pending);
            }
        }

        if (_hasPendingDataContextChange && _hasObservedLoaded)
        {
            _hasPendingDataContextChange = false;
            NotifyDataContextChanged();
        }
    }
#endif

    internal void Opened()
    {
        var wasSynchronizingCollection = _isSynchronizingCollection;
        _isSynchronizingCollection = true;
        try
        {
            AttachedToVisualTree();
            AttachedToLogicalTree();
        }
        finally
        {
            _isSynchronizingCollection = wasSynchronizingCollection;
            if (!wasSynchronizingCollection && _pendingSynchronizations.Count > 0)
            {
                var pending = _pendingSynchronizations.ToList();
                _pendingSynchronizations.Clear();
                SynchronizeBehaviorEvents(pending);
            }
        }
    }

    private void DispatchBehaviorEvent(Action<IBehaviorEventsHandler> dispatch)
    {
        var wasSynchronizingCollection = _isSynchronizingCollection;
        _isSynchronizingCollection = true;
        try
        {
            foreach (var item in this.ToList())
            {
                if (item is IBehaviorEventsHandler behaviorEventsHandler and
                    IBehavior { AssociatedObject: not null } behavior &&
                    !_pendingSynchronizations.Contains(behavior))
                {
                    dispatch(behaviorEventsHandler);
                }
            }
        }
        finally
        {
            _isSynchronizingCollection = wasSynchronizingCollection;
            if (!wasSynchronizingCollection && _pendingSynchronizations.Count > 0)
            {
                var pending = _pendingSynchronizations.ToList();
                _pendingSynchronizations.Clear();
                SynchronizeBehaviorEvents(pending);
            }
        }
    }

#if UNO
    private void BehaviorCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
    {
        var eventIndex = (int)eventArgs.Index;

        switch (eventArgs.CollectionChange)
        {
            case CollectionChange.Reset:
                OnItemsReset();
                break;
            case CollectionChange.ItemInserted:
                OnItemAdded(eventIndex, this[eventIndex]);
                break;
            case CollectionChange.ItemChanged:
                OnItemReplaced(eventIndex, this[eventIndex]);
                break;
            case CollectionChange.ItemRemoved:
                OnItemRemoved(eventIndex);
                break;
            default:
                Debug.Assert(false, "Unsupported collection operation attempted.");
                break;
        }
#if DEBUG
        VerifyOldCollectionIntegrity();
#endif
    }
#else
    private void BehaviorCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                OnItemsReset();
                break;

            case NotifyCollectionChangedAction.Add:
                OnItemAdded(eventArgs.NewStartingIndex, eventArgs.NewItems?[0] as AvaloniaObject);
                break;

            case NotifyCollectionChangedAction.Replace:
            {
                var eventIndex = eventArgs.OldStartingIndex;
                eventIndex = eventIndex == -1 ? 0 : eventIndex;
                OnItemReplaced(eventIndex, eventArgs.NewItems?[0] as AvaloniaObject);
                break;
            }

            case NotifyCollectionChangedAction.Remove:
                OnItemRemoved(eventArgs.OldStartingIndex);
                break;

            case NotifyCollectionChangedAction.Move:
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
#endif

    private void OnItemsReset()
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
    }

    private void OnItemAdded(int eventIndex, AvaloniaObject? changedItem)
    {
        var behavior = VerifiedAttach(changedItem);
        _oldCollection.Insert(eventIndex, behavior);
        if (!_isAttachingCollection)
        {
            QueueOrSynchronizeBehaviorEvents(behavior);
        }
    }

    private void OnItemReplaced(int eventIndex, AvaloniaObject? changedItem)
    {
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
    }

    private void OnItemRemoved(int eventIndex)
    {
        var oldItem = _oldCollection[eventIndex];
        if (oldItem.AssociatedObject is not null)
        {
            oldItem.Detach();
        }

        _oldCollection.RemoveAt(eventIndex);
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
        if (_isSynchronizingCollection || HasPendingHostLifecycleEvent())
        {
            QueuePendingSynchronization(behavior);
            return;
        }

        SynchronizeBehaviorEvents([behavior]);
    }

    private void QueueOrSynchronizeBehaviorEvents(IReadOnlyList<IBehavior> behaviors)
    {
        if (_isSynchronizingCollection || HasPendingHostLifecycleEvent())
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

    private bool HasPendingHostLifecycleEvent()
    {
        var associatedObject = AssociatedObject;
        if (associatedObject is null)
        {
            return false;
        }

        if (!_hasObservedInitialized && IsHostInitialized(associatedObject))
        {
            return true;
        }

        if (!_hasObservedLogicalAttachment && IsHostAttachedToLogicalTree(associatedObject))
        {
            return true;
        }

        if (!_hasObservedVisualAttachment && IsHostAttachedToVisualTree(associatedObject))
        {
            return true;
        }

        return !_hasObservedLoaded && IsHostLoaded(associatedObject);
    }

    private void CaptureCurrentLifecycleState()
    {
        var associatedObject = AssociatedObject;
        _hasObservedInitialized = IsHostInitialized(associatedObject);
        _hasObservedLogicalAttachment = IsHostAttachedToLogicalTree(associatedObject);
        _hasObservedVisualAttachment = IsHostAttachedToVisualTree(associatedObject);
        _hasObservedLoaded = IsHostLoaded(associatedObject);
    }

    private void SynchronizeBehaviorEvents(IReadOnlyList<IBehavior> behaviors)
    {
        _isSynchronizingCollection = true;
        try
        {
            var currentBatch = behaviors.ToList();
            for (var phase = 0; phase < 4; phase++)
            {
                for (var index = 0; index < currentBatch.Count; index++)
                {
                    SynchronizePhase(currentBatch[index], phase);
                    DrainPendingSynchronizations(currentBatch, phase);
                }
            }
        }
        finally
        {
            _isSynchronizingCollection = false;
            _pendingSynchronizations.Clear();
        }
    }

    private void DrainPendingSynchronizations(List<IBehavior> currentBatch, int currentPhase)
    {
        var catchUpBatch = new List<(IBehavior Behavior, int NextPhase)>();
        while (true)
        {
            if (_pendingSynchronizations.Count > 0)
            {
                var pending = _pendingSynchronizations.ToList();
                _pendingSynchronizations.Clear();
                foreach (var behavior in pending)
                {
                    if (!currentBatch.Contains(behavior))
                    {
                        currentBatch.Add(behavior);
                    }

                    var isAlreadyQueued = false;
                    for (var index = 0; index < catchUpBatch.Count; index++)
                    {
                        if (ReferenceEquals(catchUpBatch[index].Behavior, behavior))
                        {
                            isAlreadyQueued = true;
                            break;
                        }
                    }

                    if (!isAlreadyQueued)
                    {
                        catchUpBatch.Add((behavior, 0));
                    }
                }
            }

            var nextIndex = -1;
            var nextPhase = int.MaxValue;
            for (var index = 0; index < catchUpBatch.Count; index++)
            {
                var candidatePhase = catchUpBatch[index].NextPhase;
                if (candidatePhase <= currentPhase && candidatePhase < nextPhase)
                {
                    nextIndex = index;
                    nextPhase = candidatePhase;
                }
            }

            if (nextIndex < 0)
            {
                return;
            }

            var next = catchUpBatch[nextIndex];
            SynchronizePhase(next.Behavior, next.NextPhase);
            catchUpBatch[nextIndex] = (next.Behavior, next.NextPhase + 1);
        }
    }

    private static void SynchronizePhase(IBehavior behavior, int phase)
    {
        switch (phase)
        {
            case 0:
                SynchronizeInitialized(behavior);
                break;
            case 1:
                SynchronizeLogicalAttachment(behavior);
                break;
            case 2:
                SynchronizeVisualAttachment(behavior);
                break;
            case 3:
                SynchronizeLoaded(behavior);
                break;
        }
    }

    private static void SynchronizeInitialized(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            IsHostInitialized(behavior.AssociatedObject))
        {
            eventsHandler.InitializedEventHandler();
        }
    }

    private static void SynchronizeLogicalAttachment(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            IsHostAttachedToLogicalTree(behavior.AssociatedObject))
        {
            eventsHandler.AttachedToLogicalTreeEventHandler();
        }
    }

    private static void SynchronizeVisualAttachment(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            IsHostAttachedToVisualTree(behavior.AssociatedObject))
        {
            eventsHandler.AttachedToVisualTreeEventHandler();
        }
    }

    private static void SynchronizeLoaded(IBehavior behavior)
    {
        if (behavior is IBehaviorEventsHandler eventsHandler &&
            IsHostLoaded(behavior.AssociatedObject))
        {
            eventsHandler.LoadedEventHandler();
        }
    }

#if UNO
    // WinUI has no initialization, logical tree or visual tree attachment notifications distinct from
    // Loaded/Unloaded; Interaction raises all of them from FrameworkElement.Loaded/Unloaded.
    private static bool IsHostInitialized(AvaloniaObject? associatedObject)
        => associatedObject is FrameworkElement { IsLoaded: true };

    private static bool IsHostAttachedToLogicalTree(AvaloniaObject? associatedObject)
        => associatedObject is FrameworkElement { IsLoaded: true };

    private static bool IsHostAttachedToVisualTree(AvaloniaObject? associatedObject)
        => associatedObject is FrameworkElement { IsLoaded: true };

    private static bool IsHostLoaded(AvaloniaObject? associatedObject)
        => associatedObject is FrameworkElement { IsLoaded: true };
#else
    private static bool IsHostInitialized(AvaloniaObject? associatedObject)
        => associatedObject is StyledElement { IsInitialized: true };

    private static bool IsHostAttachedToLogicalTree(AvaloniaObject? associatedObject)
        => associatedObject is StyledElement styledElement &&
           (((ILogical)styledElement).IsAttachedToLogicalTree || IsOpenTopLevel(associatedObject));

    private static bool IsHostAttachedToVisualTree(AvaloniaObject? associatedObject)
        => associatedObject is Visual visual &&
           (visual.IsAttachedToVisualTree() || IsOpenTopLevel(associatedObject));

    private static bool IsHostLoaded(AvaloniaObject? associatedObject)
        => associatedObject is Control { IsLoaded: true };

    private static bool IsOpenTopLevel(AvaloniaObject? associatedObject)
        => associatedObject is TopLevel { IsLoaded: true };
#endif

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
