// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Numerics;
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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

        ValidateDuration(duration);
        if (duration == TimeSpan.Zero)
        {
            SetVector3Value(visual, propertyName, keyFrames[keyFrames.Count - 1].Value);
            return;
        }

        var animation = visual.Compositor.CreateVector3KeyFrameAnimation();
        foreach (var keyFrame in keyFrames)
        {
            animation.InsertKeyFrame(keyFrame.Progress, keyFrame.Value);
        }

        ConfigureAndStartAnimation(visual, propertyName, duration, animation);
    }

    public static void StartOffsetAnimation(Control element, CompositionVisual visual, TimeSpan duration, IReadOnlyList<Vector3KeyFrame> keyFrames)
    {
        if (keyFrames.Count == 0)
        {
            return;
        }

        Vector3 layoutOffset = GetLayoutOffset(element);
        ValidateDuration(duration);
        if (duration == TimeSpan.Zero)
        {
            SetOffset(visual, layoutOffset + keyFrames[keyFrames.Count - 1].Value);
            return;
        }

        Vector3KeyFrameAnimation animation = visual.Compositor.CreateVector3KeyFrameAnimation();
        foreach (Vector3KeyFrame keyFrame in keyFrames)
        {
            animation.InsertKeyFrame(keyFrame.Progress, layoutOffset + keyFrame.Value);
        }

        ConfigureAndStartAnimation(visual, OffsetPropertyName, duration, animation);
    }

    /// <summary>
    /// Sets the composition offset of an element visual, relative to its layout position on Uno Platform and WinUI.
    /// </summary>
    /// <remarks>
    /// Native WinUI layout owns <c>Visual.Offset</c> (the arranged position of the element): the offset is the
    /// <c>Translation</c> of the visual there, which <c>ElementComposition.GetElementVisual</c> enables.
    /// </remarks>
    public static void SetOffset(CompositionVisual visual, Vector3 value)
    {
#if WINUI
        visual.Properties.InsertVector3(OffsetPropertyName, value);
#else
        visual.Offset = value;
#endif
    }

    /// <summary>
    /// The composition property animated for offsets: <c>Translation</c> on native WinUI, <c>Offset</c> elsewhere.
    /// </summary>
#if WINUI
    internal const string OffsetPropertyName = "Translation";
#else
    internal const string OffsetPropertyName = "Offset";
#endif

    public static Vector3 GetLayoutOffset(Control element)
    {
#if UNO
        // WinUI composes Visual.Offset on top of the arranged position of the element, so composition offsets are
        // already relative to the layout slot.
        return Vector3.Zero;
#else
        Rect bounds = element.Bounds;
        return new Vector3((float)bounds.X, (float)bounds.Y, 0f);
#endif
    }

    public static Vector3 GetLayoutOffset(Control element, Vector3 relativeOffset)
    {
        return GetLayoutOffset(element) + relativeOffset;
    }

    public static void StartScalarAnimation(CompositionVisual visual, string propertyName, TimeSpan duration, IReadOnlyList<ScalarKeyFrame> keyFrames)
    {
        if (keyFrames.Count == 0)
        {
            return;
        }

        ValidateDuration(duration);
        if (duration == TimeSpan.Zero)
        {
            SetScalarValue(visual, propertyName, keyFrames[keyFrames.Count - 1].Value);
            return;
        }

        var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        foreach (var keyFrame in keyFrames)
        {
            animation.InsertKeyFrame(keyFrame.Progress, keyFrame.Value);
        }

        ConfigureAndStartAnimation(visual, propertyName, duration, animation);
    }

    public static void StartOrientationAnimation(CompositionVisual visual, Quaternion orientation, TimeSpan duration)
    {
#if UNO
        // Uno Platform (Skia) implements neither QuaternionKeyFrameAnimation nor a CenterPoint for Visual.Orientation,
        // so the orientation is animated as a rotation around its axis (RotationAxis/RotationAngle use CenterPoint).
        if (TryGetAxisAngle(orientation, out Vector3 axis, out float angle))
        {
            visual.RotationAxis = axis;
        }

        ScalarKeyFrameAnimation animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1f, angle);
        animation.Duration = duration;
        visual.StartAnimation("RotationAngle", animation);
#else
        var animation = visual.Compositor.CreateQuaternionKeyFrameAnimation();
        animation.InsertKeyFrame(1f, orientation);
        animation.Duration = duration;
        visual.StartAnimation("Orientation", animation);
#endif
    }

#if UNO
    internal static bool TryGetAxisAngle(Quaternion orientation, out Vector3 axis, out float angle)
    {
        Quaternion normalized = Quaternion.Normalize(orientation);
        if (normalized.W < 0f)
        {
            normalized = Quaternion.Negate(normalized);
        }

        Vector3 vector = new(normalized.X, normalized.Y, normalized.Z);
        float length = vector.Length();
        if (length <= 1e-6f || float.IsNaN(length))
        {
            axis = Vector3.UnitZ;
            angle = 0f;
            return false;
        }

        axis = vector / length;
        angle = 2f * MathF.Atan2(length, normalized.W);
        return true;
    }

#endif
    private static void SetVector3Value(CompositionVisual visual, string propertyName, Vector3 value)
    {
        switch (propertyName)
        {
            case "Offset":
                SetOffset(visual, value);
                break;
            case "Scale":
                visual.Scale = value;
                break;
            default:
                throw new ArgumentException($"Unsupported Vector3 composition property '{propertyName}'.", nameof(propertyName));
        }
    }

    private static void SetScalarValue(CompositionVisual visual, string propertyName, float value)
    {
        switch (propertyName)
        {
            case "Opacity":
                visual.Opacity = value;
                break;
            case "RotationAngle":
                visual.RotationAngle = value;
                break;
            default:
                throw new ArgumentException($"Unsupported scalar composition property '{propertyName}'.", nameof(propertyName));
        }
    }

    private static void ValidateDuration(TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);
    }

    private static void ConfigureAndStartAnimation(CompositionVisual visual, string propertyName, TimeSpan duration, CompositionAnimation animation)
    {
        if (animation is KeyFrameAnimation keyFrameAnimation)
        {
#if !UNO
            // Normal is the default playback direction; Uno Platform does not implement KeyFrameAnimation.Direction.
            keyFrameAnimation.Direction = PlaybackDirection.Normal;
#endif
            keyFrameAnimation.Duration = duration;
            keyFrameAnimation.IterationBehavior = AnimationIterationBehavior.Count;
            keyFrameAnimation.IterationCount = 1;
        }

        visual.StartAnimation(propertyName == "Offset" ? OffsetPropertyName : propertyName, animation);
    }
}
