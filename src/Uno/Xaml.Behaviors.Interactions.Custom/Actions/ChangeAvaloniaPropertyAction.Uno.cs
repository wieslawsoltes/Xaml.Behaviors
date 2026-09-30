// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform property type lookup.
/// </content>
public partial class ChangeAvaloniaPropertyAction
{
    /// <summary>
    /// Gets the value type of a dependency property.
    /// </summary>
    /// <remarks>
    /// WinUI dependency properties expose neither their type nor their name; the type of the default value is used
    /// (value types), otherwise <see cref="object"/> and the value is assigned as is.
    /// </remarks>
    /// <param name="targetObject">The object owning the value.</param>
    /// <param name="targetProperty">The property.</param>
    /// <returns>The property value type.</returns>
    private static Type GetPropertyType(DependencyObject targetObject, DependencyProperty targetProperty)
    {
        var defaultValue = targetProperty.GetMetadata(targetObject.GetType())?.DefaultValue;
        return defaultValue is null || defaultValue == DependencyProperty.UnsetValue
            ? typeof(object)
            : defaultValue.GetType();
    }
}
