// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Applies composition-based parallax offsets without requiring a behavior.
/// </summary>
public static class ParallaxAnimation
{
    /// <summary>
    /// Calculates the composition offset for a scroll offset and parallax ratio.
    /// </summary>
    /// <param name="scrollOffset">The current scroll offset.</param>
    /// <param name="parallaxRatio">The proportion of the scroll offset to apply.</param>
    /// <returns>The composition visual offset.</returns>
    public static Vector3 CalculateOffset(Vector scrollOffset, double parallaxRatio)
    {
        return new Vector3(
            (float)(scrollOffset.X * parallaxRatio),
            (float)(scrollOffset.Y * parallaxRatio),
            0f);
    }

    /// <summary>
    /// Applies a calculated parallax offset to a control's composition visual.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <param name="scrollOffset">The current scroll offset.</param>
    /// <param name="parallaxRatio">The proportion of the scroll offset to apply.</param>
    /// <returns><c>true</c> when a composition visual was updated; otherwise, <c>false</c>.</returns>
    public static bool Apply(Control? target, Vector scrollOffset, double parallaxRatio)
    {
        CompositionVisual? visual = target is null ? null : ElementComposition.GetElementVisual(target);
        if (visual is null)
        {
            return false;
        }

        visual.Offset = CalculateOffset(scrollOffset, parallaxRatio);
        return true;
    }
}
