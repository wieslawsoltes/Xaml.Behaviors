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
/// property when it had none). A local value set while a temporary value is effective is lost when it is cleared.
/// </remarks>
internal static class AnimationValueLayer
{
    private static readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, BaseValue>> s_baseValues = new();

    /// <summary>
    /// Sets the temporary value of a property.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">The temporary value.</param>
    public static void SetValue(DependencyObject dependencyObject, DependencyProperty property, object? value)
    {
        var baseValues = s_baseValues.GetOrCreateValue(dependencyObject);
        if (!baseValues.ContainsKey(property))
        {
            baseValues.Add(property, BaseValue.Read(dependencyObject, property));
        }

        dependencyObject.SetValue(property, value);
    }

    /// <summary>
    /// Clears the temporary value of a property and restores its local value or binding.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    public static void ClearValue(DependencyObject dependencyObject, DependencyProperty property)
    {
        if (!s_baseValues.TryGetValue(dependencyObject, out var baseValues) ||
            !baseValues.Remove(property, out var baseValue))
        {
            return;
        }

        if (baseValues.Count == 0)
        {
            s_baseValues.Remove(dependencyObject);
        }

        baseValue.Restore(dependencyObject, property);
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
