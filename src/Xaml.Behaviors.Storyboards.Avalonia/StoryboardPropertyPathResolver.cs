// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Animation;

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Resolves storyboard target property paths into concrete animatable targets and properties.
/// </summary>
internal static class StoryboardPropertyPathResolver
{
    private static readonly ConcurrentDictionary<string, Type?> s_cachedTypes = new(StringComparer.Ordinal);

    /// <summary>
    /// Resolves the specified property path against the supplied root object.
    /// </summary>
    /// <param name="root">The root target object.</param>
    /// <param name="path">The parsed property path.</param>
    /// <returns>The resolved animatable target and property.</returns>
    [RequiresUnreferencedCode("Storyboard property path resolution uses reflection to locate target types and members.")]
    public static StoryboardResolvedTarget Resolve(AvaloniaObject root, StoryboardPropertyPath path)
    {
        if (root is null)
        {
            throw new ArgumentNullException(nameof(root));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var current = root;

        for (var i = 0; i < path.Segments.Count; i++)
        {
            var segment = path.Segments[i];
            var ownerType = ResolveOwnerType(segment, current, path.OriginalPath);
            var property = ResolveAvaloniaProperty(ownerType, segment.PropertyName, path.OriginalPath);
            var isLast = i == path.Segments.Count - 1;

            if (isLast)
            {
                if (current is not Animatable animatable)
                {
                    throw new InvalidOperationException(
                        $"Storyboard target property path '{path.OriginalPath}' resolved to '{current.GetType().FullName}' which is not Animatable.");
                }

                return new StoryboardResolvedTarget(animatable, property);
            }

            current = ResolveNextObject(current, property, segment, path.OriginalPath);
        }

        throw new InvalidOperationException(
            $"Storyboard target property path '{path.OriginalPath}' did not resolve to a property.");
    }

    [RequiresUnreferencedCode("Storyboard property path type resolution scans loaded assemblies.")]
    private static Type ResolveOwnerType(
        StoryboardPropertyPathSegment segment,
        AvaloniaObject current,
        string fullPath)
    {
        if (!segment.IsAttached)
        {
            return current.GetType();
        }

        if (string.IsNullOrWhiteSpace(segment.OwnerTypeName))
        {
            throw new InvalidOperationException(
                $"Storyboard target property path '{fullPath}' contains an attached property without an owner type.");
        }

        var ownerTypeName = segment.OwnerTypeName!.Trim();
        return ResolveType(ownerTypeName)
            ?? throw new InvalidOperationException(
                $"Storyboard target property path '{fullPath}' could not resolve owner type '{segment.OwnerTypeName}'.");
    }

    [RequiresUnreferencedCode("Storyboard property path property resolution uses reflection to access AvaloniaProperty fields.")]
    private static AvaloniaProperty ResolveAvaloniaProperty(Type ownerType, string propertyName, string fullPath)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new InvalidOperationException(
                $"Storyboard target property path '{fullPath}' contains an empty property segment.");
        }

        var registry = AvaloniaPropertyRegistry.Instance;
        var property = registry.FindRegistered(ownerType, propertyName);

        if (property is null)
        {
            var field = ownerType.GetField(
                propertyName + "Property",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field?.GetValue(null) is AvaloniaProperty fieldProperty)
            {
                property = fieldProperty;
            }
        }

        if (property is null)
        {
            throw new InvalidOperationException(
                $"Storyboard target property '{propertyName}' could not be found on '{ownerType.FullName}'.");
        }

        return property;
    }

    private static AvaloniaObject ResolveNextObject(
        AvaloniaObject current,
        AvaloniaProperty property,
        StoryboardPropertyPathSegment segment,
        string fullPath)
    {
        var value = current.GetValue(property);
        value = ApplyIndexers(value, segment.IndexerArguments, fullPath);

        if (value is AvaloniaObject next)
        {
            return next;
        }

        throw new InvalidOperationException(
            $"Storyboard target property path '{fullPath}' resolved segment '{segment.PropertyName}' to '{value?.GetType().FullName ?? "null"}', which is not an AvaloniaObject.");
    }

    private static object? ApplyIndexers(
        object? value,
        IReadOnlyList<string> indexers,
        string fullPath)
    {
        if (indexers.Count == 0)
        {
            return value;
        }

        foreach (var argument in indexers)
        {
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"Storyboard target property path '{fullPath}' could not apply indexer '{argument}' because the intermediate value was null.");
            }

            if (value is IList list)
            {
                if (!int.TryParse(argument, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                {
                    throw new InvalidOperationException(
                        $"Storyboard target property path '{fullPath}' uses index '{argument}' which is not a valid integer.");
                }

                if (index < 0 || index >= list.Count)
                {
                    throw new InvalidOperationException(
                        $"Storyboard target property path '{fullPath}' index '{argument}' is out of range.");
                }

                value = list[index];
                continue;
            }

            if (value is IDictionary dictionary)
            {
                value = dictionary[argument];
                continue;
            }

            throw new InvalidOperationException(
                $"Storyboard target property path '{fullPath}' uses an indexer on '{value.GetType().FullName}', which does not support indexing.");
        }

        return value;
    }

    [RequiresUnreferencedCode("Storyboard property path type resolution scans loaded assemblies.")]
    private static Type? ResolveType(string ownerTypeName)
    {
        return s_cachedTypes.GetOrAdd(ownerTypeName, static key =>
        {
            var candidateNames = EnumerateCandidateNames(key).ToArray();

            foreach (var name in candidateNames)
            {
                var direct = Type.GetType(name, throwOnError: false, ignoreCase: false);
                if (direct is not null)
                {
                    return direct;
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var name in candidateNames)
                {
                    var type = assembly.GetType(name, throwOnError: false, ignoreCase: false);
                    if (type is not null)
                    {
                        return type;
                    }
                }

                var types = GetTypesSafely(assembly);
                foreach (var type in types)
                {
                    if (candidateNames.Any(n => string.Equals(type.FullName, n, StringComparison.Ordinal)))
                    {
                        return type;
                    }

                    if (candidateNames.Any(n => string.Equals(type.Name, n, StringComparison.Ordinal)))
                    {
                        return type;
                    }
                }
            }

            return null;
        });
    }

    private static IEnumerable<string> EnumerateCandidateNames(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            yield break;
        }

        var trimmed = raw.Trim();
        yield return trimmed;

        var colonIndex = raw.IndexOf(':');
        if (colonIndex >= 0 && colonIndex < raw.Length - 1)
        {
            var withoutPrefix = raw.Substring(colonIndex + 1).Trim();
            if (withoutPrefix.Length > 0 && !string.Equals(withoutPrefix, trimmed, StringComparison.Ordinal))
            {
                yield return withoutPrefix;
            }
        }
    }

    [RequiresUnreferencedCode("Enumerating assembly types for storyboard property resolution can keep trimmed members alive.")]
    private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!
                .Cast<Type>();
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }
}

/// <summary>
/// Represents a resolved storyboard target and property.
/// </summary>
internal readonly struct StoryboardResolvedTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoryboardResolvedTarget"/> struct.
    /// </summary>
    /// <param name="target">The resolved animatable target.</param>
    /// <param name="property">The property to animate.</param>
    public StoryboardResolvedTarget(Animatable target, AvaloniaProperty property)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Property = property ?? throw new ArgumentNullException(nameof(property));
    }

    /// <summary>
    /// Gets the animatable target instance.
    /// </summary>
    public Animatable Target { get; }

    /// <summary>
    /// Gets the property to animate.
    /// </summary>
    public AvaloniaProperty Property { get; }
}
