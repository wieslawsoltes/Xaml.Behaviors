// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Xaml.Interactions.Custom;

internal static class CompositionAnimationHelpers
{
    internal readonly struct ScalarKeyFrame
    {
        public ScalarKeyFrame(float progress, float value)
        {
            Progress = progress;
            Value = value;
        }

        public float Progress { get; }
        public float Value { get; }
    }

    public static float DegreesToRadians(float degrees)
    {
        return (float)(Math.PI / 180.0) * degrees;
    }

    internal readonly struct Vector3KeyFrame
    {
        public Vector3KeyFrame(float progress, Vector3 value)
        {
            Progress = progress;
            Value = value;
        }

        public float Progress { get; }
        public Vector3 Value { get; }
    }

    public static void EnsureCenterPoint(Control element, CompositionVisual compositionVisual)
    {
        var bounds = element.Bounds;
        if (bounds.Width > 0 && bounds.Height > 0)
        {
            compositionVisual.CenterPoint = new Vector3((float)(bounds.Width * 0.5), (float)(bounds.Height * 0.5), 0f);
        }
        else
        {
            var halfX = (float)(compositionVisual.Size.X * 0.5);
            var halfY = (float)(compositionVisual.Size.Y * 0.5);
            compositionVisual.CenterPoint = new Vector3(halfX, halfY, 0f);
        }
    }

    public static void SetNormalizedCenterPoint(Control element, CompositionVisual compositionVisual, Vector2 anchor)
    {
        var bounds = element.Bounds;
        if (bounds.Width > 0 && bounds.Height > 0)
        {
            compositionVisual.CenterPoint = new Vector3(
                (float)(bounds.Width * anchor.X),
                (float)(bounds.Height * anchor.Y),
                0f);
        }
        else
        {
            compositionVisual.CenterPoint = new Vector3(
                (float)(compositionVisual.Size.X * anchor.X),
                (float)(compositionVisual.Size.Y * anchor.Y),
                0f);
        }
    }

    public static void StartVector3Animation(CompositionVisual visual, string propertyName, TimeSpan duration, IReadOnlyList<Vector3KeyFrame> keyFrames)
    {
        if (keyFrames.Count == 0)
        {
            return;
        }

        var animation = visual.Compositor.CreateVector3KeyFrameAnimation();
        foreach (var keyFrame in keyFrames)
        {
            animation.InsertKeyFrame(keyFrame.Progress, keyFrame.Value);
        }

        ConfigureAndStartAnimation(visual, propertyName, duration, animation);
    }

    public static void StartScalarAnimation(CompositionVisual visual, string propertyName, TimeSpan duration, IReadOnlyList<ScalarKeyFrame> keyFrames)
    {
        if (keyFrames.Count == 0)
        {
            return;
        }

        var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        foreach (var keyFrame in keyFrames)
        {
            animation.InsertKeyFrame(keyFrame.Progress, keyFrame.Value);
        }

        ConfigureAndStartAnimation(visual, propertyName, duration, animation);
    }

    private static void ConfigureAndStartAnimation(CompositionVisual visual, string propertyName, TimeSpan duration, CompositionAnimation animation)
    {
        if (animation is KeyFrameAnimation keyFrameAnimation)
        {
            keyFrameAnimation.Direction = PlaybackDirection.Normal;
            keyFrameAnimation.Duration = duration;
            keyFrameAnimation.IterationBehavior = AnimationIterationBehavior.Count;
            keyFrameAnimation.IterationCount = 1;
        }

        visual.StartAnimation(propertyName, animation);
    }
}
