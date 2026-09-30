// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Runs the translation animation used for fluid layout movement (Uno Platform counterpart of the Avalonia
/// <c>FluidMoveAnimation</c> of Xaml.Behaviors.Animations).
/// </summary>
internal static class FluidMoveAnimation
{
    /// <summary>
    /// Prepares the translation transform of <paramref name="target"/> and animates it from the previous layout offset
    /// back to the current position.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <param name="offsetX">The previous horizontal offset.</param>
    /// <param name="offsetY">The previous vertical offset.</param>
    /// <param name="duration">The animation duration.</param>
    /// <returns><c>true</c> when the animation was started; otherwise, <c>false</c>.</returns>
    public static bool TryRun(FrameworkElement? target, double offsetX, double offsetY, TimeSpan duration)
    {
        if (target is null)
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        if (target.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            target.RenderTransform = transform;
        }

        transform.X = offsetX;
        transform.Y = offsetY;

        var storyboard = new Storyboard();
        storyboard.Children.Add(CreateAnimation(transform, nameof(TranslateTransform.X), offsetX, duration));
        storyboard.Children.Add(CreateAnimation(transform, nameof(TranslateTransform.Y), offsetY, duration));
        storyboard.Begin();
        return true;
    }

    private static DoubleAnimation CreateAnimation(TranslateTransform transform, string property, double from, TimeSpan duration)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = 0d,
            Duration = new Duration(duration),
            FillBehavior = FillBehavior.HoldEnd,
        };
        Storyboard.SetTarget(animation, transform);
        Storyboard.SetTargetProperty(animation, property);
        return animation;
    }
}
