using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that will change a specified property to a specified value when invoked.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ChangePropertyAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    private static readonly char[] s_trimChars = ['(', ')'];
    private static readonly char[] s_separator = ['.'];
    private static readonly ConcurrentDictionary<(Type, string), PropertySetter> s_propertyCache = new();
    private static readonly ConcurrentDictionary<string, Type?> s_typeCache = new();

    private readonly struct PropertySetter
    {
        public PropertySetter(Action<object, object?> setter, Type propertyType)
        {
            Setter = setter;
            PropertyType = propertyType;
        }

        public Action<object, object?> Setter { get; }
        public Type PropertyType { get; }
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    private static Type? GetTypeByName(string name)
    {
        return s_typeCache.GetOrAdd(name, static key =>
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var i = assemblies.Length - 1; i >= 0; i--)
            {
                var type = assemblies[i].GetType(key);
                if (type is not null)
                {
                    return type;
                }
            }

            for (var i = assemblies.Length - 1; i >= 0; i--)
            {
                foreach (var type in assemblies[i].GetTypes())
                {
                    if (type.Name == key)
                    {
                        return type;
                    }
                }
            }

            return null;
        });
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    private static AvaloniaProperty? FindAttachedProperty(object? targetObject, string propertyName)
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

    /// <summary>
    /// Identifies the <seealso cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<ChangePropertyAction, string?>(nameof(PropertyName));

    /// <summary>
    /// Identifies the <seealso cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetObjectProperty =
        AvaloniaProperty.Register<ChangePropertyAction, object?>(nameof(TargetObject));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<ChangePropertyAction, object?>(nameof(Value));

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to set. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the object whose property will be changed.
    /// If <seealso cref="TargetObject"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public object? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if updating the property value succeeds; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        object? targetObject;
        if (GetValue(TargetObjectProperty) is not null)
        {
            targetObject = TargetObject;
        }
        else
        {
            targetObject = sender;
        }

        if (targetObject is null)
        {
            return false;
        }

        var propertyName = PropertyName;
        if (propertyName is null)
        {
            return false;
        }

        if (targetObject is AvaloniaObject avaloniaObject)
        {
            if (propertyName.Contains('.'))
            {
                var avaloniaProperty = FindAttachedProperty(targetObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(avaloniaObject, avaloniaProperty);
                    return true;
                }

                return false;
            }
            else
            {
                var avaloniaProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(avaloniaObject, propertyName);
                if (avaloniaProperty is not null)
                {
                    UpdateAvaloniaPropertyValue(avaloniaObject, avaloniaProperty);
                    return true;
                }
            }
        }

        UpdatePropertyValue(targetObject);
        return true;
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    private void UpdatePropertyValue(object targetObject)
    {
        var propertyName = PropertyName;
        if (propertyName is null)
        {
            return;
        }

        var targetType = targetObject.GetType();
        var targetTypeName = targetType.Name;

        if (!s_propertyCache.TryGetValue((targetType, propertyName), out var setter))
        {
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

            setter = new PropertySetter(CreateSetter(propertyInfo), propertyInfo.PropertyType);
            s_propertyCache.TryAdd((targetType, propertyName), setter);
        }

        Exception? innerException = null;
        try
        {
            object? result = null;
            var propertyType = setter.PropertyType;
            var propertyTypeInfo = propertyType.GetTypeInfo();
            if (Value is null)
            {
                // The result can be null if the type is generic (nullable), or the default value of the type in question
                result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
            }
            else if (propertyTypeInfo.IsAssignableFrom(Value.GetType().GetTypeInfo()))
            {
                result = Value;
            }
            else
            {
                var valueAsString = Value.ToString();
                if (valueAsString is not null)
                {
                    if (propertyTypeInfo.IsEnum)
                    {
                        result = Enum.Parse(propertyType, valueAsString, false);
                    }
                    else
                    {
                        var convert = Interactivity.TypeConverterHelper.Convert(valueAsString, propertyType);
                        result = convert ?? Value;
                    }
                }
            }

            setter.Setter(targetObject, result);
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
                    Value?.GetType().Name ?? "null",
                    propertyName,
                    setter.PropertyType.Name),
                innerException);
        }
    }

    [RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
    private void UpdateAvaloniaPropertyValue(AvaloniaObject avaloniaObject, AvaloniaProperty property)
    {
        ValidateAvaloniaProperty(property);

        Exception? innerException = null;
        try
        {
            object? result = null;
            var propertyType = property.PropertyType;
            var propertyTypeInfo = propertyType.GetTypeInfo();
            if (Value is null)
            {
                // The result can be null if the type is generic (nullable), or the default value of the type in question
                result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
            }
            else if (propertyTypeInfo.IsAssignableFrom(Value.GetType().GetTypeInfo()))
            {
                result = Value;
            }
            else
            {
                var valueAsString = Value.ToString();
                if (valueAsString is not null)
                {
                    if (propertyTypeInfo.IsEnum)
                    {
                        result = Enum.Parse(propertyType, valueAsString, false);
                    }
                    else
                    {
                        var convert = Interactivity.TypeConverterHelper.Convert(valueAsString, propertyType);
                        result = convert ?? Value;
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
                    Value?.GetType().Name ?? "null",
                    PropertyName,
                    avaloniaObject.GetType().Name),
                innerException);
        }
    }

    /// <summary>
    /// Ensures the property is not null and can be written to.
    /// </summary>
    private void ValidateAvaloniaProperty(AvaloniaProperty? property)
    {
        if (property is null)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot find a property named {0}.",
                PropertyName));
        }
        else if (property.IsReadOnly)
        {
            throw new ArgumentException(string.Format(
                CultureInfo.CurrentCulture,
                "Property {0} is read-only.",
                PropertyName));
        }
    }

    private static Action<object, object?> CreateSetter(PropertyInfo propertyInfo)
    {
        var target = Expression.Parameter(typeof(object), "target");
        var value = Expression.Parameter(typeof(object), "value");

        var instance = Expression.Convert(target, propertyInfo.DeclaringType!);
        var convertedValue = Expression.Convert(value, propertyInfo.PropertyType);
        var call = Expression.Call(instance, propertyInfo.SetMethod!, convertedValue);
        var lambda = Expression.Lambda<Action<object, object?>>(call, target, value);
        return lambda.Compile();
    }
}
