// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// A behavior that performs actions when the bound data meets a specified condition.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class DataTriggerBehavior : StyledElementTrigger
{
    private bool _isConditionMet;
    private bool _hasConditionState;
    private readonly List<IReversibleAction> _appliedActions = [];
    private ActionCollection? _subscribedActions;

    /// <summary>
    /// Gets or sets the bound object that the <see cref="DataTriggerBehavior"/> will listen to. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Binding { get; set; }

    /// <summary>
    /// Gets or sets the type of comparison to be performed between <see cref="DataTriggerBehavior.Binding"/> and <see cref="DataTriggerBehavior.Value"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ComparisonConditionType ComparisonCondition { get; set; }

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="DataTriggerBehavior.Binding"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reversible actions should be reverted when the condition becomes false.
    /// When false, behavior matches legacy semantics and only executes actions when the condition is true.
    /// </summary>
    [StyledProperty(DefaultValue = false)]
    public partial bool RevertOnFalse { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == BindingProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == ComparisonConditionProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == ValueProperty)
        {
            OnValueChanged(change);
        }

        if (change.Property == RevertOnFalseProperty)
        {
            if (change.GetOldValue<bool>() && !change.GetNewValue<bool>())
            {
                RevertActions(change);
            }

            _hasConditionState = false;
            OnValueChanged(change);
        }

        if (change.Property == IsEnabledProperty && RevertOnFalse)
        {
            var isEnabled = change.GetNewValue<bool>();
            if (!isEnabled)
            {
                RevertActions(change);
            }

            _hasConditionState = false;
            if (isEnabled)
            {
                OnValueChanged(change);
            }
        }

        if (change.Property == ActionsProperty && AssociatedObject is not null)
        {
            UpdateActionSubscription(change.GetNewValue<ActionCollection?>());
            if (RevertOnFalse)
            {
                RevertActions(change);
                _hasConditionState = false;
                OnValueChanged(change);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnInitializedEvent()
    {
        base.OnInitializedEvent();

        Execute(parameter: null);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateActionSubscription(Actions);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (RevertOnFalse)
        {
            RevertActions(parameter: null);
        }

        _hasConditionState = false;
        UpdateActionSubscription(actions: null);
        base.OnDetaching();
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // Property changes of this behavior are always raised on this instance.
        Dispatcher.UIThread.Post(() =>
        {
            Execute(parameter: args);
        });
    }

    private void Execute(object? parameter)
    {        
        if (AssociatedObject is null)
        {
            return;
        }

#if UNO
        // The bindings of the behavior resolve through the data context inherited when the associated object enters
        // the tree: evaluate from the initialized event (raised when it is loaded), not from changes queued before.
        if (!IsInitializedNotified)
        {
            return;
        }
#endif

        if (!IsEnabled)
        {
            return;
        }

        var binding = Binding;
        if (!IsSet(BindingProperty) || Equals(binding, AvaloniaProperty.UnsetValue))
        {
            return;
        }

        if (binding is null &&
            ComparisonCondition is not ComparisonConditionType.Equal and
            not ComparisonConditionType.NotEqual)
        {
            return;
        }

        if (!RevertOnFalse)
        {
            // Preserve legacy behavior: execute whenever condition evaluates true.
            if (ComparisonConditionTypeHelper.Compare(binding, ComparisonCondition, Value))
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
            }

            return;
        }

        var isConditionMet = ComparisonConditionTypeHelper.Compare(binding, ComparisonCondition, Value);

        if (!_hasConditionState)
        {
            _hasConditionState = true;
            _isConditionMet = isConditionMet;

            if (isConditionMet)
            {
                ApplyActions(parameter);
            }

            return;
        }

        if (_isConditionMet == isConditionMet)
        {
            return;
        }

        _isConditionMet = isConditionMet;

        if (isConditionMet)
        {
            ApplyActions(parameter);
            return;
        }

        RevertActions(parameter);
    }

    private void RevertActions(object? parameter)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        for (var index = _appliedActions.Count - 1; index >= 0; index--)
        {
            _appliedActions[index].Revert(AssociatedObject, parameter);
        }

        _appliedActions.Clear();
    }

    private void ApplyActions(object? parameter)
    {
        _appliedActions.Clear();
        _appliedActions.AddRange(ReversibleActionExecution.Execute(AssociatedObject, Actions, parameter));
    }

    private void UpdateActionSubscription(ActionCollection? actions)
    {
        if (_subscribedActions is not null)
        {
            _subscribedActions.CollectionChanged -= ActionsCollectionChanged;
        }

        _subscribedActions = actions;
        if (_subscribedActions is not null)
        {
            _subscribedActions.CollectionChanged += ActionsCollectionChanged;
        }
    }

    private void ActionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        if (!RevertOnFalse || AssociatedObject is null)
        {
            return;
        }

        RevertActions(eventArgs);
        _hasConditionState = false;
        Dispatcher.UIThread.Post(() => Execute(eventArgs));
    }
}
