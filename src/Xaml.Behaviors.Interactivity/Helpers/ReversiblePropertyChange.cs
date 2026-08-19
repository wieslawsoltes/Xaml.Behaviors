// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia.Data;

namespace Avalonia.Xaml.Interactivity;

internal sealed class ReversiblePropertyChange
{
    internal delegate bool TryGetValue(out object? value);

    internal delegate bool TrySetTemporaryValue(object? value, out IDisposable? reversion);

    private sealed class PropertyFrame(
        object? value,
        Func<object?, bool> setter,
        TrySetTemporaryValue temporarySetter,
        IDisposable? temporaryValue)
    {
        public object? Value { get; set; } = value;
        public Func<object?, bool> Setter { get; set; } = setter;
        public TrySetTemporaryValue TemporarySetter { get; set; } = temporarySetter;
        public IDisposable? TemporaryValue { get; set; } = temporaryValue;
    }

    private sealed class PropertyStack(
        object? originalValue,
        Func<object?, bool> originalSetter,
        bool useTemporaryValues)
    {
        public object? OriginalValue { get; } = originalValue;
        public Func<object?, bool> OriginalSetter { get; } = originalSetter;
        public bool UseTemporaryValues { get; } = useTemporaryValues;
        public List<PropertyFrame> Frames { get; } = [];
    }

    private static readonly ConditionalWeakTable<object, Dictionary<string, PropertyStack>> s_propertyStacks = new();

    private readonly string _propertyName;
    private object? _target;
    private PropertyFrame? _frame;

    public ReversiblePropertyChange(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        _propertyName = propertyName;
    }

    public bool Apply(
        object target,
        object? value,
        TryGetValue getter,
        Func<object?, bool> setter,
        TrySetTemporaryValue temporarySetter,
        bool isDirectProperty)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(temporarySetter);

        if (isDirectProperty)
        {
            return false;
        }

        if (_frame is not null && ReferenceEquals(_target, target))
        {
            if (TryGetStack(target, out var existingStack))
            {
                var frameIndex = existingStack.Frames.IndexOf(_frame);
                if (frameIndex >= 0)
                {
                    if (frameIndex == existingStack.Frames.Count - 1 &&
                        !TryApplyValue(existingStack, _frame, value, setter, temporarySetter))
                    {
                        return false;
                    }

                    _frame.Value = value;
                    _frame.Setter = setter;
                    _frame.TemporarySetter = temporarySetter;
                    return true;
                }
            }

            ClearState();
        }
        else if (_frame is not null && !Revert())
        {
            return false;
        }

        var propertyStacks = s_propertyStacks.GetOrCreateValue(target);
        var isNewStack = !propertyStacks.TryGetValue(_propertyName, out var propertyStack);
        IDisposable? temporaryValue = null;

        if (isNewStack)
        {
            if (!getter(out var originalValue))
            {
                if (propertyStacks.Count == 0)
                {
                    s_propertyStacks.Remove(target);
                }

                return false;
            }

            var useTemporaryValues = temporarySetter(value, out temporaryValue);
            propertyStack = new PropertyStack(originalValue, setter, useTemporaryValues);
            if (!useTemporaryValues && !setter(value))
            {
                if (propertyStacks.Count == 0)
                {
                    s_propertyStacks.Remove(target);
                }

                return false;
            }
        }
        else if (propertyStack!.UseTemporaryValues)
        {
            if (!temporarySetter(value, out temporaryValue))
            {
                return false;
            }
        }
        else if (!setter(value))
        {
            return false;
        }

        var frame = new PropertyFrame(value, setter, temporarySetter, temporaryValue);
        propertyStack!.Frames.Add(frame);
        if (isNewStack)
        {
            propertyStacks.Add(_propertyName, propertyStack);
        }

