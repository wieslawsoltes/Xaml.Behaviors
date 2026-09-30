// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia object helpers used by the shared sources.
/// </summary>
internal static class AvaloniaObjectCompatExtensions
{
    /// <summary>
    /// Gets the new value of a property change.
    /// </summary>
    public static T GetNewValue<T>(this DependencyPropertyChangedEventArgs e) => e.NewValue is T value ? value : default!;

    /// <summary>
    /// Gets the old value of a property change.
    /// </summary>
    public static T GetOldValue<T>(this DependencyPropertyChangedEventArgs e) => e.OldValue is T value ? value : default!;

    /// <summary>
    /// Checks whether a local value is set for the property.
    /// </summary>
    public static bool IsSet(this DependencyObject o, DependencyProperty property)
        => o.ReadLocalValue(property) != DependencyProperty.UnsetValue;

    /// <summary>
    /// Sets the value of a property without replacing its two-way bindings.
    /// </summary>
    /// <remarks>
    /// WinUI has no <c>SetCurrentValue</c>; a local value is set. Two-way bindings propagate the value,
    /// one-way bindings are replaced.
    /// </remarks>
    public static void SetCurrentValue(this DependencyObject o, DependencyProperty property, object? value)
        => o.SetValue(property, value);

    /// <summary>
    /// Updates the backing field of a direct property and publishes the value to the dependency property.
    /// </summary>
    /// <returns><c>true</c> when the value changed.</returns>
    public static bool SetAndRaise<T>(this DependencyObject o, DependencyProperty property, ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        o.SetValue(property, value);
        return true;
    }
}
