// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Builds WinUI storyboards equivalent to Avalonia key-frame animations (linear interpolation between key frames).
/// </summary>
internal static class StoryboardFactory
{
    /// <summary>
    /// Creates a storyboard from animation timelines.
    /// </summary>
    /// <param name="fillBehavior">
    /// The fill behavior: <see cref="FillBehavior.Stop"/> matches Avalonia <c>FillMode.None</c>,
    /// <see cref="FillBehavior.HoldEnd"/> matches <c>FillMode.Forward</c>.
    /// </param>
    /// <param name="children">The child timelines.</param>
    /// <returns>The storyboard.</returns>
    public static Storyboard Create(FillBehavior fillBehavior, params ReadOnlySpan<Timeline> children)
    {
        Storyboard storyboard = new() { FillBehavior = fillBehavior };
        for (int i = 0; i < children.Length; i++)
        {
            children[i].FillBehavior = fillBehavior;
            storyboard.Children.Add(children[i]);
        }

        return storyboard;
    }

    /// <summary>
    /// Creates a double key-frame animation of a property path with linear key frames.
    /// </summary>
    /// <param name="targetProperty">The WinUI property path (for example <c>Opacity</c>).</param>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="keyFrames">The key times and values.</param>
    /// <returns>The animation.</returns>
    public static DoubleAnimationUsingKeyFrames CreateDoubleKeyFrames(
        string targetProperty,
        TimeSpan duration,
        params ReadOnlySpan<(TimeSpan KeyTime, double Value)> keyFrames)
    {
        DoubleAnimationUsingKeyFrames animation = new() { Duration = new Duration(duration) };
        Storyboard.SetTargetProperty(animation, targetProperty);
        for (int i = 0; i < keyFrames.Length; i++)
        {
            animation.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(keyFrames[i].KeyTime),
                Value = keyFrames[i].Value
            });
        }

        return animation;
    }
}
