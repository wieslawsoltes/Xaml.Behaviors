// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A behavior that performs actions when all bound data conditions are satisfied.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class MultiDataTriggerBehavior : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Conditions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ConditionCollection?> ConditionsProperty =
        AvaloniaProperty.Register<MultiDataTriggerBehavior, ConditionCollection?>(nameof(Conditions));

    private readonly HashSet<Condition> _subscribedConditions = [];
    private readonly Dictionary<Condition, IDisposable?> _propertySubscriptions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiDataTriggerBehavior"/> class.
    /// </summary>
    public MultiDataTriggerBehavior()
    {
        var defaultConditions = new ConditionCollection();
        SetCurrentValue(ConditionsProperty, defaultConditions);
        AttachConditions(defaultConditions);
    }

    /// <summary>
    /// Gets or sets the collection of conditions that must all be satisfied before actions are executed. This is an avalonia property.
    /// </summary>
    public ConditionCollection? Conditions
    {
        get => GetValue(ConditionsProperty);
        set => SetValue(ConditionsProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ConditionsProperty)
        {
            if (change.GetOldValue<ConditionCollection?>() is { } oldConditions)
            {
                DetachConditions(oldConditions);
            }

            if (change.GetNewValue<ConditionCollection?>() is { } newConditions)
            {
                AttachConditions(newConditions);
            }

            ScheduleExecute(change);
        }
    }

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();

        Execute(parameter: null);
        RefreshConditionSubscriptions();
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree()
    {
        base.OnAttachedToLogicalTree();

        RefreshConditionSubscriptions();
    }

    /// <inheritdoc />
    protected override void OnLoaded()
    {
        base.OnLoaded();

        RefreshConditionSubscriptions();
    }

    private void AttachConditions(ConditionCollection? conditions)
    {
        if (conditions is null)
        {
            return;
        }

        conditions.CollectionChanged += ConditionsCollectionChanged;

        foreach (var condition in conditions)
        {
            AttachCondition(condition);
        }
    }

    private void DetachConditions(ConditionCollection? conditions)
    {
        if (conditions is null)
        {
            return;
        }

        conditions.CollectionChanged -= ConditionsCollectionChanged;

        DetachSubscribedConditions();
    }

    private void ConditionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
            {
                DetachSubscribedConditions();

                if (sender is ConditionCollection collection)
                {
                    foreach (var condition in collection)
                    {
                        AttachCondition(condition);
                    }
                }

                break;
            }

            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
            {
                if (e.OldItems is not null)
                {
                    foreach (Condition condition in e.OldItems)
                    {
                        DetachCondition(condition);
                    }
                }

                goto case NotifyCollectionChangedAction.Add;
            }

            case NotifyCollectionChangedAction.Add:
            {
                if (e.NewItems is not null)
                {
                    foreach (Condition condition in e.NewItems)
                    {
                        AttachCondition(condition);
                    }
                }

                break;
            }

            case NotifyCollectionChangedAction.Move:
            default:
                break;
        }

        ScheduleExecute(parameter: null);
    }

    private void AttachCondition(Condition condition)
    {
        if (_subscribedConditions.Add(condition))
        {
            condition.PropertyChanged += ConditionPropertyChanged;
            UpdateConditionSubscription(condition);
        }
    }

    private void DetachCondition(Condition condition)
    {
        if (_subscribedConditions.Remove(condition))
        {
            condition.PropertyChanged -= ConditionPropertyChanged;
            DisposePropertySubscription(condition);
        }
    }

    private void ConditionPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Condition.BindingValueProperty ||
            e.Property == Condition.ValueProperty ||
            e.Property == Condition.ComparisonConditionProperty)
        {
            ScheduleExecute(e);
            return;
        }

        if (e.Property == Condition.PropertyProperty ||
            e.Property == Condition.SourceNameProperty)
        {
            UpdateConditionSubscription((Condition)sender!);
            ScheduleExecute(e);
        }
    }

    private void ScheduleExecute(object? parameter)
    {
        Dispatcher.UIThread.Post(() => Execute(parameter));
    }

    private void UpdateConditionSubscription(Condition condition)
    {
        DisposePropertySubscription(condition);

        if (condition.Property is null)
        {
            return;
        }

        var target = ResolveConditionTarget(condition);
        if (target is null)
        {
            return;
        }

        var observable = target.GetObservable(condition.Property);
        _propertySubscriptions[condition] =
            observable.Subscribe(new AnonymousObserver<object?>(_ => ScheduleExecute(null)));
    }

    private AvaloniaObject? ResolveConditionTarget(Condition condition)
    {
        if (!string.IsNullOrEmpty(condition.SourceName))
        {
            var sourceName = condition.SourceName!;

            var namedTarget = FindInNameScope(AssociatedObject, sourceName) ??
                              FindInNameScope(AssociatedStyledElement, sourceName);
            if (namedTarget is not null)
            {
                return namedTarget;
            }

            if (AssociatedObject is ILogical logical)
            {
                var logicalTarget = logical.Find<AvaloniaObject>(sourceName);
                if (logicalTarget is not null)
                {
                    return logicalTarget;
                }
            }

            if (AssociatedStyledElement is ILogical styledLogical)
            {
                return styledLogical.Find<AvaloniaObject>(sourceName);
            }

            return null;
        }

        return AssociatedObject;
    }

    private static AvaloniaObject? FindInNameScope(AvaloniaObject? source, string sourceName)
    {
        if (source is not ILogical logicalSource)
        {
            return null;
        }

        foreach (var logical in logicalSource.GetSelfAndLogicalAncestors())
        {
            if (logical is StyledElement styledElement)
            {
                var nameScope = NameScope.GetNameScope(styledElement);
                if (nameScope?.Find(sourceName) is AvaloniaObject found)
                {
                    return found;
                }
            }
        }

        return null;
    }

    private void RefreshConditionSubscriptions()
    {
        foreach (var condition in SnapshotSubscribedConditions())
        {
            UpdateConditionSubscription(condition);
        }
    }

    private void DetachSubscribedConditions()
    {
        foreach (var condition in SnapshotSubscribedConditions())
        {
            DetachCondition(condition);
        }
    }

    private void DisposePropertySubscription(Condition condition)
    {
        if (_propertySubscriptions.TryGetValue(condition, out var disposable))
        {
            disposable?.Dispose();
            _propertySubscriptions.Remove(condition);
        }
    }

    private IEnumerable<Condition> SnapshotSubscribedConditions()
    {
        if (_subscribedConditions.Count == 0)
        {
            return Array.Empty<Condition>();
        }

        var snapshot = new Condition[_subscribedConditions.Count];
        _subscribedConditions.CopyTo(snapshot);
        return snapshot;
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (!IsEnabled)
        {
            return;
        }

        var conditions = Conditions;
        if (conditions is null || conditions.Count == 0)
        {
            return;
        }

        foreach (var condition in conditions)
        {
            if (!TryGetConditionValue(condition, out var value))
            {
                return;
            }

            if (!ComparisonConditionTypeHelper.Compare(value, condition.ComparisonCondition, condition.Value))
            {
                return;
            }
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }

    private bool TryGetConditionValue(Condition condition, out object? value)
    {
        value = null;

        if (condition.Property is not null)
        {
            var target = ResolveConditionTarget(condition);
            if (target is null)
            {
                return false;
            }

            value = target.GetValue(condition.Property);
            if (Equals(value, AvaloniaProperty.UnsetValue))
            {
                value = null;
                return false;
            }

            return true;
        }

        if (condition.Binding is not null)
        {
            if (!condition.IsSet(Condition.BindingValueProperty))
            {
                return false;
            }

            value = condition.BindingValue;
            if (Equals(value, AvaloniaProperty.UnsetValue))
            {
                value = null;
                return false;
            }

            return true;
        }

        value = condition.BindingValue;
        return !Equals(value, AvaloniaProperty.UnsetValue);
    }
}
