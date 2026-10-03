// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Xaml.Interactivity;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform property type lookup.
/// </content>
public partial class ChangeAvaloniaPropertyAction
{
    /// <summary>
    /// Gets or sets the value type of <see cref="TargetProperty"/> used to convert <see cref="Value"/> (Uno Platform
    /// only). WinUI dependency properties do not expose their type; when this is not set, the type is inferred from the
    /// default or the current value of the property. This is a dependency property.
    /// </summary>
    [StyledProperty]
    public partial Type? TargetPropertyType { get; set; }

    /// <summary>
    /// Gets the value type of a dependency property.
    /// </summary>
    /// <remarks>
    /// WinUI dependency properties expose neither their type nor their name (Uno Platform's bindable metadata is looked
    /// up by property name). The type is <see cref="TargetPropertyType"/> when set, otherwise the type of the default
    /// value, otherwise the type of the current value, otherwise <see cref="object"/>. Inferred types may be more
    /// derived than the property type (a <c>SolidColorBrush</c> for a <c>Brush</c> property).
    /// </remarks>
    /// <param name="targetObject">The object owning the value.</param>
    /// <param name="targetProperty">The property.</param>
    /// <param name="isExactType">
    /// <c>true</c> when the returned type is the property type (set explicitly, or a value type or sealed type of the
    /// default value); <c>false</c> when it is inferred.
    /// </param>
    /// <returns>The property value type.</returns>
    private Type GetPropertyType(DependencyObject targetObject, DependencyProperty targetProperty, out bool isExactType)
    {
        if (TargetPropertyType is { } propertyType)
        {
            isExactType = true;
            return propertyType;
        }

        var defaultValue = targetProperty.GetMetadata(targetObject.GetType())?.DefaultValue;
        if (defaultValue is not null && defaultValue != DependencyProperty.UnsetValue)
        {
            var defaultType = defaultValue.GetType();
            isExactType = defaultType.IsValueType || defaultType.IsSealed;
            return defaultType;
        }

        isExactType = false;
        var currentValue = targetObject.GetValue(targetProperty);
        return currentValue is not null && currentValue != DependencyProperty.UnsetValue
            ? currentValue.GetType()
            : typeof(object);
    }

    /// <summary>
    /// Converts a value to a property type inferred from a default or current value.
    /// </summary>
    /// <remarks>
    /// Only strings are converted: other values may be valid for the (unknown, possibly less derived) property type.
    /// When the string cannot be converted to the inferred type, it is assigned as is.
    /// </remarks>
    /// <param name="value">The value.</param>
    /// <param name="inferredType">The inferred type.</param>
    /// <returns>The value to assign.</returns>
    private static object? ConvertToInferredType(object? value, Type inferredType)
    {
        if (value is not string text || inferredType == typeof(object) || inferredType.IsInstanceOfType(value))
        {
            return value;
        }

        try
        {
            var result = inferredType.IsEnum
                ? Enum.Parse(inferredType, text, false)
                : TypeConverterHelper.Convert(text, inferredType);
            return result is not null && inferredType.IsInstanceOfType(result) ? result : value;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or InvalidCastException or OverflowException or NotSupportedException)
        {
            return value;
        }
    }
}
