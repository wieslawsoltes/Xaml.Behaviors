// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Creates and runs the translation animation used for fluid layout movement.
/// </summary>
public static class FluidMoveAnimation
{
    /// <summary>
    /// Creates an animation from a previous layout offset back to the current position.
    /// </summary>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns>The configured translation animation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duration"/> is negative.</exception>
    public static Animation.Animation Create(double offsetX, double offsetY, TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        return new Animation.Animation
        {
            Duration = duration,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, offsetX),
                        new Setter(TranslateTransform.YProperty, offsetY)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d),
                        new Setter(TranslateTransform.YProperty, 0d)
                    }
                }
            }
        };
    }

    /// <summary>
    /// Creates and starts a fluid movement animation on a translation transform.
    /// </summary>
    /// <param name="target">The target transform.</param>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    public static void Run(TranslateTransform target, double offsetX, double offsetY, TimeSpan duration)
    {
        Animation.Animation animation = Create(offsetX, offsetY, duration);
        AnimationRunner.TryRun(animation, target);
    }
}
