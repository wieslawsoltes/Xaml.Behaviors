// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>VisualExtensions</c> and <c>LogicalExtensions</c> tree helpers used by
/// the shared sources.
/// </summary>
/// <remarks>
/// The visual tree is walked with <see cref="VisualTreeHelper"/>. WinUI has no logical tree: the logical parent is
/// <see cref="FrameworkElement.Parent"/> (for example the <c>Popup</c> hosting a child) and falls back to the visual
/// parent.
/// </remarks>
internal static class VisualTreeCompat
{
    /// <summary>Gets the visual parent of an element.</summary>
    public static DependencyObject? GetVisualParent(this DependencyObject element)
        => VisualTreeHelper.GetParent(element);

    /// <summary>Gets the visual children of an element.</summary>
    public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
    {
        var count = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < count; i++)
        {
            yield return VisualTreeHelper.GetChild(element, i);
        }
    }

    /// <summary>Gets the visual ancestors of an element, closest first.</summary>
    public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject element)
    {
        for (var current = VisualTreeHelper.GetParent(element); current is not null; current = VisualTreeHelper.GetParent(current))
        {
            yield return current;
        }
    }

    /// <summary>Gets the element and its visual ancestors, closest first.</summary>
    public static IEnumerable<DependencyObject> GetSelfAndVisualAncestors(this DependencyObject element)
    {
        yield return element;
        foreach (var ancestor in element.GetVisualAncestors())
        {
            yield return ancestor;
        }
    }

    /// <summary>Determines whether an element is a visual ancestor of another element.</summary>
    public static bool IsVisualAncestorOf(this DependencyObject element, DependencyObject? target)
    {
        for (var current = target is null ? null : VisualTreeHelper.GetParent(target); current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (ReferenceEquals(current, element))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Finds the closest visual ancestor of the given type.</summary>
    public static T? FindAncestorOfType<T>(this DependencyObject? element, bool includeSelf = false)
        where T : class
    {
        if (element is null)
        {
            return null;
        }

        for (var current = includeSelf ? element : VisualTreeHelper.GetParent(element); current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (current is T result)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>Finds the first visual descendant of the given type (depth first).</summary>
    public static T? FindDescendantOfType<T>(this DependencyObject? element, bool includeSelf = false)
        where T : class
    {
        if (element is null)
        {
            return null;
        }

        if (includeSelf && element is T self)
        {
            return self;
        }

        var count = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is T result)
            {
                return result;
            }

            if (child.FindDescendantOfType<T>() is { } descendant)
            {
                return descendant;
            }
        }

        return null;
    }

    /// <summary>Gets the logical parent of an element (<see cref="FrameworkElement.Parent"/>, then the visual parent).</summary>
    public static DependencyObject? GetLogicalParent(this DependencyObject element)
        => (element as FrameworkElement)?.Parent ?? VisualTreeHelper.GetParent(element);

    /// <summary>Gets the element and its logical ancestors, closest first.</summary>
    public static IEnumerable<DependencyObject> GetSelfAndLogicalAncestors(this DependencyObject element)
    {
        for (DependencyObject? current = element; current is not null; current = current.GetLogicalParent())
        {
            yield return current;
        }
    }

    /// <summary>Finds the closest logical ancestor of the given type.</summary>
    public static T? FindLogicalAncestorOfType<T>(this DependencyObject? element, bool includeSelf = false)
        where T : class
    {
        if (element is null)
        {
            return null;
        }

        for (var current = includeSelf ? element : element.GetLogicalParent(); current is not null; current = current.GetLogicalParent())
        {
            if (current is T result)
            {
                return result;
            }
        }

        return null;
    }
}
