// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    private static readonly ConcurrentDictionary<(Type Type, string OwnerTypeName, string Name), (Type? OwnerType, DependencyProperty? Property)> s_attachedProperties = new();
    private static readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, List<TemporaryValue>>> s_temporaryValues = new();

    internal static DependencyProperty? FindRegisteredProperty(DependencyObject dependencyObject, string propertyName)
        => s_properties.GetOrAdd((dependencyObject.GetType(), propertyName), static key => FindStaticProperty(key.Type, key.Name + "Property"));

    private static DependencyProperty? FindAvaloniaAttachedProperty(object? targetObject, string propertyName)
    {
        if (targetObject is null || !TrySplitAttachedName(propertyName, out var ownerTypeName, out var name))
        {
            return null;
        }

        return ResolveAttachedProperty(targetObject.GetType(), ownerTypeName, name).Property;
    }

    private static Type GetPropertyType(DependencyProperty property, DependencyObject dependencyObject, string propertyName)
    {
        if (TrySplitAttachedName(propertyName, out var ownerTypeName, out var name))
        {
            var ownerType = ResolveAttachedProperty(dependencyObject.GetType(), ownerTypeName, name).OwnerType;
            var getter = ownerType?.GetMethod("Get" + name, BindingFlags.Public | BindingFlags.Static);
            if (getter is not null)
            {
                return getter.ReturnType;
            }

            // An owner qualified property of the element itself, e.g. (TextBox.FontSize).
            if (ownerType is not null &&
                ownerType.IsInstanceOfType(dependencyObject) &&
                dependencyObject.GetType().GetRuntimeProperty(name) is { } ownerClrProperty)
            {
                return ownerClrProperty.PropertyType;
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
            var ownerType = ResolveAttachedProperty(dependencyObject.GetType(), ownerTypeName, name).OwnerType;
            if (ownerType?.GetMethod("Set" + name, BindingFlags.Public | BindingFlags.Static) is not null)
            {
                return false;
            }

            // An owner qualified property of the element itself, e.g. (TextBox.FontSize), is set like a regular property.
            if (ownerType is null || !ownerType.IsInstanceOfType(dependencyObject))
            {
                return true;
            }

            var ownerClrProperty = dependencyObject.GetType().GetRuntimeProperty(name);
            return ownerClrProperty is not null && ownerClrProperty.SetMethod is not { IsPublic: true };
        }

        var clrProperty = dependencyObject.GetType().GetRuntimeProperty(propertyName);
        return clrProperty is not null && clrProperty.SetMethod is not { IsPublic: true };
    }

    // WinUI has no field backed (direct) dependency properties.
    private static bool IsDirectProperty(DependencyProperty property) => false;

    /// <summary>
    /// Sets a temporary value (Avalonia: a value with the animation priority) that is removed when the returned
    /// disposable is disposed.
    /// </summary>
    /// <remarks>
    /// WinUI has a single animation value per property, so the temporary values of a property are stacked like the
    /// Avalonia animation values: the newest one is effective and removing an older one keeps it; removing the
    /// effective one applies the previous one, or clears the animation value when none is left.
    /// </remarks>
    internal static IDisposable? SetTemporaryValue(DependencyObject dependencyObject, DependencyProperty property, object? value)
    {
        var values = s_temporaryValues.GetOrCreateValue(dependencyObject);
        if (!values.TryGetValue(property, out var stack))
        {
            stack = [];
            values.Add(property, stack);
        }

        var entry = new TemporaryValue(value);
        stack.Add(entry);
        dependencyObject.SetAnimationValue(property, value);
        return DisposableAction.Create(() => RemoveTemporaryValue(dependencyObject, property, entry));
    }

    private static void RemoveTemporaryValue(DependencyObject dependencyObject, DependencyProperty property, TemporaryValue entry)
    {
        if (!s_temporaryValues.TryGetValue(dependencyObject, out var values) ||
            !values.TryGetValue(property, out var stack))
        {
            return;
        }

        var index = stack.IndexOf(entry);
        if (index < 0)
        {
            return;
        }

        stack.RemoveAt(index);
        if (index < stack.Count)
        {
            // A newer temporary value is effective.
            return;
        }

        if (stack.Count > 0)
        {
            dependencyObject.SetAnimationValue(property, stack[^1].Value);
            return;
        }

        values.Remove(property);
        if (values.Count == 0)
        {
            s_temporaryValues.Remove(dependencyObject);
        }

        dependencyObject.ClearAnimationValue(property);
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

    private static (Type? OwnerType, DependencyProperty? Property) ResolveAttachedProperty(Type targetType, string ownerTypeName, string name)
        => s_attachedProperties.GetOrAdd((targetType, ownerTypeName, name), static key => FindAttachedProperty(key.Type, key.OwnerTypeName, key.Name));

    /// <summary>
    /// Resolves an owner qualified property name, e.g. <c>(Grid.Column)</c> or <c>(TextBox.FontSize)</c>.
    /// </summary>
    /// <remarks>
    /// Like the Avalonia lookup (registered attached properties of the target type, then inherited properties of an
    /// owner in the hierarchy of the target), the owner is a type of the target hierarchy or any type with that name,
    /// whatever its accessibility, that declares the <c>{Name}Property</c> dependency property; types with the same
    /// name that do not declare the property are skipped.
    /// </remarks>
    private static (Type? OwnerType, DependencyProperty? Property) FindAttachedProperty(Type targetType, string ownerTypeName, string name)
    {
        var memberName = name + "Property";
        for (var current = targetType; current is not null; current = current.BaseType)
        {
            if ((current.Name == ownerTypeName || current.FullName == ownerTypeName) &&
                FindStaticProperty(current, memberName) is { } hierarchyProperty)
            {
                return (current, hierarchyProperty);
            }
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (ownerTypeName.Contains('.'))
            {
                if (assembly.GetType(ownerTypeName, throwOnError: false, ignoreCase: false) is { } exact &&
                    FindStaticProperty(exact, memberName) is { } exactProperty)
                {
                    return (exact, exactProperty);
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
                if (type is not null &&
                    type.Name == ownerTypeName &&
                    FindStaticProperty(type, memberName) is { } property)
                {
                    return (type, property);
                }
            }
        }

        return (null, null);
    }

    private sealed class TemporaryValue(object? value)
    {
        public object? Value { get; } = value;
    }
}
