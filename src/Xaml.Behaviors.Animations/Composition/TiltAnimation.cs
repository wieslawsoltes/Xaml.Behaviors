// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Calculates and applies composition-based pointer tilt animations.
/// </summary>
public static class TiltAnimation
{
    /// <summary>
    /// Calculates a tilt orientation for a pointer position inside a target size.
    /// </summary>
    /// <param name="size">The target size.</param>
    /// <param name="pointerPosition">The pointer position relative to the target.</param>
    /// <param name="tiltStrength">The maximum tilt angle in degrees.</param>
    /// <returns>The calculated orientation, or identity for an empty size or central position.</returns>
    public static Quaternion CalculateOrientation(Size size, Point pointerPosition, double tiltStrength)
    {
        return TryCalculateOrientation(size, pointerPosition, tiltStrength, out Quaternion orientation)
            ? orientation
            : Quaternion.Identity;
    }

    /// <summary>
    /// Tries to calculate a non-neutral tilt orientation for a pointer position inside a target size.
    /// </summary>
    /// <param name="size">The target size.</param>
    /// <param name="pointerPosition">The pointer position relative to the target.</param>
    /// <param name="tiltStrength">The maximum tilt angle in degrees.</param>
    /// <param name="orientation">The calculated orientation when the method returns <c>true</c>.</param>
    /// <returns><c>true</c> when a non-neutral orientation was calculated; otherwise, <c>false</c>.</returns>
    public static bool TryCalculateOrientation(
        Size size,
        Point pointerPosition,
        double tiltStrength,
        out Quaternion orientation)
    {
        orientation = Quaternion.Identity;
        if (size.Width <= 0d || size.Height <= 0d)
        {
            return false;
        }

        double centerX = size.Width / 2d;
        double centerY = size.Height / 2d;
        double xDifference = (pointerPosition.X - centerX) / centerX;
        double yDifference = (pointerPosition.Y - centerY) / centerY;
        var axis = new Vector3((float)-yDifference, (float)xDifference, 0f);
        if (axis.LengthSquared() <= 0.001f)
        {
            return false;
        }

        axis = Vector3.Normalize(axis);
        double distance = Math.Min(Math.Sqrt((xDifference * xDifference) + (yDifference * yDifference)), 1d);
        float angle = (float)(distance * tiltStrength * (Math.PI / 180d));
        orientation = Quaternion.CreateFromAxisAngle(axis, angle);
        return true;
    }

    /// <summary>
    /// Applies a tilt animation for a pointer position.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <param name="pointerPosition">The pointer position relative to the target.</param>
    /// <param name="tiltStrength">The maximum tilt angle in degrees.</param>
    /// <returns><c>true</c> when a composition visual was animated; otherwise, <c>false</c>.</returns>
    public static bool Apply(Control? target, Point pointerPosition, double tiltStrength)
    {
        if (target is null)
        {
            return false;
        }

        return TryCalculateOrientation(target.Bounds.Size, pointerPosition, tiltStrength, out Quaternion orientation)
            && AnimateTo(target, orientation, TimeSpan.FromMilliseconds(50));
    }

    /// <summary>
    /// Animates a control back to its neutral orientation.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <returns><c>true</c> when a composition visual was animated; otherwise, <c>false</c>.</returns>
    public static bool Reset(Control? target)
    {
        return AnimateTo(target, Quaternion.Identity, TimeSpan.FromMilliseconds(400));
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

    private static bool AnimateTo(Control? target, Quaternion orientation, TimeSpan duration)
    {
        CompositionVisual? visual = target is null ? null : ElementComposition.GetElementVisual(target);
        if (visual is null)
        {
            return false;
        }

        var animation = visual.Compositor.CreateQuaternionKeyFrameAnimation();
        animation.InsertKeyFrame(1f, orientation);
        animation.Duration = duration;
        visual.StartAnimation("Orientation", animation);
        return true;
    }
}
