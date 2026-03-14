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
        var focusableElements = GetFocusableElements(scope);
        if (focusableElements.Count == 0)
        {
            return null;
        }

        var currentElement = current as InputElement;
        var currentIndex = currentElement is null
            ? -1
            : focusableElements.IndexOf(currentElement);

        if (direction == NavigationDirection.Previous)
        {
            if (currentIndex > 0)
            {
                return focusableElements[currentIndex - 1];
            }

            return wrap ? focusableElements[^1] : null;
        }

        if (currentIndex >= 0 && currentIndex < focusableElements.Count - 1)
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
