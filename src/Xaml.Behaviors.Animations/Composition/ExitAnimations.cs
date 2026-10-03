// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition;
#else
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Provides exit animation helpers inspired by animate.css.
/// </summary>
public static class ExitAnimations
{
    /// <summary>
    /// Animates <paramref name="element"/> with the BackOutDown exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBackOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutDown, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the BackOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBackOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutLeft, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the BackOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBackOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutRight, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the BackOutUp exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBackOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutUp, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the BounceOut exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBounceOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOut, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the BounceOutDown exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBounceOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutDown);

    /// <summary>
    /// Animates <paramref name="element"/> with the BounceOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBounceOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutLeft);

    /// <summary>
    /// Animates <paramref name="element"/> with the BounceOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBounceOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutRight);

    /// <summary>
    /// Animates <paramref name="element"/> with the BounceOutUp exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetBounceOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutUp);

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOut exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFadeOut);

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutDown exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, 24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutDownBig exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutDownBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutLeftBig exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutLeftBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutRightBig exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutRightBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutUp exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, -24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutUpBig exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutUpBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutTopLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutTopLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, -24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutTopRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutTopRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, -24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutBottomLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutBottomLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, 24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FadeOutBottomRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFadeOutBottomRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, 24f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the FlipOutX exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFlipOutX(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipOutX, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the FlipOutY exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetFlipOutY(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipOutY, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the LightSpeedOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetLightSpeedOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedOut(-1));

    /// <summary>
    /// Animates <paramref name="element"/> with the LightSpeedOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetLightSpeedOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedOut(1));

    /// <summary>
    /// Animates <paramref name="element"/> with the RotateOut exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRotateOut(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(90f, new Vector2(0.5f, 0.5f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the RotateOutDownLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRotateOutDownLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(45f, new Vector2(0f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the RotateOutDownRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRotateOutDownRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(-45f, new Vector2(1f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the RotateOutUpLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRotateOutUpLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(-45f, new Vector2(0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the RotateOutUpRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRotateOutUpRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(45f, new Vector2(1f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the SlideOutDown exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetSlideOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the SlideOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetSlideOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the SlideOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetSlideOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the SlideOutUp exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetSlideOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Animates <paramref name="element"/> with the ZoomOut exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetZoomOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateZoomOut, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the ZoomOutDown exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetZoomOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(0f, 240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the ZoomOutLeft exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetZoomOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(-240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the ZoomOutRight exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetZoomOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the ZoomOutUp exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetZoomOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(0f, -240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the Hinge exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetHinge(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateHinge, ensureCenterPoint: true);

    /// <summary>
    /// Animates <paramref name="element"/> with the RollOut exit effect over the specified duration.
    /// </summary>
    /// <param name="element">Control that receives the animation.</param>
    /// <param name="milliseconds">Animation duration in milliseconds.</param>
    public static void SetRollOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateRollOut);

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
                CompositionAnimationHelpers.SetOffset(visual, CompositionAnimationHelpers.GetLayoutOffset(element, definition.InitialOffset.Value));
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
                CompositionAnimationHelpers.StartOffsetAnimation(element, visual, duration, definition.OffsetFrames);
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

    private static CompositionAnimationDefinition CreateBackOutDown() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.7f, 0.7f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.2f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0.0f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, 240f, 0f))
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackOutLeft() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.7f, 0.7f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.2f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0.0f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(-240f, 0f, 0f))
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackOutRight() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.7f, 0.7f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.2f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0.0f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(240f, 0f, 0f))
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBackOutUp() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0.7f, 0.7f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.7f, 0.7f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.2f, 0.7f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0.0f)
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, -240f, 0f))
            },
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBounceOut() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.25f, new Vector3(0.9f, 0.9f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(1.1f, 1.1f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.75f, new Vector3(1.1f, 1.1f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.3f, 0.3f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateBounceOutDown() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0f, -20f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(0f, 10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.45f, new Vector3(0f, 10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, 300f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.45f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            });

    private static CompositionAnimationDefinition CreateBounceOutLeft() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(20f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(-10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.45f, new Vector3(-10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(-300f, 0f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.45f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            });

    private static CompositionAnimationDefinition CreateBounceOutRight() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(-20f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.45f, new Vector3(10f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(300f, 0f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.45f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            });

    private static CompositionAnimationDefinition CreateBounceOutUp() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.2f, new Vector3(0f, 20f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(0f, -10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.45f, new Vector3(0f, -10f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, -300f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.45f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            });

    private static CompositionAnimationDefinition CreateFadeOut() =>
        new(
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateFadeOutOffset(Vector3 targetOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, targetOffset)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateFlipOutX() =>
        new(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.3f, CompositionAnimationHelpers.DegreesToRadians(20f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, CompositionAnimationHelpers.DegreesToRadians(-90f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            ensureCenterPoint: true,
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateFlipOutY() =>
        new(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.3f, CompositionAnimationHelpers.DegreesToRadians(-20f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, CompositionAnimationHelpers.DegreesToRadians(90f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            ensureCenterPoint: true,
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateLightSpeedOut(int direction) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(direction * 200f, 0f, 0f))
            },
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, CompositionAnimationHelpers.DegreesToRadians(direction * 45f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateRotateOut(float endAngle, Vector2 anchor)
    {
        var endRadians = CompositionAnimationHelpers.DegreesToRadians(endAngle);
        return new CompositionAnimationDefinition(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, endRadians)
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            anchor: anchor,
            initialOpacity: 1f,
            ensureCenterPoint: true);
    }

    private static CompositionAnimationDefinition CreateSlideOut(Vector3 targetOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, targetOffset)
            },
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateZoomOut() =>
        new(
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.5f, new Vector3(0.3f, 0.3f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.1f, 0.1f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.5f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            ensureCenterPoint: true,
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateZoomOutDirectional(Vector3 targetOffset) =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, targetOffset)
            },
            scaleFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.One),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.4f, new Vector3(0.4f, 0.4f, 1f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0.1f, 0.1f, 1f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            ensureCenterPoint: true,
            initialOpacity: 1f);

    private static CompositionAnimationDefinition CreateHinge() =>
        new(
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.2f, CompositionAnimationHelpers.DegreesToRadians(80f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.4f, CompositionAnimationHelpers.DegreesToRadians(60f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.6f, CompositionAnimationHelpers.DegreesToRadians(80f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, CompositionAnimationHelpers.DegreesToRadians(60f)),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, CompositionAnimationHelpers.DegreesToRadians(80f))
            },
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(0.8f, new Vector3(0f, 0f, 0f)),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(0f, 300f, 0f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(0.8f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            anchor: new Vector2(0f, 0f),
            initialOpacity: 1f,
            ensureCenterPoint: true);

    private static CompositionAnimationDefinition CreateRollOut() =>
        new(
            offsetFrames: new[]
            {
                new CompositionAnimationHelpers.Vector3KeyFrame(0.0f, Vector3.Zero),
                new CompositionAnimationHelpers.Vector3KeyFrame(1.0f, new Vector3(240f, 0f, 0f))
            },
            rotationFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 0f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, CompositionAnimationHelpers.DegreesToRadians(120f))
            },
            opacityFrames: new[]
            {
                new CompositionAnimationHelpers.ScalarKeyFrame(0.0f, 1f),
                new CompositionAnimationHelpers.ScalarKeyFrame(1.0f, 0f)
            },
            initialOpacity: 1f);

#if UNO
    // WinUI XAML takes the type of an attached property from its getter. These attached properties are write-only (the
    // value starts the animation), so their getters return NaN.
    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBackOutDown"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBackOutDown(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBackOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBackOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBackOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBackOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBackOutUp"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBackOutUp(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBounceOut"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBounceOut(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBounceOutDown"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBounceOutDown(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBounceOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBounceOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBounceOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBounceOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetBounceOutUp"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetBounceOutUp(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOut"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOut(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutDown"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutDown(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutDownBig"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutDownBig(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutLeftBig"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutLeftBig(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutRightBig"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutRightBig(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutUp"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutUp(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutUpBig"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutUpBig(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutTopLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutTopLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutTopRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutTopRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutBottomLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutBottomLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFadeOutBottomRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFadeOutBottomRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFlipOutX"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFlipOutX(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetFlipOutY"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetFlipOutY(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetLightSpeedOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetLightSpeedOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetLightSpeedOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetLightSpeedOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRotateOut"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRotateOut(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRotateOutDownLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRotateOutDownLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRotateOutDownRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRotateOutDownRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRotateOutUpLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRotateOutUpLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRotateOutUpRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRotateOutUpRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetSlideOutDown"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetSlideOutDown(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetSlideOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetSlideOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetSlideOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetSlideOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetSlideOutUp"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetSlideOutUp(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetZoomOut"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetZoomOut(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetZoomOutDown"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetZoomOutDown(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetZoomOutLeft"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetZoomOutLeft(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetZoomOutRight"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetZoomOutRight(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetZoomOutUp"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetZoomOutUp(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetHinge"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetHinge(Control element) => double.NaN;

    /// <summary>
    /// The getter WinUI XAML requires for the write-only <see cref="SetRollOut"/> attached property; always <see cref="double.NaN"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><see cref="double.NaN"/>.</returns>
    public static double GetRollOut(Control element) => double.NaN;
#endif
}
