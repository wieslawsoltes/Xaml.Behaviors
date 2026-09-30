// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <content>
/// Uno Platform property lookup. WinUI has no property registry and <see cref="DependencyProperty"/> exposes
/// neither its name nor its type, so properties are resolved from the conventional static
/// <c>{Name}Property</c> members and the CLR accessors, like the Avalonia helper does for attached properties.
/// </content>
internal static partial class PropertyHelper
{
    private const BindingFlags StaticMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly ConcurrentDictionary<(Type Type, string Name), DependencyProperty?> s_properties = new();
    private static readonly ConcurrentDictionary<(Type Type, string Name), Type?> s_ownerTypes = new();

    internal static DependencyProperty? FindRegisteredProperty(DependencyObject dependencyObject, string propertyName)
        => s_properties.GetOrAdd((dependencyObject.GetType(), propertyName), static key => FindStaticProperty(key.Type, key.Name + "Property"));

    private static DependencyProperty? FindAvaloniaAttachedProperty(object? targetObject, string propertyName)
    {
        if (targetObject is null || !TrySplitAttachedName(propertyName, out var ownerTypeName, out var name))
        {
            return null;
        }

        var ownerType = FindOwnerType(targetObject.GetType(), ownerTypeName);
        return ownerType is null
            ? null
            : s_properties.GetOrAdd((ownerType, name), static key => FindStaticProperty(key.Type, key.Name + "Property"));
    }

    private static Type GetPropertyType(DependencyProperty property, DependencyObject dependencyObject, string propertyName)
    {
        if (TrySplitAttachedName(propertyName, out var ownerTypeName, out var name))
        {
            var getter = FindOwnerType(dependencyObject.GetType(), ownerTypeName)?.GetMethod("Get" + name, BindingFlags.Public | BindingFlags.Static);
            if (getter is not null)
            {
                return getter.ReturnType;
            }
        }
        else if (dependencyObject.GetType().GetRuntimeProperty(propertyName) is { } clrProperty)
        {
            return clrProperty.PropertyType;
        }

        return property.GetMetadata(dependencyObject.GetType())?.DefaultValue?.GetType() ?? typeof(object);
    }

    private static bool IsReadOnlyProperty(DependencyProperty property, DependencyObject dependencyObject, string propertyName)
    {
        if (TrySplitAttachedName(propertyName, out var ownerTypeName, out var name))
        {
            var ownerType = FindOwnerType(dependencyObject.GetType(), ownerTypeName);
            return ownerType?.GetMethod("Set" + name, BindingFlags.Public | BindingFlags.Static) is null;
        }

        var clrProperty = dependencyObject.GetType().GetRuntimeProperty(propertyName);
        return clrProperty is not null && clrProperty.SetMethod is not { IsPublic: true };
    }

    // WinUI has no field backed (direct) dependency properties.
    private static bool IsDirectProperty(DependencyProperty property) => false;

    internal static IDisposable? SetTemporaryValue(DependencyObject dependencyObject, DependencyProperty property, object? value)
    {
        dependencyObject.SetValue(property, value, DependencyPropertyValuePrecedences.Animations);
        return DisposableAction.Create(() =>
            dependencyObject.SetValue(property, DependencyProperty.UnsetValue, DependencyPropertyValuePrecedences.Animations));
    }

    private static bool TrySplitAttachedName(string propertyName, out string ownerTypeName, out string name)
    {
        var parts = propertyName.Trim().Trim(s_trimChars).Split(s_separator);
        if (parts.Length == 2)
        {
            ownerTypeName = parts[0];
            name = parts[1];
            return true;
        }

        ownerTypeName = string.Empty;
        name = string.Empty;
        return false;
    }

    private static DependencyProperty? FindStaticProperty(Type type, string memberName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.GetField(memberName, StaticMemberFlags)?.GetValue(null) is DependencyProperty field)
            {
                return field;
            }

            if (current.GetProperty(memberName, StaticMemberFlags)?.GetValue(null) is DependencyProperty property)
            {
                return property;
            }
        }

        return null;
    }

    private static Type? FindOwnerType(Type targetType, string ownerTypeName)
        => s_ownerTypes.GetOrAdd((targetType, ownerTypeName), static key => ResolveOwnerType(key.Type, key.Name));

    private static Type? ResolveOwnerType(Type targetType, string ownerTypeName)
    {
        for (var current = targetType; current is not null; current = current.BaseType)
        {
            if (current.Name == ownerTypeName || current.FullName == ownerTypeName)
            {
                return current;
            }
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (ownerTypeName.Contains('.'))
            {
                if (assembly.GetType(ownerTypeName, throwOnError: false, ignoreCase: false) is { } exact)
                {
                    return exact;
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

            foreach (var type in types)
            {
                if (type is { IsPublic: true } && type.Name == ownerTypeName)
                {
                    return type;
                }
            }
        }

        return null;
    }
}
