// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>VisualExtensions</c> and <c>LogicalExtensions</c> tree traversal
/// helpers used by the shared sources.
/// </summary>
/// <remarks>
/// WinUI has no logical tree: the logical helpers walk the visual tree, falling back to
/// <see cref="FrameworkElement.Parent"/> for elements that are not (yet) part of a visual tree. Items controls are
/// ancestors of their item containers in both trees, which is what the shared sources rely on.
/// </remarks>
internal static class TreeTraversalExtensions
{
    /// <summary>
    /// Enumerates the element and its ancestors (Avalonia <c>GetSelfAndLogicalAncestors</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The element followed by its ancestors.</returns>
    public static IEnumerable<DependencyObject> GetSelfAndLogicalAncestors(this DependencyObject element)
    {
        for (DependencyObject? current = element; current is not null; current = ParentOf(current))
        {
            yield return current;
        }
    }

    /// <summary>
    /// Enumerates the ancestors of the element (Avalonia <c>GetLogicalAncestors</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The ancestors, nearest first.</returns>
    public static IEnumerable<DependencyObject> GetLogicalAncestors(this DependencyObject element)
    {
        for (var current = ParentOf(element); current is not null; current = ParentOf(current))
        {
            yield return current;
        }
    }

    /// <summary>
    /// Enumerates the visual descendants of the element, depth first (Avalonia <c>GetVisualDescendants</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The descendants.</returns>
    public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
    {
        var count = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            yield return child;

            foreach (var descendant in child.GetVisualDescendants())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// Finds the nearest ancestor of the given type (Avalonia <c>FindAncestorOfType</c>).
    /// </summary>
    /// <typeparam name="T">The ancestor type.</typeparam>
    /// <param name="element">The element.</param>
    /// <param name="includeSelf">Whether the element itself is considered.</param>
    /// <returns>The ancestor or <c>null</c>.</returns>
    public static T? FindAncestorOfType<T>(this DependencyObject? element, bool includeSelf = false)
        where T : class
    {
        if (element is null)
        {
            return null;
        }

        for (var current = includeSelf ? element : ParentOf(element); current is not null; current = ParentOf(current))
        {
            if (current is T result)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the visual parent, or <see cref="FrameworkElement.Parent"/> when the element is not in a visual tree.
    /// </summary>
    private static DependencyObject? ParentOf(DependencyObject element)
        => VisualTreeHelper.GetParent(element) ?? (element as FrameworkElement)?.Parent;
}
