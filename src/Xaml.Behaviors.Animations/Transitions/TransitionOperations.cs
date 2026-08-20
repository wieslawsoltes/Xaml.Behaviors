// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Animation;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides reusable operations for a styled element's transition collection.
/// </summary>
public static class TransitionOperations
{
    /// <summary>
    /// Adds a transition to an element, creating its collection when necessary.
    /// </summary>
    /// <param name="target">The element whose transitions are changed.</param>
    /// <param name="transition">The transition to add.</param>
    /// <returns><c>true</c> when the transition was added; otherwise, <c>false</c>.</returns>
    public static bool Add(StyledElement? target, TransitionBase? transition)
    {
        if (target is null || transition is null)
        {
            return false;
        }

        target.Transitions ??= [];
        target.Transitions.Add(transition);
        return true;
    }

    /// <summary>
    /// Removes a transition from an element.
    /// </summary>
    /// <param name="target">The element whose transitions are changed.</param>
    /// <param name="transition">The transition to remove.</param>
    /// <returns><c>true</c> when the transition was removed; otherwise, <c>false</c>.</returns>
    public static bool Remove(StyledElement? target, TransitionBase? transition)
    {
        return target?.Transitions is { } transitions
            && transition is not null
            && transitions.Remove(transition);
    }

    /// <summary>
    /// Clears all transitions from an element.
    /// </summary>
    /// <param name="target">The element whose transitions are cleared.</param>
    /// <returns><c>true</c> when an existing collection was cleared; otherwise, <c>false</c>.</returns>
    public static bool Clear(StyledElement? target)
    {
        if (target?.Transitions is not { } transitions)
        {
            return false;
        }

        transitions.Clear();
        return true;
    }

    /// <summary>
    /// Replaces an element's transitions and returns the previous collection.
    /// </summary>
    /// <param name="target">The element whose transitions are replaced.</param>
    /// <param name="transitions">The new transitions collection.</param>
    /// <returns>The previous transitions collection.</returns>
    public static Transitions? Replace(StyledElement target, Transitions? transitions)
    {
        Transitions? previous = target.Transitions;
        target.Transitions = transitions;
        return previous;
    }
}
