// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Sets and clears temporary values that override the local value or binding of a property without replacing it, like
/// the Avalonia animation values.
/// </summary>
/// <remarks>
/// Uno Platform stores them with the animation value precedence. Native WinUI has no public value precedences: the
/// WinUI port saves the local value (or binding) when the first temporary value is set and restores it when the
/// temporary value is cleared (see src/WinUI/Xaml.Behaviors.Interactivity/Compat/AnimationValueLayer.cs).
/// </remarks>
internal static class AnimationValueCompat
{
    /// <summary>
    /// Sets the temporary value of a property.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    /// <param name="value">The temporary value.</param>
    public static void SetAnimationValue(this DependencyObject dependencyObject, DependencyProperty property, object? value)
    {
#if WINUI
        AnimationValueLayer.SetValue(dependencyObject, property, value);
#else
        dependencyObject.SetValue(property, value, DependencyPropertyValuePrecedences.Animations);
#endif
    }

    /// <summary>
    /// Clears the temporary value of a property, which makes its local value or binding effective again.
    /// </summary>
    /// <param name="dependencyObject">The object.</param>
    /// <param name="property">The property.</param>
    public static void ClearAnimationValue(this DependencyObject dependencyObject, DependencyProperty property)
    {
#if WINUI
        AnimationValueLayer.ClearValue(dependencyObject, property);
#else
        dependencyObject.SetValue(property, DependencyProperty.UnsetValue, DependencyPropertyValuePrecedences.Animations);
#endif
    }
}
