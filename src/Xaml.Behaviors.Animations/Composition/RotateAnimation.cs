// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides rotation animation helpers using composition animations.
/// </summary>
public static class RotateAnimation
{
    /// <summary>
    /// Applies a clockwise rotation animation to the element when loaded (0 to 360 degrees).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetRotateClockwise(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 0f, 360f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a counter-clockwise rotation animation to the element when loaded (0 to -360 degrees).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetRotateCounterClockwise(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 0f, -360f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a rotate-in animation to the element when loaded (from -180 to 0 degrees).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetRotateIn(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, -180f, 0f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a rotate-out animation to the element when loaded (from 0 to 180 degrees).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetRotateOut(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 0f, 180f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a flip animation to the element when loaded (180 degree rotation).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetFlip(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 0f, 180f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a swing animation to the element when loaded (back and forth rotation).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetSwing(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            ApplySwing(element, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a custom rotation animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="fromAngle">The starting rotation angle in degrees.</param>
    /// <param name="toAngle">The ending rotation angle in degrees.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetCustomRotate(Control element, double fromAngle, double toAngle, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, (float)fromAngle, (float)toAngle, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a rotation animation to the visual.
    /// </summary>
    /// <param name="visual">The visual to animate.</param>
    /// <param name="fromAngle">The starting rotation angle in degrees.</param>
    /// <param name="toAngle">The ending rotation angle in degrees.</param>
    /// <param name="duration">The duration of the animation.</param>
    private static void Apply(Visual visual, float fromAngle, float toAngle, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        if (compositionVisual is null)
        {
            return;
        }

        var compositor = compositionVisual.Compositor;

        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        var fromRadians = CompositionAnimationHelpers.DegreesToRadians(fromAngle);
        var toRadians = CompositionAnimationHelpers.DegreesToRadians(toAngle);
        rotationAnimation.InsertKeyFrame(0.0f, fromRadians);
        rotationAnimation.InsertKeyFrame(1.0f, toRadians);
        rotationAnimation.Direction = PlaybackDirection.Normal;
        rotationAnimation.Duration = duration;
        rotationAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        rotationAnimation.IterationCount = 1;

        compositionVisual.StartAnimation("RotationAngle", rotationAnimation);
    }

    /// <summary>
    /// Applies a swing animation to the visual.
    /// </summary>
    /// <param name="visual">The visual to animate.</param>
    /// <param name="duration">The duration of the animation.</param>
    private static void ApplySwing(Visual visual, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        if (compositionVisual is null)
        {
            return;
        }

        var compositor = compositionVisual.Compositor;

        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0.0f, 0f);
        rotationAnimation.InsertKeyFrame(0.25f, CompositionAnimationHelpers.DegreesToRadians(15f));
        rotationAnimation.InsertKeyFrame(0.5f, 0f);
        rotationAnimation.InsertKeyFrame(0.75f, CompositionAnimationHelpers.DegreesToRadians(-15f));
        rotationAnimation.InsertKeyFrame(1.0f, 0f);
        rotationAnimation.Direction = PlaybackDirection.Normal;
        rotationAnimation.Duration = duration;
        rotationAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        rotationAnimation.IterationCount = 1;

        compositionVisual.StartAnimation("RotationAngle", rotationAnimation);
    }
}
