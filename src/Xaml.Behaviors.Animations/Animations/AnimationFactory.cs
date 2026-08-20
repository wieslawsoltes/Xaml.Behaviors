// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Animation;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Creates commonly used Avalonia key-frame animations.
/// </summary>
public static class AnimationFactory
{
    /// <summary>
    /// Creates an opacity animation that holds at zero during the initial delay and then fades to one.
    /// </summary>
    /// <param name="initialDelay">The delay before the fade begins.</param>
    /// <param name="duration">The duration of the fade.</param>
    /// <returns>The configured fade-in animation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="initialDelay"/> or <paramref name="duration"/> is negative.
    /// </exception>
    public static Animation.Animation CreateFadeIn(TimeSpan initialDelay, TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(initialDelay, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);

        TimeSpan totalDuration = initialDelay + duration;
        return CreateFadeInTimeline(initialDelay, totalDuration, totalDuration);
    }

    /// <summary>
    /// Creates an opacity animation with explicit hold, completion, and total times.
    /// </summary>
    /// <param name="initialDelay">The key time through which opacity remains zero.</param>
    /// <param name="fadeCompletionTime">The key time at which opacity becomes one.</param>
    /// <param name="totalDuration">The total animation duration.</param>
    /// <returns>The configured fade-in animation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any supplied time is negative or the key times are not chronological.
    /// </exception>
    public static Animation.Animation CreateFadeInTimeline(
        TimeSpan initialDelay,
        TimeSpan fadeCompletionTime,
        TimeSpan totalDuration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(initialDelay, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(fadeCompletionTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(totalDuration, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(initialDelay, fadeCompletionTime);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fadeCompletionTime, totalDuration);

        return new Animation.Animation
        {
            Duration = totalDuration,
            Children =
            {
                new KeyFrame
                {
                    KeyTime = TimeSpan.Zero,
                    Setters = { new Setter(Visual.OpacityProperty, 0d) }
                },
                new KeyFrame
                {
                    KeyTime = initialDelay,
                    Setters = { new Setter(Visual.OpacityProperty, 0d) }
                },
                new KeyFrame
                {
                    KeyTime = fadeCompletionTime,
                    Setters = { new Setter(Visual.OpacityProperty, 1d) }
                }
            }
        };
    }
}
