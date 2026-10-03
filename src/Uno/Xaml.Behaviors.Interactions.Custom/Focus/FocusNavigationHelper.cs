// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of the Avalonia tab order helper used by the focus behaviors.
/// </summary>
/// <remarks>
/// WinUI implements tab navigation (tab index, <c>TabFocusNavigation</c> modes, focusability) in
/// <see cref="FocusManager"/>; this helper maps the Avalonia helper API onto it. Candidates are computed from the
/// focused element of the scope's <see cref="XamlRoot"/>.
/// </remarks>
internal static class FocusNavigationHelper
{
    /// <summary>
    /// Finds the first (<see cref="FocusNavigationDirection.Next"/>) or last
    /// (<see cref="FocusNavigationDirection.Previous"/>) focusable element of a scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="direction">The navigation direction.</param>
    /// <returns>The boundary element, or <c>null</c> when the scope has no focusable element.</returns>
    public static UIElement? FindBoundary(UIElement scope, FocusNavigationDirection direction)
    {
        var boundary = direction == FocusNavigationDirection.Previous
            ? FocusManager.FindLastFocusableElement(scope)
            : FocusManager.FindFirstFocusableElement(scope);
        return boundary as UIElement;
    }

    /// <summary>
    /// Finds the element that receives focus when navigating from the focused element in the given direction.
    /// </summary>
    /// <param name="scope">The scope the candidate must belong to.</param>
    /// <param name="current">The current (focused) element.</param>
    /// <param name="direction">The navigation direction.</param>
    /// <param name="wrap">Whether to wrap to the boundary of the scope when no candidate is found.</param>
    /// <returns>The next element, or <c>null</c>.</returns>
    public static UIElement? FindAdjacent(UIElement scope, object? current, FocusNavigationDirection direction, bool wrap)
    {
        if (current is not DependencyObject)
        {
            return wrap ? FindBoundary(scope, direction) : null;
        }

        var options = new FindNextElementOptions
        {
            SearchRoot = scope.XamlRoot?.Content ?? scope,
        };

        if (FocusManager.FindNextElement(direction, options) is UIElement next && IsWithinScope(scope, next))
        {
            return next;
        }

        return wrap ? FindBoundary(scope, direction) : null;
    }

    /// <summary>
    /// Determines whether an element is the scope or one of its visual descendants.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="candidate">The candidate element.</param>
    /// <returns><c>true</c> when the candidate belongs to the scope.</returns>
    public static bool IsWithinScope(DependencyObject scope, DependencyObject? candidate)
    {
        for (var current = candidate; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (ReferenceEquals(current, scope))
            {
                return true;
            }
        }

        return false;
    }
}
