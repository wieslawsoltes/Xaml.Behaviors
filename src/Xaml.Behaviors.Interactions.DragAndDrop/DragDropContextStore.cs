// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Concurrent;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides an in-process store for objects attached to drag data.
/// </summary>
internal static class DragDropContextStore
{
    private static readonly ConcurrentDictionary<string, WeakReference<object?>> s_entries = new();

    public static string? Add(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var key = Guid.NewGuid().ToString();
        s_entries[key] = new WeakReference<object?>(value);
        return key;
    }

    public static object? Get(string? key)
    {
        if (key is null)
        {
            return null;
        }

        if (s_entries.TryGetValue(key, out var reference))
        {
            if (reference.TryGetTarget(out var value))
            {
                return value;
            }

            s_entries.TryRemove(key, out _);
        }

        return null;
    }

    public static void Remove(string? key)
    {
        if (key is null)
        {
            return;
        }

        s_entries.TryRemove(key, out _);
    }
}
