// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides composition animations inspired by popular Framer Motion presets.
/// </summary>
public static class FramerMotionAnimations
{
    /// <summary>
    /// Applies a "fade in" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFadeInDefinition);

    /// <summary>
    /// Applies a "fade in up" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInDirectional(new Vector3(0f, 40f, 0f)));

    /// <summary>
    /// Applies a "fade in down" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInDirectional(new Vector3(0f, -40f, 0f)));

    /// <summary>
    /// Applies a "fade in left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInDirectional(new Vector3(-40f, 0f, 0f)));

    /// <summary>
    /// Applies a "fade in right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInDirectional(new Vector3(40f, 0f, 0f)));

    /// <summary>
    /// Applies a "slide in from left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideInFromLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(-120f, 0f, 0f)));

    /// <summary>
    /// Applies a "slide in from right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideInFromRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(120f, 0f, 0f)));

    /// <summary>
    /// Applies a "slide in from top" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideInFromTop(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(0f, -120f, 0f)));

    /// <summary>
    /// Applies a "slide in from bottom" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideInFromBottom(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(0f, 120f, 0f)));

    /// <summary>
    /// Applies a "pop in" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetPopIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreatePopInDefinition, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "pop out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetPopOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreatePopOutDefinition, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "spring in" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSpringIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateSpringInDefinition);

    /// <summary>
    /// Applies a "spring out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSpringOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateSpringOutDefinition);

    /// <summary>
    /// Applies a "rotate in" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateIn(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateDefinition(-15f, 0f), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "rotate out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOut(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateDefinition(0f, 15f), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "scale in" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetScaleIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateScaleInDefinition, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "scale out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetScaleOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateScaleOutDefinition, ensureCenterPoint: true);

    private static void Run(Control element, double milliseconds, Func<CompositionAnimationDefinition> definitionFactory, bool ensureCenterPoint = false)
    {
        element.Loaded += (_, _) =>
        {
            var visual = ElementComposition.GetElementVisual(element);
            if (visual is null)
            {
                return;
            }

            var definition = definitionFactory();

            if (definition.InitialOffset.HasValue)
            {
                visual.Offset = definition.InitialOffset.Value;
            }

            if (definition.InitialScale.HasValue)
            {
                visual.Scale = definition.InitialScale.Value;
            }

            if (definition.InitialOpacity.HasValue)
            {
                visual.Opacity = definition.InitialOpacity.Value;
            }

            if (definition.InitialRotation.HasValue)
            {
                visual.RotationAngle = definition.InitialRotation.Value;
            }

            if (definition.Anchor.HasValue)
            {
                CompositionAnimationHelpers.SetNormalizedCenterPoint(element, visual, definition.Anchor.Value);
            }
            else if (definition.EnsureCenterPoint || ensureCenterPoint ||
                     (definition.ScaleFrames?.Length > 0) || (definition.RotationFrames?.Length > 0))
            {
                CompositionAnimationHelpers.EnsureCenterPoint(element, visual);
            }

            var duration = TimeSpan.FromMilliseconds(milliseconds);

            if (definition.OffsetFrames is { Length: > 0 })
            {
                CompositionAnimationHelpers.StartVector3Animation(visual, "Offset", duration, definition.OffsetFrames);
            }

            if (definition.ScaleFrames is { Length: > 0 })
            {
                CompositionAnimationHelpers.StartVector3Animation(visual, "Scale", duration, definition.ScaleFrames);
            }

            if (definition.OpacityFrames is { Length: > 0 })
            {
                CompositionAnimationHelpers.StartScalarAnimation(visual, "Opacity", duration, definition.OpacityFrames);
            }

            if (definition.RotationFrames is { Length: > 0 })
            {
                CompositionAnimationHelpers.StartScalarAnimation(visual, "RotationAngle", duration, definition.RotationFrames);
            }
        };
    }

    private static CompositionAnimationDefinition CreateFadeInDefinition() =>
        new(
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateFadeInDirectional(Vector3 offset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, offset),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.7f, offset * 0.2f),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.7f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: offset,
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateSlideIn(Vector3 offset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, offset),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, offset * -0.1f),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: offset);

    private static CompositionAnimationDefinition CreatePopInDefinition() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.8f, 0.8f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(1.1f, 1.1f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.7f, new Vector3(0.98f, 0.98f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialScale: new Vector3(0.8f, 0.8f, 1f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreatePopOutDefinition() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.3f, new Vector3(1.05f, 1.05f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.6f, 0.6f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.3f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateSpringInDefinition() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0f, 120f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(0f, -12f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 6f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(0f, 120f, 0f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateSpringOutDefinition() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.3f, new Vector3(0f, -12f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(0f, 6f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, 140f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateRotateDefinition(float startAngle, float endAngle)
    {
        var startRadians = CompositionAnimationHelpers.DegreesToRadians(startAngle);
        var midRadians = CompositionAnimationHelpers.DegreesToRadians(endAngle * 0.6f);
        var endRadians = CompositionAnimationHelpers.DegreesToRadians(endAngle);
        var startVisible = Math.Abs(startAngle) < float.Epsilon;
        var endVisible = Math.Abs(endAngle) < float.Epsilon;

        return new CompositionAnimationDefinition(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, startRadians),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, midRadians),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, endRadians)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, startVisible ? 1f : 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, endVisible ? 1f : 0f)
            },
            initialRotation: startRadians,
            initialOpacity: startVisible ? 1f : 0f,
            ensureCenterPoint: true);
    }

    private static CompositionAnimationDefinition CreateScaleInDefinition() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.95f, 0.95f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(1.02f, 1.02f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialScale: new Vector3(0.95f, 0.95f, 1f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateScaleOutDefinition() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(0.95f, 0.95f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.8f, 0.8f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);
}
