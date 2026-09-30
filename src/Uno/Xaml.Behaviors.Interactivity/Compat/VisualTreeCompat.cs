// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterparts of the Avalonia visual/logical tree helpers used by the shared sources.
/// </summary>
/// <remarks>WinUI has no logical tree: logical lookups walk the visual tree.</remarks>
internal static class VisualTreeCompatExtensions
{
    /// <summary>
    /// Gets the top-most hit testable element at a position (Avalonia <c>Visual.GetVisualAt</c>).
    /// </summary>
    /// <param name="root">The element whose subtree is searched.</param>
    /// <param name="position">The position relative to <paramref name="root"/>.</param>
    /// <returns>The element or <c>null</c>.</returns>
    /// <remarks>
    /// The deepest visible, hit test visible element whose layout bounds contain the position is returned (children are
    /// visited top-most first). Render transforms of the descendants are taken into account.
    /// </remarks>
    public static FrameworkElement? GetVisualAt(this UIElement root, Point position)
        => root is FrameworkElement frameworkElement ? HitTest(frameworkElement, root, position) : null;

    private static FrameworkElement? HitTest(FrameworkElement element, UIElement root, Point position)
    {
        if (element.Visibility != Visibility.Visible || !element.IsHitTestVisible)
        {
            return null;
        }

        for (var i = VisualTreeHelper.GetChildrenCount(element) - 1; i >= 0; i--)
        {
            if (VisualTreeHelper.GetChild(element, i) is FrameworkElement child && HitTest(child, root, position) is { } hit)
            {
                return hit;
            }
        }

        var origin = element.TransformToVisual(root).TransformPoint(new Point(0, 0));
        var bounds = new Rect(origin.X, origin.Y, element.ActualWidth, element.ActualHeight);
        return bounds.Contains(position) ? element : null;
    }

    /// <summary>
    /// Finds the closest ancestor of a type (Avalonia <c>FindLogicalAncestorOfType</c>; the visual tree on WinUI).
    /// </summary>
    /// <typeparam name="T">The ancestor type.</typeparam>
    /// <param name="element">The element.</param>
    /// <param name="includeSelf">Whether the element itself is considered.</param>
    /// <returns>The ancestor or <c>null</c>.</returns>
    public static T? FindLogicalAncestorOfType<T>(this DependencyObject? element, bool includeSelf = false) where T : class
    {
        var current = includeSelf ? element : element is null ? null : VisualTreeHelper.GetParent(element);
        while (current is not null)
        {
            if (current is T result)
            {
                return result;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    /// <summary>
    /// Gets the visual children of an element (Avalonia <c>GetVisualChildren</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The children.</returns>
    public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
    {
        var count = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < count; i++)
        {
            yield return VisualTreeHelper.GetChild(element, i);
        }
    }

    /// <summary>
    /// Gets the visual descendants of an element, depth first (Avalonia <c>GetVisualDescendants</c>).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The descendants.</returns>
    public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
    {
        var stack = new Stack<DependencyObject>();
        for (var i = VisualTreeHelper.GetChildrenCount(element) - 1; i >= 0; i--)
        {
            stack.Push(VisualTreeHelper.GetChild(element, i));
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            for (var i = VisualTreeHelper.GetChildrenCount(current) - 1; i >= 0; i--)
            {
                stack.Push(VisualTreeHelper.GetChild(current, i));
            }
        }
    }
}
