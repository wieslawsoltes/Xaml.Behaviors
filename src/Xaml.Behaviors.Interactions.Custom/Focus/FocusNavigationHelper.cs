// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

internal static class FocusNavigationHelper
{
    public static InputElement? FindBoundary(Visual scope, NavigationDirection direction)
    {
        var focusableElements = GetFocusableElements(scope);
        if (focusableElements.Count == 0)
        {
            return null;
        }

        return direction == NavigationDirection.Previous
            ? focusableElements[^1]
            : focusableElements[0];
    }

    public static InputElement? FindAdjacent(Visual scope, IInputElement? current, NavigationDirection direction, bool wrap)
    {
        if (current is not InputElement currentElement)
        {
            return wrap ? FindBoundary(scope, direction) : null;
        }

        foreach (var navigationScope in GetSpecialNavigationScopes(currentElement, scope))
        {
            if (TryFindInNavigationScope(navigationScope, currentElement, direction, out var candidate))
            {
                return candidate;
            }
        }

        if (scope is InputElement scopeElement
            && TryFindInNavigationScope(scopeElement, currentElement, direction, out var scopedCandidate))
        {
            return scopedCandidate;
        }

        return FindAdjacentInScope(scope, currentElement, direction, wrap);
    }

    private static IEnumerable<InputElement> GetSpecialNavigationScopes(InputElement current, Visual root)
    {
        for (var visual = current.GetVisualParent(); visual is not null; visual = visual.GetVisualParent())
        {
            if (ReferenceEquals(visual, root))
            {
                yield break;
            }

            if (visual is InputElement inputElement)
            {
                var mode = KeyboardNavigation.GetTabNavigation(inputElement);
                if (mode == KeyboardNavigationMode.Cycle || mode == KeyboardNavigationMode.Contained)
                {
                    yield return inputElement;
                }
            }
        }
    }

    private static bool TryFindInNavigationScope(
        InputElement scope,
        InputElement current,
        NavigationDirection direction,
        out InputElement? candidate)
    {
        var mode = KeyboardNavigation.GetTabNavigation(scope);
        if (mode != KeyboardNavigationMode.Cycle && mode != KeyboardNavigationMode.Contained)
        {
            candidate = null;
            return false;
        }

        candidate = FindAdjacentInScope(scope, current, direction, wrap: mode == KeyboardNavigationMode.Cycle);
        if (candidate is not null)
        {
            return true;
        }

        if (mode == KeyboardNavigationMode.Contained)
        {
            candidate = current;
            return true;
        }

        return false;
    }

    private static InputElement? FindAdjacentInScope(
        Visual scope,
        InputElement current,
        NavigationDirection direction,
        bool wrap)
    {
        var focusableElements = GetFocusableElements(scope);
        if (focusableElements.Count == 0)
        {
            return null;
        }

        var currentIndex = focusableElements.IndexOf(current);
        if (currentIndex < 0)
        {
            return wrap
                ? (direction == NavigationDirection.Previous ? focusableElements[^1] : focusableElements[0])
                : null;
        }

        if (direction == NavigationDirection.Previous)
        {
            if (currentIndex > 0)
            {
                return focusableElements[currentIndex - 1];
            }

            return wrap ? focusableElements[^1] : null;
        }

        if (currentIndex < focusableElements.Count - 1)
        {
            return focusableElements[currentIndex + 1];
        }

        return wrap ? focusableElements[0] : null;
    }

    private static List<InputElement> GetFocusableElements(Visual scope)
    {
        return scope
            .GetSelfAndVisualDescendants()
            .OfType<InputElement>()
            .Where(CanParticipateInTabNavigation)
            .Select((element, order) => new FocusCandidate(element, order))
            .OrderBy(candidate => candidate.Element.TabIndex)
            .ThenBy(candidate => candidate.Order)
            .Select(candidate => candidate.Element)
            .ToList();
    }

    private static bool CanParticipateInTabNavigation(InputElement element)
    {
        return element.Focusable
               && element.IsTabStop
               && element.IsEffectivelyEnabled
               && element.IsEffectivelyVisible;
    }

    private readonly record struct FocusCandidate(InputElement Element, int Order);
}
