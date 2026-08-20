// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Maintains and applies a composition orientation for an orbit-style pointer interaction.
/// </summary>
public sealed class OrbitAnimation
{
    /// <summary>
    /// Gets the current orientation.
    /// </summary>
    public Quaternion Orientation { get; private set; } = Quaternion.Identity;

    /// <summary>
    /// Calculates the next normalized orientation from a pointer delta.
    /// </summary>
    /// <param name="orientation">The current orientation.</param>
    /// <param name="pointerDelta">The pointer movement since the previous update.</param>
    /// <param name="sensitivity">The rotation sensitivity.</param>
    /// <returns>The next normalized orientation.</returns>
    public static Quaternion CalculateOrientation(
        Quaternion orientation,
        Vector pointerDelta,
        double sensitivity)
    {
        float rotationAroundX = (float)(pointerDelta.Y * sensitivity * 0.01);
        float rotationAroundY = (float)(pointerDelta.X * sensitivity * 0.01);
        Quaternion rotationX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -rotationAroundX);
        Quaternion rotationY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotationAroundY);
        return Quaternion.Normalize(orientation * rotationX * rotationY);
    }

    /// <summary>
    /// Updates the orientation and applies it to a control's composition visual.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <param name="pointerDelta">The pointer movement since the previous update.</param>
    /// <param name="sensitivity">The rotation sensitivity.</param>
    /// <returns><c>true</c> when a composition visual was animated; otherwise, <c>false</c>.</returns>
    public bool Rotate(Control? target, Vector pointerDelta, double sensitivity)
    {
        CompositionVisual? visual = target is null ? null : ElementComposition.GetElementVisual(target);
        if (visual is null)
        {
            return false;
        }

        Orientation = CalculateOrientation(Orientation, pointerDelta, sensitivity);
        var animation = visual.Compositor.CreateQuaternionKeyFrameAnimation();
        animation.InsertKeyFrame(1f, Orientation);
        animation.Duration = TimeSpan.FromMilliseconds(1);
        visual.StartAnimation("Orientation", animation);
        return true;
    }

    /// <summary>
    /// Updates a control's composition center point from its current bounds.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <returns><c>true</c> when a composition visual was updated; otherwise, <c>false</c>.</returns>
    public static bool UpdateCenterPoint(Control? target)
    {
        CompositionVisual? visual = target is null ? null : ElementComposition.GetElementVisual(target);
        if (visual is null || target is null)
        {
            return false;
        }

        visual.CenterPoint = new Vector3(
            (float)target.Bounds.Width / 2f,
            (float)target.Bounds.Height / 2f,
            0f);
        return true;
    }

    /// <summary>
    /// Resets the maintained orientation to the identity quaternion.
    /// </summary>
    public void Reset()
    {
        Orientation = Quaternion.Identity;
    }
}
