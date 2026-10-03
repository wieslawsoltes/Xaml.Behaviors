// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>StyledPropertyMetadata&lt;TValue&gt;</c> used to override defaults per type.
/// </summary>
/// <typeparam name="TValue">The property type.</typeparam>
/// <param name="defaultValue">The default value for the overriding type.</param>
internal sealed class StyledPropertyMetadata<TValue>(TValue defaultValue = default!) : IStyledPropertyMetadata
{
    /// <summary>Gets the default value.</summary>
    public TValue DefaultValue { get; } = defaultValue;

    object? IStyledPropertyMetadata.DefaultValue => DefaultValue;
}

/// <summary>
/// Non generic view of <see cref="StyledPropertyMetadata{TValue}"/>.
/// </summary>
internal interface IStyledPropertyMetadata
{
    /// <summary>Gets the default value.</summary>
    object? DefaultValue { get; }
}

/// <summary>
/// Per type default value overrides (WinUI dependency properties have a single default value).
/// </summary>
/// <remarks>
/// <c>XProperty.OverrideMetadata&lt;TOwner&gt;(...)</c> in static constructors records the default; the Uno base
/// classes apply the most derived override as a local value when an instance is created.
/// </remarks>
internal static class PropertyMetadataOverrides
{
    private static readonly ConcurrentDictionary<Type, List<(DependencyProperty Property, object? Value)>> s_overrides = new();
    private static readonly ConcurrentDictionary<Type, (DependencyProperty Property, object? Value)[]> s_effective = new();

    public static void OverrideMetadata<TOwner>(this DependencyProperty property, IStyledPropertyMetadata metadata)
    {
        var list = s_overrides.GetOrAdd(typeof(TOwner), static _ => []);
        lock (list)
        {
            list.RemoveAll(x => x.Property == property);
            list.Add((property, metadata.DefaultValue));
        }

        s_effective.Clear();
    }

    public static void Apply(DependencyObject instance)
    {
        var effective = s_effective.GetOrAdd(instance.GetType(), static type => Resolve(type));
        foreach (var (property, value) in effective)
        {
            instance.SetValue(property, value);
        }
    }

    private static (DependencyProperty Property, object? Value)[] Resolve(Type type)
    {
        var result = new List<(DependencyProperty Property, object? Value)>();
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (!s_overrides.TryGetValue(current, out var list))
            {
                continue;
            }

            lock (list)
            {
                foreach (var item in list)
                {
                    if (!result.Exists(x => x.Property == item.Property))
                    {
                        result.Add(item);
                    }
                }
            }
        }

        return result.ToArray();
    }
}
