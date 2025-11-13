// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides scale animation helpers using composition animations.
/// </summary>
public static class ScaleAnimation
{
    /// <summary>
    /// Applies a scale-in animation to the element when loaded (from 0 to 1).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetScaleIn(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, new Vector3(0, 0, 0), Vector3.One, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a scale-out animation to the element when loaded (from 1 to 0).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetScaleOut(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, Vector3.One, new Vector3(0, 0, 0), TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a zoom-in animation to the element when loaded (from 1.5 to 1).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetZoomIn(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, new Vector3(1.5f, 1.5f, 1), Vector3.One, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a zoom-out animation to the element when loaded (from 1 to 1.5).
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetZoomOut(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, Vector3.One, new Vector3(1.5f, 1.5f, 1), TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a bounce animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetBounce(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            ApplyBounce(element, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a custom scale animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="fromScaleX">The starting X scale value.</param>
    /// <param name="fromScaleY">The starting Y scale value.</param>
    /// <param name="toScaleX">The ending X scale value.</param>
    /// <param name="toScaleY">The ending Y scale value.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetCustomScale(Control element, double fromScaleX, double fromScaleY, double toScaleX, double toScaleY, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, new Vector3((float)fromScaleX, (float)fromScaleY, 1), new Vector3((float)toScaleX, (float)toScaleY, 1), TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a scale animation to the visual.
    /// </summary>
    /// <param name="visual">The visual to animate.</param>
    /// <param name="fromScale">The starting scale value.</param>
    /// <param name="toScale">The ending scale value.</param>
    /// <param name="duration">The duration of the animation.</param>
    private static void Apply(Visual visual, Vector3 fromScale, Vector3 toScale, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        if (compositionVisual is null)
        {
            return;
        }

        var compositor = compositionVisual.Compositor;

        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0.0f, fromScale);
        scaleAnimation.InsertKeyFrame(1.0f, toScale);
        scaleAnimation.Direction = PlaybackDirection.Normal;
        scaleAnimation.Duration = duration;
        scaleAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        scaleAnimation.IterationCount = 1;

        compositionVisual.StartAnimation("Scale", scaleAnimation);
    }

    /// <summary>
    /// Applies a bounce animation to the visual.
    /// </summary>
    /// <param name="visual">The visual to animate.</param>
    /// <param name="duration">The duration of the animation.</param>
    private static void ApplyBounce(Visual visual, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        if (compositionVisual is null)
        {
            return;
        }

        var compositor = compositionVisual.Compositor;

        var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0.0f, new Vector3(0, 0, 0));
        scaleAnimation.InsertKeyFrame(0.5f, new Vector3(1.1f, 1.1f, 1));
        scaleAnimation.InsertKeyFrame(0.75f, new Vector3(0.9f, 0.9f, 1));
        scaleAnimation.InsertKeyFrame(1.0f, Vector3.One);
        scaleAnimation.Direction = PlaybackDirection.Normal;
        scaleAnimation.Duration = duration;
        scaleAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        scaleAnimation.IterationCount = 1;

        compositionVisual.StartAnimation("Scale", scaleAnimation);
    }
}