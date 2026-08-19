// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// A behavior that performs actions when the bound data meets a specified condition.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class DataTriggerBehavior : StyledElementTrigger
{
    private bool _isConditionMet;
    private bool _hasConditionState;
    private readonly List<IReversibleAction> _appliedActions = [];
    private ActionCollection? _subscribedActions;

    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<DataTriggerBehavior, object?>(nameof(Binding));

    /// <summary>
    /// Identifies the <seealso cref="ComparisonCondition"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> ComparisonConditionProperty =
        AvaloniaProperty.Register<DataTriggerBehavior, ComparisonConditionType>(nameof(ComparisonCondition));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<DataTriggerBehavior, object?>(nameof(Value));

    /// <summary>
    /// Identifies the <seealso cref="RevertOnFalse"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> RevertOnFalseProperty =
        AvaloniaProperty.Register<DataTriggerBehavior, bool>(nameof(RevertOnFalse), defaultValue: false);

    /// <summary>
    /// Gets or sets the bound object that the <see cref="DataTriggerBehavior"/> will listen to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of comparison to be performed between <see cref="DataTriggerBehavior.Binding"/> and <see cref="DataTriggerBehavior.Value"/>. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType ComparisonCondition
    {
        get => GetValue(ComparisonConditionProperty);
        set => SetValue(ComparisonConditionProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to be compared with the value of <see cref="DataTriggerBehavior.Binding"/>. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether reversible actions should be reverted when the condition becomes false.
    /// When false, behavior matches legacy semantics and only executes actions when the condition is true.
    /// </summary>
    public bool RevertOnFalse
    {
        get => GetValue(RevertOnFalseProperty);
        set => SetValue(RevertOnFalseProperty, value);
    }

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
        if (args.Sender is not DataTriggerBehavior behavior)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            behavior.Execute(parameter: args);
        });
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
