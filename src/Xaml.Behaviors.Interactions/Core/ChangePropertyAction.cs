// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// An action that will change a specified property to a specified value when invoked.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class ChangePropertyAction : StyledElementAction, IReversibleAction
{
    private ReversiblePropertyChange? _reversibleChange;
    private object? _appliedTarget;
    private string? _appliedPropertyName;
    private bool _preserveValueSource;

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the value to set. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <summary>
    /// Gets or sets the object whose property will be changed.
    /// If <seealso cref="TargetObject"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial object? TargetObject { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if updating the property value succeeds; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        return ExecuteCore(sender, preserveValueSource: false);
    }

    /// <inheritdoc />
    public object? ExecuteReversibly(object? sender, object? parameter)
    {
        return ExecuteCore(sender, preserveValueSource: true);
    }

    private object ExecuteCore(object? sender, bool preserveValueSource)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var targetObject = GetValue(TargetObjectProperty) is not null ? TargetObject : sender;
        if (targetObject is null)
        {
            return false;
        }

        var propertyName = PropertyName;
        if (propertyName is null)
        {
            return false;
        }

        if (!preserveValueSource)
        {
            return PropertyHelper.UpdatePropertyValue(
                targetObject,
                propertyName,
                Value,
                preserveValueSource: false);
        }

        if (_reversibleChange is not null &&
            (!ReferenceEquals(_appliedTarget, targetObject) ||
             !string.Equals(_appliedPropertyName, propertyName, StringComparison.Ordinal)))
        {
            if (!_reversibleChange.Revert())
            {
                return false;
            }

            ClearAppliedState();
        }

        if (_reversibleChange is null)
        {
            _reversibleChange = new ReversiblePropertyChange(propertyName);
        }

        var applied = _reversibleChange.Apply(
            targetObject,
            Value,
            TryGetValue,
            SetValue,
            SetTemporaryValue,
            PropertyHelper.IsDirectAvaloniaProperty(targetObject, propertyName));
        if (applied)
        {
            _appliedTarget = targetObject;
            _appliedPropertyName = propertyName;
        }

        return applied;

        bool TryGetValue(out object? value)
        {
            var found = PropertyHelper.TryGetPropertyValue(
                targetObject,
                propertyName,
                out value,
                out var capturedPreserveValueSource);
            if (found)
            {
                _preserveValueSource = capturedPreserveValueSource;
            }

            return found;
        }

        bool SetValue(object? value)
        {
            return PropertyHelper.UpdatePropertyValue(
                targetObject,
                propertyName,
                value,
                _preserveValueSource);
        }

        bool SetTemporaryValue(object? value, out IDisposable? reversion)
        {
            return PropertyHelper.TrySetTemporaryAvaloniaPropertyValue(
                targetObject,
                propertyName,
                value,
                out reversion);
        }
    }

    /// <summary>
    /// Reverts the property value captured when this action was first applied.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if reverting the property value succeeds; else false.</returns>
    public object Revert(object? sender, object? parameter)
    {
        if (_reversibleChange is null || !_reversibleChange.Revert())
        {
            return false;
        }

        ClearAppliedState();
        return true;
    }

    private void ClearAppliedState()
    {
        _reversibleChange = null;
        _appliedTarget = null;
        _appliedPropertyName = null;
        _preserveValueSource = false;
    }
}
