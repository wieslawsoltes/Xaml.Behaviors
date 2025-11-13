// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides fade animation helpers using composition animations.
/// </summary>
public static class FadeAnimation
{
    /// <summary>
    /// Applies a fade-in animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetFadeIn(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 0.0f, 1.0f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a fade-out animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetFadeOut(Control element, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, 1.0f, 0.0f, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies a custom fade animation to the element when loaded.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="fromOpacity">The starting opacity value (0.0 to 1.0).</param>
    /// <param name="toOpacity">The ending opacity value (0.0 to 1.0).</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds.</param>
    public static void SetCustomFade(Control element, double fromOpacity, double toOpacity, double milliseconds)
    {
        element.Loaded += (_, _) =>
        {
            Apply(element, (float)fromOpacity, (float)toOpacity, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    /// <summary>
    /// Applies an opacity animation to the visual.
    /// </summary>
    /// <param name="visual">The visual to animate.</param>
    /// <param name="fromOpacity">The starting opacity value.</param>
    /// <param name="toOpacity">The ending opacity value.</param>
    /// <param name="duration">The duration of the animation.</param>
    private static void Apply(Visual visual, float fromOpacity, float toOpacity, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual);
        if (compositionVisual is null)
        {
            return;
        }

        var compositor = compositionVisual.Compositor;

        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.InsertKeyFrame(0.0f, fromOpacity);
        opacityAnimation.InsertKeyFrame(1.0f, toOpacity);
        opacityAnimation.Direction = PlaybackDirection.Normal;
        opacityAnimation.Duration = duration;
        opacityAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        opacityAnimation.IterationCount = 1;

        compositionVisual.StartAnimation("Opacity", opacityAnimation);
    }
}