        _target = target;
        _frame = frame;
        return true;
    }

    public bool Revert()
    {
        if (_target is null ||
            _frame is null ||
            !TryGetStack(_target, out var propertyStack))
        {
            return false;
        }

        var frameIndex = propertyStack.Frames.IndexOf(_frame);
        if (frameIndex < 0)
        {
            ClearState();
            return false;
        }

        var isTopFrame = frameIndex == propertyStack.Frames.Count - 1;
        if (propertyStack.UseTemporaryValues)
        {
            _frame.TemporaryValue?.Dispose();
            if (isTopFrame && frameIndex > 0)
            {
                var precedingFrame = propertyStack.Frames[frameIndex - 1];
                if (!precedingFrame.TemporarySetter(precedingFrame.Value, out var replacement))
                {
                    return false;
                }

                precedingFrame.TemporaryValue?.Dispose();
                precedingFrame.TemporaryValue = replacement;
            }
        }
        else if (isTopFrame)
        {
            var restored = frameIndex > 0
                ? propertyStack.Frames[frameIndex - 1].Setter(propertyStack.Frames[frameIndex - 1].Value)
                : propertyStack.OriginalSetter(propertyStack.OriginalValue);
            if (!restored)
            {
                return false;
            }
        }

        propertyStack.Frames.RemoveAt(frameIndex);
        if (propertyStack.Frames.Count == 0 &&
            s_propertyStacks.TryGetValue(_target, out var propertyStacks))
        {
            propertyStacks.Remove(_propertyName);
            if (propertyStacks.Count == 0)
            {
                s_propertyStacks.Remove(_target);
            }
        }

        ClearState();
        return true;
    }

    private static bool TryApplyValue(
        PropertyStack propertyStack,
        PropertyFrame frame,
        object? value,
        Func<object?, bool> setter,
        TrySetTemporaryValue temporarySetter)
    {
        if (!propertyStack.UseTemporaryValues)
        {
            return setter(value);
        }

        if (!temporarySetter(value, out var replacement))
        {
            return false;
        }

        frame.TemporaryValue?.Dispose();
        frame.TemporaryValue = replacement;
        return true;
    }

    private bool TryGetStack(object target, out PropertyStack propertyStack)
    {
        if (s_propertyStacks.TryGetValue(target, out var propertyStacks) &&
            propertyStacks.TryGetValue(_propertyName, out var foundStack))
        {
            propertyStack = foundStack;
            return true;
        }

        propertyStack = null!;
        return false;
    }

    private void ClearState()
    {
        _target = null;
        _frame = null;
    }
}

/// <summary>
/// Coordinates reversible, typed property changes across action instances.
/// </summary>
/// <typeparam name="TTarget">The target type.</typeparam>
/// <typeparam name="TValue">The property value type.</typeparam>
public sealed class ReversiblePropertyChange<TTarget, TValue>
{
    private readonly string _propertyName;
    private readonly ReversiblePropertyChange _change;

    /// <summary>
    /// Initializes a new reversible property-change coordinator.
    /// </summary>
    /// <param name="propertyName">The target property name.</param>
    public ReversiblePropertyChange(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        _propertyName = propertyName;
        _change = new ReversiblePropertyChange(propertyName);
    }

    /// <summary>
    /// Applies a value while preserving the state required to revert it.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="value">The value to apply.</param>
    /// <param name="getter">Gets the current property value.</param>
    /// <param name="setter">Sets the property value.</param>
    /// <returns><c>true</c> when the reversible value was applied; otherwise <c>false</c>.</returns>
    public bool Apply(
        TTarget target,
        TValue value,
        Func<TTarget, TValue> getter,
        Action<TTarget, TValue> setter)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);

        var targetObject = (object)target;
        var avaloniaObject = targetObject as AvaloniaObject;
        var avaloniaProperty = avaloniaObject is not null
            ? AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, _propertyName)
            : null;

        return _change.Apply(
            targetObject,
            value,
            TryGetValue,
            SetValue,
            SetTemporaryValue,
            avaloniaProperty?.IsDirect == true);

        bool TryGetValue(out object? currentValue)
        {
            currentValue = getter(target);
            return true;
        }

        bool SetValue(object? newValue)
        {
            setter(target, (TValue)newValue!);
            return true;
        }

        bool SetTemporaryValue(object? newValue, out IDisposable? reversion)
        {
            reversion = avaloniaProperty is null
                ? null
                : avaloniaObject!.SetValue(avaloniaProperty, newValue, BindingPriority.Animation);
            return reversion is not null;
        }
    }

    /// <summary>
    /// Reverts the value previously applied by this coordinator.
    /// </summary>
    /// <param name="setter">Sets the target property value.</param>
    /// <returns><c>true</c> when an applied value was reverted; otherwise <c>false</c>.</returns>
    public bool Revert(Action<TTarget, TValue> setter)
    {
        ArgumentNullException.ThrowIfNull(setter);
        return _change.Revert();
    }
}
