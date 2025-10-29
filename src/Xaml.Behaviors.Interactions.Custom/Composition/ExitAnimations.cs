// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides exit animation helpers inspired by animate.css.
/// </summary>
public static class ExitAnimations
{
    /// <summary>
    /// Applies a "back out down" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetBackOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutDown, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "back out left" animation to the specified control over the given duration.
    /// </summary>
    /// <param name="element">The control to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetBackOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutLeft, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Back Out Right" animation to the specified UI element over the given duration.   
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetBackOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutRight, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "back out up" animation to the specified UI element over the given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetBackOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBackOutUp, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "bounce out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetBounceOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOut, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "bounce out down" animation effect to the specified control over a given duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds. Must be a positive value.</param>
    public static void SetBounceOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutDown);

    /// <summary>
    /// Applies a "bounce out left" animation to the specified control over the given duration.
    /// </summary>
    /// <param name="element">The control to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetBounceOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutLeft);

    /// <summary>
    /// Applies a "bounce out right" animation effect to the specified control over the given duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the animation effect will be applied.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds. Must be a positive value.</param>
    public static void SetBounceOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutRight);

    /// <summary>
    /// Applies a "bounce out up" animation effect to the specified control over a given duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds. Must be a positive value.</param>
    public static void SetBounceOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateBounceOutUp);

    /// <summary>
    /// Applies a fade-out animation to the specified UI element over the given duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the fade-out animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFadeOut);

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it downward over the specified duration.
    /// </summary>
    /// <param name="element">The control to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, 24f, 0f)));

    /// <summary>
    /// Applies a "fade out and move down" animation effect to the specified UI element over the given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation effect will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutDownBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified <see cref="Control"/> element, moving it to the left over the
    /// specified duration.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, 0f, 0f)));

    /// <summary>
    /// Applies a "fade out to the left" animation effect to the specified control.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutLeftBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified UI element, moving it to the right over the specified duration.
    /// </summary>
    /// <param name="element">The UI element to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the fade-out animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFadeOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, 0f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it to the right while fading out.
    /// </summary>
    /// <param name="element">The control to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutRightBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it upward as it fades.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, -24f, 0f)));

    /// <summary>
    /// Applies a "fade out and move up" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation effect will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutUpBig(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it upward and left as it fades.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutTopLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, -24f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it upward and right as it fades.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutTopRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, -24f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it downward and left as it fades.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutBottomLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(-24f, 24f, 0f)));

    /// <summary>
    /// Applies a fade-out animation to the specified control, moving it downward and right as it fades.
    /// </summary>
    /// <param name="element">The <see cref="Control"/> to which the fade-out animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a positive value.</param>
    public static void SetFadeOutBottomRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateFadeOutOffset(new Vector3(24f, 24f, 0f)));

    /// <summary>
    /// Applies a "flip out X" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFlipOutX(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipOutX, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "flip out Y" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetFlipOutY(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateFlipOutY, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "light speed out left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetLightSpeedOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedOut(-1));

    /// <summary>
    /// Applies a "light speed out right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetLightSpeedOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateLightSpeedOut(1));

    /// <summary>
    /// Applies a "Rotate out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOut(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(90f, new Vector2(0.5f, 0.5f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Rotate Out Down Left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOutDownLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(45f, new Vector2(0f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Rotate Out Down Right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOutDownRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(-45f, new Vector2(1f, 1f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Rotate Out Up Left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOutUpLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(-45f, new Vector2(0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Rotate Out Up Right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetRotateOutUpRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateRotateOut(45f, new Vector2(1f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Slide Out Down" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(0f, 240f, 0f)));

    /// <summary>
    /// Applies a "Slide Out Left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(-240f, 0f, 0f)));

    /// <summary>
    /// Applies a "Slide Out Right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(240f, 0f, 0f)));

    /// <summary>
    /// Applies a "Slide Out Up" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetSlideOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateSlideOut(new Vector3(0f, -240f, 0f)));

    /// <summary>
    /// Applies a "Zoom out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetZoomOut(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateZoomOut, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Zoom out down" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetZoomOutDown(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(0f, 240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Zoom out left" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetZoomOutLeft(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(-240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Zoom out right" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetZoomOutRight(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(240f, 0f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Zoom out up" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetZoomOutUp(Control element, double milliseconds) =>
        Run(element, milliseconds, () => CreateZoomOutDirectional(new Vector3(0f, -240f, 0f)), ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Hinge" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
    public static void SetHinge(Control element, double milliseconds) =>
        Run(element, milliseconds, CreateHinge, ensureCenterPoint: true);

    /// <summary>
    /// Applies a "Roll out" animation effect to the specified UI element over a given duration.
    /// </summary>
    /// <param name="element">The UI element to which the animation will be applied.</param>
    /// <param name="milliseconds">The duration of the animation, in milliseconds. Must be a non-negative value.</param>
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
}
