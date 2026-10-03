// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Xaml.Interactivity;

/// <summary>
/// Native WinUI emulation of the animation value precedence of Uno Platform (see <see cref="AnimationValueCompat"/>).
/// </summary>
/// <remarks>
/// The first temporary value of a property saves its local value, or the binding that sets it, and replaces it; the
/// following ones replace the effective value. Clearing restores the saved binding or local value (or clears the
/// property when it had none). A different local value set while a temporary value is effective becomes the value
/// that is restored, and the temporary value stays effective (a local value equal to the temporary value cannot be
/// observed).
/// </remarks>
internal static class AnimationValueLayer
{
    private static readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, Entry>> s_entries = new();

    /// <summary>
    /// Sets the temporary value of a property.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">The temporary value.</param>
    public static void SetValue(DependencyObject dependencyObject, DependencyProperty property, object? value)
    {
        var entries = s_entries.GetOrCreateValue(dependencyObject);
        if (!entries.TryGetValue(property, out var entry))
        {
            entry = new Entry(BaseValue.Read(dependencyObject, property));
            entries.Add(property, entry);

            // A local value set on a property with a two-way binding updates the binding source instead of replacing
            // the binding: remove the binding (it is restored when the temporary value is cleared).
            if (entry.Base.HasBinding)
            {
                dependencyObject.ClearValue(property);
            }

            entry.Token = dependencyObject.RegisterPropertyChangedCallback(property, (sender, changed) => OnValueChanged(sender, changed, entry));
        }

        entry.Apply(dependencyObject, property, value);
    }

    /// <summary>
    /// Clears the temporary value of a property and restores its local value or binding.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    public static void ClearValue(DependencyObject dependencyObject, DependencyProperty property)
    {
        if (!s_entries.TryGetValue(dependencyObject, out var entries) ||
            !entries.Remove(property, out var entry))
        {
            return;
        }

        if (entries.Count == 0)
        {
            s_entries.Remove(dependencyObject);
        }

        dependencyObject.UnregisterPropertyChangedCallback(property, entry.Token);
        entry.Base.Restore(dependencyObject, property);
    }

    private static void OnValueChanged(DependencyObject sender, DependencyProperty property, Entry entry)
    {
        if (entry.IsApplying)
        {
            return;
        }

        // A local value replaced the temporary value: it is the value to restore, and the temporary value stays.
        object? current = sender.GetValue(property);
        if (!Equals(current, entry.Value))
        {
            entry.Base = BaseValue.FromLocalValue(current);
            entry.Apply(sender, property, entry.Value);
        }
    }

    private sealed class Entry(BaseValue baseValue)
    {
        public BaseValue Base { get; set; } = baseValue;

        public object? Value { get; private set; }

        public long Token { get; set; }

        public bool IsApplying { get; private set; }

        public void Apply(DependencyObject dependencyObject, DependencyProperty property, object? value)
        {
            Value = value;
            IsApplying = true;
            try
            {
                dependencyObject.SetValue(property, value);
            }
            finally
            {
                IsApplying = false;
            }
        }
    }

    private readonly struct BaseValue
    {
        private readonly object? _localValue;
        private readonly BindingBase? _binding;

        private BaseValue(object? localValue, BindingBase? binding)
        {
            _localValue = localValue;
            _binding = binding;
        }

        public bool HasBinding => _binding is not null;

        public static BaseValue FromLocalValue(object? value) => new(value, null);

        public static BaseValue Read(DependencyObject dependencyObject, DependencyProperty property)
        {
            if (dependencyObject is FrameworkElement element && element.GetBindingExpression(property) is { } expression)
            {
                return new BaseValue(DependencyProperty.UnsetValue, expression.ParentBinding);
            }

            return new BaseValue(dependencyObject.ReadLocalValue(property), null);
        }

        public void Restore(DependencyObject dependencyObject, DependencyProperty property)
        {
            if (_binding is not null && dependencyObject is FrameworkElement element)
            {
                element.SetBinding(property, _binding);
            }
            else if (ReferenceEquals(_localValue, DependencyProperty.UnsetValue))
            {
                dependencyObject.ClearValue(property);
            }
            else
            {
                dependencyObject.SetValue(property, _localValue);
            }
        }
    }
}
