// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Numerics;

namespace Avalonia.Xaml.Interactions.Custom;

internal sealed class CompositionAnimationDefinition
{
    public CompositionAnimationDefinition(
        CompositionAnimationHelpers.Vector3KeyFrame[]? offsetFrames = null,
        CompositionAnimationHelpers.Vector3KeyFrame[]? scaleFrames = null,
        CompositionAnimationHelpers.ScalarKeyFrame[]? opacityFrames = null,
        CompositionAnimationHelpers.ScalarKeyFrame[]? rotationFrames = null,
        Vector2? anchor = null,
        Vector3? initialOffset = null,
        Vector3? initialScale = null,
        float? initialOpacity = null,
        float? initialRotation = null,
        bool ensureCenterPoint = false)
    {
        OffsetFrames = offsetFrames;
        ScaleFrames = scaleFrames;
        OpacityFrames = opacityFrames;
        RotationFrames = rotationFrames;
        Anchor = anchor;
        InitialOffset = initialOffset;
        InitialScale = initialScale;
        InitialOpacity = initialOpacity;
        InitialRotation = initialRotation;
        EnsureCenterPoint = ensureCenterPoint;
    }

    public CompositionAnimationHelpers.Vector3KeyFrame[]? OffsetFrames { get; }
    public CompositionAnimationHelpers.Vector3KeyFrame[]? ScaleFrames { get; }
    public CompositionAnimationHelpers.ScalarKeyFrame[]? OpacityFrames { get; }
    public CompositionAnimationHelpers.ScalarKeyFrame[]? RotationFrames { get; }
    public Vector2? Anchor { get; }
    public Vector3? InitialOffset { get; }
    public Vector3? InitialScale { get; }
    public float? InitialOpacity { get; }
    public float? InitialRotation { get; }
    public bool EnsureCenterPoint { get; }
}
