// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Avalonia.Data;

namespace Avalonia.Xaml.Interactivity;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
internal static class PropertyHelper
{
    private static readonly char[] s_trimChars = ['(', ')'];
    private static readonly char[] s_separator = ['.'];

    private static bool IsTypeNameInHierarchy(Type targetType, string typeName)
    {
        for (Type? currentType = targetType; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.Name == typeName)
            {
                return true;
            }
        }

        return false;
    }

    private static AvaloniaProperty? InitializeOwnerAndFindAttachedProperty(
        Type targetType,
        string ownerTypeName,
        string propertyName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (ownerTypeName.Contains('.'))
            {
                var ownerType = assembly.GetType(ownerTypeName, throwOnError: false, ignoreCase: false);
                var exactMatch = TryInitializeAttachedProperty(targetType, ownerType, propertyName);
                if (exactMatch is not null)
                {
                    return exactMatch;
                }

                continue;
            }

            Type?[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                types = exception.Types;
            }

            foreach (var ownerType in types)
            {
                if (ownerType?.Name != ownerTypeName)
                {
                    continue;
                }

                var match = TryInitializeAttachedProperty(targetType, ownerType, propertyName);
                if (match is not null)
                {
                    return match;
                }
            }
        }

        return null;
    }

    private static AvaloniaProperty? TryInitializeAttachedProperty(
        Type targetType,
        Type? ownerType,
        string propertyName)
    {
        if (ownerType is null)
        {
            return null;
        }

        var field = ownerType.GetTypeInfo().GetDeclaredField($"{propertyName}Property");
        if (field is null ||
            !field.IsStatic ||
            !typeof(AvaloniaProperty).GetTypeInfo().IsAssignableFrom(field.FieldType.GetTypeInfo()))
        {
            return null;
        }

        if (field.GetValue(null) is not AvaloniaProperty candidate ||
            candidate.OwnerType != ownerType)
        {
            return null;
        }

        var registeredAttached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType);
        foreach (var avaloniaProperty in registeredAttached)
        {
            if (ReferenceEquals(avaloniaProperty, candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static AvaloniaProperty? FindAvaloniaAttachedProperty(object? targetObject, string propertyName)
    {
        if (targetObject is null)
        {
            return null;
        }
        
        var propertyNames = propertyName.Trim().Trim(s_trimChars).Split(s_separator);
        if (propertyNames.Length != 2)
        {
            return null;
        }
        var targetPropertyTypeName = propertyNames[0];
        var targetPropertyName = propertyNames[1];
        var targetType = targetObject.GetType();

        var registeredAttached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType);

        foreach (var avaloniaProperty in registeredAttached)
        {
            if (avaloniaProperty.OwnerType.Name == targetPropertyTypeName && avaloniaProperty.Name == targetPropertyName)
            {
                return avaloniaProperty;
            }
        }

        var initializedAttachedProperty = InitializeOwnerAndFindAttachedProperty(
            targetType,
            targetPropertyTypeName,
            targetPropertyName);
        if (initializedAttachedProperty is not null)
        {
            return initializedAttachedProperty;
        }

        var registeredInherited = AvaloniaPropertyRegistry.Instance.GetRegisteredInherited(targetType);

        foreach (var avaloniaProperty in registeredInherited)
        {
            if ((avaloniaProperty.OwnerType.Name == targetPropertyTypeName ||
                 IsTypeNameInHierarchy(targetType, targetPropertyTypeName)) &&
                avaloniaProperty.Name == targetPropertyName)
            {
                return avaloniaProperty;
            }
        }

        return null;
    }

    private static void UpdateClrPropertyValue(object targetObject, string propertyName, object? value)
    {
        var targetType = targetObject.GetType();
        var targetTypeName = targetType.Name;
        var propertyInfo = targetType.GetRuntimeProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot find a property named {0} on type {1}.",
                propertyName,
                targetTypeName));
        }
        else if (!propertyInfo.CanWrite)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Property {0} on type {1} is read-only.",
                propertyName,
                targetTypeName));
        }

        Exception? innerException = null;
        try
        {
            object? result = null;
            var propertyType = propertyInfo.PropertyType;
            var propertyTypeInfo = propertyType.GetTypeInfo();
            if (value is null)
            {
                // The result can be null if the type is generic (nullable), or the default value of the type in question
                result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
            }
            else if (propertyTypeInfo.IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                result = value;
            }
            else
            {
                var valueAsString = value.ToString();
                if (valueAsString is not null)
                {
                    if (propertyTypeInfo.IsEnum)
                    {
                        result = Enum.Parse(propertyType, valueAsString, false);
                    }
                    else
                    {
                        var convert = TypeConverterHelper.Convert(valueAsString, propertyType);
                        result = convert ?? value;
                    }
                }
            }

            propertyInfo.SetValue(targetObject, result, []);
        }
        catch (FormatException e)
        {
            innerException = e;
        }
        catch (ArgumentException e)
        {
            innerException = e;
        }

        if (innerException is not null)
        {
            throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                    value?.GetType().Name ?? "null",
                    propertyName,
                    propertyInfo.PropertyType.Name),
                innerException);
        }
    }

    private static bool TryGetClrPropertyValue(object targetObject, string propertyName, out object? value)
    {
        var targetType = targetObject.GetType();
        var targetTypeName = targetType.Name;
        var propertyInfo = targetType.GetRuntimeProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot find a property named {0} on type {1}.",
                propertyName,
                targetTypeName));
        }

        if (!propertyInfo.CanRead)
        {
            value = null;
            return false;
        }

        value = propertyInfo.GetValue(targetObject, []);
        return true;
    }

    private static void ValidateAvaloniaProperty(AvaloniaProperty? property, string propertyName)
    {
        if (property is null)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot find a property named {0}.",
                propertyName));
        }
        else if (property.IsReadOnly)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Property {0} is read-only.",
                propertyName));
        }
    }

    private static void UpdateAvaloniaPropertyValue(
        AvaloniaObject avaloniaObject,
        AvaloniaProperty property,
        string propertyName,
        object? value,
        bool preserveValueSource)
    {
        ValidateAvaloniaProperty(property, propertyName);

        Exception? innerException = null;
        try
        {
            var result = ConvertAvaloniaPropertyValue(property, value);

            if (preserveValueSource)
            {
                avaloniaObject.SetCurrentValue(property, result);
            }
            else
            {
                avaloniaObject.SetValue(property, result);
            }
        }
        catch (FormatException e)
        {
            innerException = e;
        }
        catch (ArgumentException e)
        {
            innerException = e;
        }

        if (innerException is not null)
        {
            throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                    value?.GetType().Name ?? "null",
                    propertyName,
                    avaloniaObject.GetType().Name),
                innerException);
        }
    }

    public static bool TrySetTemporaryAvaloniaPropertyValue(
        object targetObject,
        string propertyName,
        object? value,
        out IDisposable? reversion)
    {
        reversion = null;
        if (targetObject is not AvaloniaObject avaloniaObject)
        {
            return false;
        }

        var property = propertyName.Contains('.')
            ? FindAvaloniaAttachedProperty(targetObject, propertyName)
            : AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
        if (property is null || property.IsDirect)
        {
            return false;
        }

        ValidateAvaloniaProperty(property, propertyName);
        try
        {
            var result = ConvertAvaloniaPropertyValue(property, value);
            reversion = avaloniaObject.SetValue(property, result, BindingPriority.Animation);
            return reversion is not null;
        }
        catch (FormatException e)
        {
            throw CreateInvalidAssignmentException(avaloniaObject, propertyName, value, e);
        }
        catch (ArgumentException e)
        {
            throw CreateInvalidAssignmentException(avaloniaObject, propertyName, value, e);
        }
    }

    public static bool IsDirectAvaloniaProperty(object targetObject, string propertyName)
    {
        if (targetObject is not AvaloniaObject avaloniaObject)
        {
            return false;
        }

        var property = propertyName.Contains('.')
            ? FindAvaloniaAttachedProperty(targetObject, propertyName)
            : AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
        return property?.IsDirect == true;
    }

    private static object? ConvertAvaloniaPropertyValue(AvaloniaProperty property, object? value)
    {
        var propertyType = property.PropertyType;
        var propertyTypeInfo = propertyType.GetTypeInfo();
        if (value is null)
        {
            return propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }

        if (propertyTypeInfo.IsAssignableFrom(value.GetType().GetTypeInfo()))
        {
            return value;
        }

        var valueAsString = value.ToString();
        if (valueAsString is null)
        {
            return null;
        }

        if (propertyTypeInfo.IsEnum)
        {
            return Enum.Parse(propertyType, valueAsString, false);
        }

        var converted = TypeConverterHelper.Convert(valueAsString, propertyType);
        return converted ?? value;
    }

    private static ArgumentException CreateInvalidAssignmentException(
        AvaloniaObject avaloniaObject,
        string propertyName,
        object? value,
        Exception innerException)
    {
        return new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                value?.GetType().Name ?? "null",
                propertyName,
                avaloniaObject.GetType().Name),
            innerException);
    }

    public static bool UpdatePropertyValue(
        object targetObject,
        string propertyName,
        object? value,
        bool preserveValueSource = false)
    {
        if (targetObject is AvaloniaObject avaloniaObject)
        {
            if (propertyName.Contains('.'))
            {
                var avaloniaProperty = FindAvaloniaAttachedProperty(targetObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(
                        avaloniaObject,
                        avaloniaProperty,
                        propertyName,
                        value,
                        preserveValueSource);
                    return true;
                }

                return false;
            }
            else
            {
                var avaloniaProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(
                        avaloniaObject,
                        avaloniaProperty,
                        propertyName,
                        value,
                        preserveValueSource);
                    return true;
                }
            }
        }

        UpdateClrPropertyValue(targetObject, propertyName, value);
        return true;
    }

    public static bool TryGetPropertyValue(
        object targetObject,
        string propertyName,
        out object? value,
        out bool preserveValueSource)
    {
        preserveValueSource = false;

        if (targetObject is AvaloniaObject avaloniaObject)
        {
            if (propertyName.Contains('.'))
            {
                var avaloniaProperty = FindAvaloniaAttachedProperty(targetObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    value = avaloniaObject.GetValue(avaloniaProperty);
                    preserveValueSource = true;
                    return true;
                }

                value = null;
                return false;
            }

            var registeredProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
            if (registeredProperty is not null)
            {
                value = avaloniaObject.GetValue(registeredProperty);
                preserveValueSource = true;
                return true;
            }
        }

        return TryGetClrPropertyValue(targetObject, propertyName, out value);
    }
}
