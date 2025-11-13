// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides entrance animation helpers inspired by common CSS transitions (e.g. animate.css, Framer Motion).
/// </summary>
public static class EntranceAnimations
{
    /// <summary>
    /// Applies the animate.css backInDown animation.
    /// </summary>
    public static void SetBackInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackInDown);

    /// <summary>
    /// Applies the animate.css backInLeft animation.
    /// </summary>
    public static void SetBackInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackInLeft);

    /// <summary>
    /// Applies the animate.css backInRight animation.
    /// </summary>
    public static void SetBackInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackInRight);

    /// <summary>
    /// Applies the animate.css backInUp animation.
    /// </summary>
    public static void SetBackInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackInUp);

    /// <summary>
    /// Applies the animate.css bounceIn animation.
    /// </summary>
    public static void SetBounceIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceIn, ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css bounceInDown animation.
    /// </summary>
    public static void SetBounceInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceInDown);

    /// <summary>
    /// Applies the animate.css bounceInLeft animation.
    /// </summary>
    public static void SetBounceInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceInLeft);

    /// <summary>
    /// Applies the animate.css bounceInRight animation.
    /// </summary>
    public static void SetBounceInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceInRight);

    /// <summary>
    /// Applies the animate.css bounceInUp animation.
    /// </summary>
    public static void SetBounceInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceInUp);

    /// <summary>
    /// Applies a simple fade-in animation.
    /// </summary>
    public static void SetFadeIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFadeIn);

    /// <summary>
    /// Applies the animate.css fadeInDown animation.
    /// </summary>
    public static void SetFadeInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(0f, -24f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInDownBig animation.
    /// </summary>
    public static void SetFadeInDownBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInLeft animation.
    /// </summary>
    public static void SetFadeInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(-24f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInLeftBig animation.
    /// </summary>
    public static void SetFadeInLeftBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInRight animation.
    /// </summary>
    public static void SetFadeInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(24f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInRightBig animation.
    /// </summary>
    public static void SetFadeInRightBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInUp animation.
    /// </summary>
    public static void SetFadeInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(0f, 24f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInUpBig animation.
    /// </summary>
    public static void SetFadeInUpBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInTopLeft animation.
    /// </summary>
    public static void SetFadeInTopLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(-24f, -24f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInTopRight animation.
    /// </summary>
    public static void SetFadeInTopRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(24f, -24f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInBottomLeft animation.
    /// </summary>
    public static void SetFadeInBottomLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(-24f, 24f, 0f)));

    /// <summary>
    /// Applies the animate.css fadeInBottomRight animation.
    /// </summary>
    public static void SetFadeInBottomRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeInOffset(new Vector3(24f, 24f, 0f)));

    /// <summary>
    /// Applies the animate.css zoomIn animation. (Compatibility alias for fade-in zoom variant.)
    /// </summary>
    public static void SetFadeInZoom(Control element, double milliseconds) =>
        SetZoomIn(element, milliseconds);

    /// <summary>
    /// Applies the animate.css flipInX animation.
    /// </summary>
    public static void SetFlipInX(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipInX, ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css flipInY animation.
    /// </summary>
    public static void SetFlipInY(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipInY, ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css lightSpeedInLeft animation.
    /// </summary>
    public static void SetLightSpeedInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedIn(-1));

    /// <summary>
    /// Applies the animate.css lightSpeedInRight animation.
    /// </summary>
    public static void SetLightSpeedInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedIn(1));

    /// <summary>
    /// Applies the animate.css rotateIn animation.
    /// </summary>
    public static void SetRotateIn(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateIn( -90f, new Vector2(0.5f, 0.5f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css rotateInDownLeft animation.
    /// </summary>
    public static void SetRotateInDownLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateIn(-45f, new Vector2(0f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css rotateInDownRight animation.
    /// </summary>
    public static void SetRotateInDownRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateIn(45f, new Vector2(1f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css rotateInUpLeft animation.
    /// </summary>
    public static void SetRotateInUpLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateIn(-45f, new Vector2(0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css rotateInUpRight animation.
    /// </summary>
    public static void SetRotateInUpRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateIn(45f, new Vector2(1f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css slideInDown animation.
    /// </summary>
    public static void SetSlideInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Applies the animate.css slideInLeft animation.
    /// </summary>
    public static void SetSlideInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css slideInRight animation.
    /// </summary>
    public static void SetSlideInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Applies the animate.css slideInUp animation.
    /// </summary>
    public static void SetSlideInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideIn(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Applies the animate.css zoomIn animation.
    /// </summary>
    public static void SetZoomIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateZoomIn, ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css zoomInDown animation.
    /// </summary>
    public static void SetZoomInDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomInDirectional(new Vector3(0f, -240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css zoomInLeft animation.
    /// </summary>
    public static void SetZoomInLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomInDirectional(new Vector3(-240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css zoomInRight animation.
    /// </summary>
    public static void SetZoomInRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomInDirectional(new Vector3(240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css zoomInUp animation.
    /// </summary>
    public static void SetZoomInUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomInDirectional(new Vector3(0f, 240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css jackInTheBox animation.
    /// </summary>
    public static void SetJackInTheBox(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateJackInTheBox, ensureCenterPoint: true);

    /// <summary>
    /// Applies the animate.css rollIn animation.
    /// </summary>
    public static void SetRollIn(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateRollIn);

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

    private static CompositionAnimationDefinition CreateBackInDown() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0f, -240f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: new Vector3(0f, -240f, 0f),
            initialScale: new Vector3(0.7f, 0.7f, 1f),
            initialOpacity: 0.7f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackInLeft() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(-240f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: new Vector3(-240f, 0f, 0f),
            initialScale: new Vector3(0.7f, 0.7f, 1f),
            initialOpacity: 0.7f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackInRight() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(240f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: new Vector3(240f, 0f, 0f),
            initialScale: new Vector3(0.7f, 0.7f, 1f),
            initialOpacity: 0.7f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackInUp() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0f, 240f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: new Vector3(0f, 240f, 0f),
            initialScale: new Vector3(0.7f, 0.7f, 1f),
            initialOpacity: 0.7f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBounceIn() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.3f, 0.3f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(1.1f, 1.1f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(0.9f, 0.9f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(1.03f, 1.03f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0.97f, 0.97f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialScale: new Vector3(0.3f, 0.3f, 1f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBounceInDown() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0f, -300f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(0f, 25f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.75f, new Vector3(0f, -10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.9f, new Vector3(0f, 5f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(0f, -300f, 0f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateBounceInLeft() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(-300f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(25f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.75f, new Vector3(-10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.9f, new Vector3(5f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(-300f, 0f, 0f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateBounceInRight() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(300f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(-25f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.75f, new Vector3(10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.9f, new Vector3(-5f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(300f, 0f, 0f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateBounceInUp() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0f, 300f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(0f, -25f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.75f, new Vector3(0f, 10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.9f, new Vector3(0f, -5f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(0f, 300f, 0f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateFadeIn() =>
        new(
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateFadeInOffset(Vector3 startOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, startOffset),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: startOffset,
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateFlipInX() =>
        new(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(90f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, CompositionAnimationHelpers.DegreesToRadians(-20f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, CompositionAnimationHelpers.DegreesToRadians(10f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, CompositionAnimationHelpers.DegreesToRadians(-5f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialRotation: CompositionAnimationHelpers.DegreesToRadians(90f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateFlipInY() =>
        new(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(90f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, CompositionAnimationHelpers.DegreesToRadians(-20f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, CompositionAnimationHelpers.DegreesToRadians(10f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, CompositionAnimationHelpers.DegreesToRadians(-5f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialRotation: CompositionAnimationHelpers.DegreesToRadians(90f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateLightSpeedIn(int direction) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(direction * -200f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(direction * 30f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(direction * -30f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(direction * -200f, 0f, 0f),
            initialRotation: CompositionAnimationHelpers.DegreesToRadians(direction * -30f),
            initialOpacity: 0f);

    private static CompositionAnimationDefinition CreateRotateIn(float startAngle, Vector2 anchor)
    {
        var startRadians = CompositionAnimationHelpers.DegreesToRadians(startAngle);
        return new CompositionAnimationDefinition(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, startRadians),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            anchor: anchor,
            initialRotation: startRadians,
            initialOpacity: 0f,
            ensureCenterPoint: true);
    }

    private static CompositionAnimationDefinition CreateSlideIn(Vector3 startOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, startOffset),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            initialOffset: startOffset);

    private static CompositionAnimationDefinition CreateZoomIn() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.3f, 0.3f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(1.05f, 1.05f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialScale: new Vector3(0.3f, 0.3f, 1f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateZoomInDirectional(Vector3 startOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, startOffset),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, startOffset * 0.2f),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.3f, 0.3f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.6f, new Vector3(1.05f, 1.05f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: startOffset,
            initialScale: new Vector3(0.3f, 0.3f, 1f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateJackInTheBox() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(0.1f, 0.1f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(0.8f, 0.8f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.One)
            },
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(30f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, CompositionAnimationHelpers.DegreesToRadians(-10f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.7f, CompositionAnimationHelpers.DegreesToRadians(3f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.7f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            anchor: new Vector2(0.5f, 1f),
            initialScale: new Vector3(0.1f, 0.1f, 1f),
            initialRotation: CompositionAnimationHelpers.DegreesToRadians(30f),
            initialOpacity: 0f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateRollIn() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, new Vector3(-240f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, Vector3.Zero)
            },
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, CompositionAnimationHelpers.DegreesToRadians(-120f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 1f)
            },
            initialOffset: new Vector3(-240f, 0f, 0f),
            initialRotation: CompositionAnimationHelpers.DegreesToRadians(-120f),
            initialOpacity: 0f);
}
