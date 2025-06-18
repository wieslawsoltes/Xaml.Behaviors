using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Avalonia.Xaml.Interactivity;

[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
internal static class PropertyHelper
{
    private static readonly char[] s_trimChars = ['(', ')'];
    private static readonly char[] s_separator = ['.'];

    private static Type? GetTypeByName(string name)
    {
        return
            AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t is not null)
            ??
            AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == name);
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
        var targetType = GetTypeByName(targetPropertyTypeName) ?? targetObject.GetType();

        var registeredAttached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType);

        foreach (var avaloniaProperty in registeredAttached)
        {
            if (avaloniaProperty.OwnerType.Name == targetPropertyTypeName && avaloniaProperty.Name == targetPropertyName)
            {
                return avaloniaProperty;
            }
        }

        var registeredInherited = AvaloniaPropertyRegistry.Instance.GetRegisteredInherited(targetType);

        foreach (var avaloniaProperty in registeredInherited)
        {
            if (avaloniaProperty.Name == targetPropertyName)
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

    private static void UpdateAvaloniaPropertyValue(AvaloniaObject avaloniaObject, AvaloniaProperty property, string propertyName, object? value)
    {
        ValidateAvaloniaProperty(property, propertyName);

        Exception? innerException = null;
        try
        {
            object? result = null;
            var propertyType = property.PropertyType;
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

            avaloniaObject.SetValue(property, result);
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

    public static bool UpdatePropertyValue(object targetObject, string propertyName, object? value)
    {
        if (targetObject is AvaloniaObject avaloniaObject)
        {
            if (propertyName.Contains('.'))
            {
                var avaloniaProperty = FindAvaloniaAttachedProperty(targetObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(avaloniaObject, avaloniaProperty, propertyName, value);
                    return true;
                }

                return false;
            }
            else
            {
                var avaloniaProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(avaloniaObject, avaloniaProperty, propertyName, value);
                    return true;
                }
            }
        }

        UpdateClrPropertyValue(targetObject, propertyName, value);
        return true;
    }
}
