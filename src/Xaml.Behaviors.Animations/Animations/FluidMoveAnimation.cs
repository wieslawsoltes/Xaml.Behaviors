// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PlatformAnimation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using PlatformAnimation = Avalonia.Animation.Animation;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Creates and runs the translation animation used for fluid layout movement.
/// </summary>
public static class FluidMoveAnimation
{
#if UNO
    private const string TranslateXPath = "(UIElement.RenderTransform).(TranslateTransform.X)";
    private const string TranslateYPath = "(UIElement.RenderTransform).(TranslateTransform.Y)";

#endif
    /// <summary>
    /// Creates an animation from a previous layout offset back to the current position.
    /// </summary>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns>The configured translation animation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duration"/> is negative.</exception>
    public static PlatformAnimation Create(double offsetX, double offsetY, TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

#if UNO
        // Avalonia FillMode.Forward: the element stays at its current position when the storyboard completes.
        return StoryboardFactory.Create(
            FillBehavior.HoldEnd,
            StoryboardFactory.CreateDoubleKeyFrames(
                TranslateXPath,
                duration,
                (TimeSpan.Zero, offsetX),
                (duration, 0d)),
            StoryboardFactory.CreateDoubleKeyFrames(
                TranslateYPath,
                duration,
                (TimeSpan.Zero, offsetY),
                (duration, 0d)));
#else
        return new PlatformAnimation
        {
            Duration = duration,
            FillMode = FillMode.Forward,
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
#endif
    }

    /// <summary>
    /// Creates and starts a fluid movement animation on a control.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    public static void Run(Control target, double offsetX, double offsetY, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(target);
        PrepareTransform(target, offsetX, offsetY);
        PlatformAnimation animation = Create(offsetX, offsetY, duration);
        AnimationRunner.TryRun(animation, target);
    }

    /// <summary>
    /// Prepares a control's translation transform and starts a fluid movement animation.
    /// </summary>
    /// <param name="target">The target control.</param>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns><c>true</c> when the target was prepared and the animation was started; otherwise, <c>false</c>.</returns>
    public static bool TryRun(Control? target, double offsetX, double offsetY, TimeSpan duration)
    {
        if (target is null)
        {
            return false;
        }

        Run(target, offsetX, offsetY, duration);
        return true;
    }

    private static void PrepareTransform(Control target, double offsetX, double offsetY)
    {
        if (target.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            target.RenderTransform = transform;
        }

        transform.X = offsetX;
        transform.Y = offsetY;
    }
}
