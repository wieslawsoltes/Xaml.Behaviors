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
        return FindBoundary(GetTabOrderEntries(scope), direction);
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
        var entries = GetTabOrderEntries(scope);
        if (entries.Count == 0)
        {
            return null;
        }

        var currentIndex = FindCurrentEntryIndex(entries, current, scope);
        if (currentIndex < 0)
        {
            return wrap ? FindBoundary(entries, direction) : null;
        }

        var step = direction == NavigationDirection.Previous ? -1 : 1;
        for (var index = currentIndex + step; index >= 0 && index < entries.Count; index += step)
        {
            if (entries[index].Target is not null)
            {
                return entries[index].Target;
            }
        }

        return wrap ? FindBoundary(entries, direction) : null;
    }

    private static int FindCurrentEntryIndex(IReadOnlyList<TabOrderEntry> entries, InputElement current, Visual scope)
    {
        for (var index = 0; index < entries.Count; index++)
        {
            if (ReferenceEquals(entries[index].Target, current))
            {
                return index;
            }
        }

        for (Visual? visual = current.GetVisualParent(); visual is not null; visual = visual.GetVisualParent())
        {
            if (visual is InputElement inputElement)
            {
                for (var index = 0; index < entries.Count; index++)
                {
                    if (ReferenceEquals(entries[index].Owner, inputElement))
                    {
                        return index;
                    }
                }
            }

            if (ReferenceEquals(visual, scope))
            {
                break;
            }
        }

        return -1;
    }

    private static List<TabOrderEntry> GetTabOrderEntries(Visual scope)
    {
        var entries = new List<TabOrderEntry>();
        BuildTabOrderEntries(scope, entries, includeSelf: true);
        return entries;
    }

    private static void BuildTabOrderEntries(Visual scope, List<TabOrderEntry> entries, bool includeSelf)
    {
        if (scope is InputElement inputElement && includeSelf && CanParticipateInTabNavigation(inputElement))
        {
            entries.Add(new TabOrderEntry(inputElement, inputElement));
        }

        foreach (var childVisual in GetOrderedVisualChildren(scope))
        {
            if (childVisual is not InputElement childInputElement)
            {
                BuildTabOrderEntries(childVisual, entries, includeSelf: false);
                continue;
            }

            var mode = KeyboardNavigation.GetTabNavigation(childInputElement);
            switch (mode)
            {
                case KeyboardNavigationMode.None:
                {
                    if (CanParticipateInTabNavigation(childInputElement))
                    {
                        entries.Add(new TabOrderEntry(childInputElement, childInputElement));
                    }
                    else
                    {
                        entries.Add(new TabOrderEntry(null, childInputElement));
                    }

                    break;
                }
                case KeyboardNavigationMode.Once:
                {
                    var onceTarget = ResolveTabOnceTarget(childInputElement);
                    if (onceTarget is not null)
                    {
                        entries.Add(new TabOrderEntry(onceTarget, childInputElement));
                    }
                    else if (CanParticipateInTabNavigation(childInputElement))
                    {
                        entries.Add(new TabOrderEntry(childInputElement, childInputElement));
                    }
                    else
                    {
                        entries.Add(new TabOrderEntry(null, childInputElement));
                    }

                    break;
                }
                default:
                    BuildTabOrderEntries(childInputElement, entries, includeSelf: true);
                    break;
            }
        }
    }

    private static InputElement? ResolveTabOnceTarget(InputElement container)
    {
        if (KeyboardNavigation.GetTabOnceActiveElement(container) is InputElement activeElement
            && IsWithinScope(container, activeElement)
            && CanParticipateInTabNavigation(activeElement))
        {
            return activeElement;
        }

        var childEntries = new List<TabOrderEntry>();
        foreach (var childVisual in GetOrderedVisualChildren(container))
        {
            if (childVisual is InputElement childInputElement)
            {
                BuildTabOrderEntries(childInputElement, childEntries, includeSelf: true);
            }
            else
            {
                BuildTabOrderEntries(childVisual, childEntries, includeSelf: false);
            }
        }

        return FindBoundary(childEntries, NavigationDirection.Next);
    }

    private static IEnumerable<Visual> GetOrderedVisualChildren(Visual scope)
    {
        return scope.GetVisualChildren()
            .OfType<Visual>()
            .Select((child, order) => new VisualOrder(child, order))
            .OrderBy(item => item.Visual is InputElement inputElement ? inputElement.TabIndex : int.MaxValue)
            .ThenBy(item => item.Order)
            .Select(item => item.Visual);
    }

    private static bool CanParticipateInTabNavigation(InputElement element)
    {
        return element.Focusable
               && element.IsTabStop
               && element.IsEffectivelyEnabled
               && element.IsEffectivelyVisible;
    }

    private static InputElement? FindBoundary(IReadOnlyList<TabOrderEntry> entries, NavigationDirection direction)
    {
        if (direction == NavigationDirection.Previous)
        {
            for (var index = entries.Count - 1; index >= 0; index--)
            {
                if (entries[index].Target is not null)
                {
                    return entries[index].Target;
                }
            }

            return null;
        }

        foreach (var entry in entries)
        {
            if (entry.Target is not null)
            {
                return entry.Target;
            }
        }

        return null;
    }

    private static bool IsWithinScope(Visual scope, InputElement candidate)
    {
        return candidate is Visual candidateVisual
            && (ReferenceEquals(scope, candidateVisual) || scope.IsVisualAncestorOf(candidateVisual));
    }

    private readonly record struct TabOrderEntry(InputElement? Target, InputElement Owner);

    private readonly record struct VisualOrder(Visual Visual, int Order);
}
