// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Applies composition-based parallax offsets without requiring a behavior.
/// </summary>
public sealed class ParallaxAnimation
{
    private readonly Control _target;
    private readonly CompositionVisual _visual;

    private ParallaxAnimation(Control target, CompositionVisual visual)
    {
        _target = target;
        _visual = visual;
    }

    /// <summary>
    /// Calculates the composition offset for a scroll offset and parallax ratio.
    /// </summary>
    /// <param name="scrollOffset">The current scroll offset.</param>
    /// <param name="parallaxRatio">The proportion of the scroll offset to apply.</param>
    /// <returns>The parallax delta to add to the target's layout offset.</returns>
    public static Vector3 CalculateOffset(Vector scrollOffset, double parallaxRatio)
    {
        return new Vector3(
            (float)(scrollOffset.X * parallaxRatio),
            (float)(scrollOffset.Y * parallaxRatio),
            0f);
    }

    /// <summary>
    /// Creates a parallax animation session that retains the target's composition visual.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <returns>A parallax animation session, or <c>null</c> when the target visual is unavailable.</returns>
    public static ParallaxAnimation? TryCreate(Control? target)
    {
        if (target is null)
        {
            return null;
        }

        CompositionVisual? visual = ElementComposition.GetElementVisual(target);
        return visual is null ? null : new ParallaxAnimation(target, visual);
    }

    /// <summary>
    /// Applies a calculated parallax offset to the retained composition visual.
    /// </summary>
    /// <param name="scrollOffset">The current scroll offset.</param>
    /// <param name="parallaxRatio">The proportion of the scroll offset to apply.</param>
    public void Apply(Vector scrollOffset, double parallaxRatio)
    {
        Vector3 delta = CalculateOffset(scrollOffset, parallaxRatio);
        Rect bounds = _target.Bounds;
        _visual.Offset = new Vector3(
            (float)bounds.Left + delta.X,
            (float)bounds.Top + delta.Y,
            delta.Z);
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
        ParallaxAnimation? animation = TryCreate(target);
        if (animation is null)
        {
            return false;
        }

        animation.Apply(scrollOffset, parallaxRatio);
        return true;
    }
}
