// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides special animation helpers that do not fit other categories.
/// </summary>
public static class SpecialAnimations
{
    /// <summary>
    /// Applies an animate.css style <c>flip</c> animation approximation using composition animations.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFlip(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyFlip, ensureCenterPoint: true);

    private static void Run(Control element, double milliseconds, Action<Control, CompositionVisual, TimeSpan> animation, bool ensureCenterPoint = false, Vector2? anchor = null)
    {
        element.Loaded += (_, _) =>
        {
            var visual = ElementComposition.GetElementVisual(element);
            if (visual is null)
            {
                return;
            }

            if (anchor.HasValue)
            {
                CompositionAnimationHelpers.SetNormalizedCenterPoint(element, visual, anchor.Value);
            }
            else if (ensureCenterPoint)
            {
                CompositionAnimationHelpers.EnsureCenterPoint(element, visual);
            }

            animation(element, visual, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    private static void ApplyFlip(Control element, CompositionVisual visual, TimeSpan duration)
    {
        var rotationFrames = new[]
        {
            new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(-360f)),
            new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, CompositionAnimationHelpers.DegreesToRadians(-190f)),
            new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, CompositionAnimationHelpers.DegreesToRadians(-170f)),
            new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, CompositionAnimationHelpers.DegreesToRadians(-60f)),
            new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, CompositionAnimationHelpers.DegreesToRadians(-30f)),
            new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
        };

        var scaleFrames = new[]
        {
            new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(1f, 1f, 1f)),
            new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(1.0f, 1.0f, 1f)),
            new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(1.1f, 1.1f, 1f)),
            new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(1.1f, 1.1f, 1f)),
            new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(1.05f, 1.05f, 1f)),
            new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(1f, 1f, 1f))
        };

        CompositionAnimationHelpers.StartScalarAnimation(visual, "RotationAngle", duration, rotationFrames);
        CompositionAnimationHelpers.StartVector3Animation(visual, "Scale", duration, scaleFrames);
    }
}
