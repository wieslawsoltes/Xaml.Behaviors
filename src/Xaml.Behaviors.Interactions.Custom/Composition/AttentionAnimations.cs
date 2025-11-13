// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides attention-drawing animation helpers inspired by popular web animation libraries such as animate.css.
/// </summary>
public static class AttentionAnimations
{
    /// <summary>
    /// Applies a bounce animation to the element when loaded.
    /// </summary>
    public static void SetBounce(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyBounce);

    /// <summary>
    /// Applies a flash animation to the element when loaded.
    /// </summary>
    public static void SetFlash(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyFlash);

    /// <summary>
    /// Applies a pulse animation to the element when loaded.
    /// </summary>
    public static void SetPulse(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyPulse, ensureCenterPoint: true);

    /// <summary>
    /// Applies a rubber band animation to the element when loaded.
    /// </summary>
    public static void SetRubberBand(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyRubberBand, ensureCenterPoint: true);

    /// <summary>
    /// Applies a horizontal shake animation to the element when loaded.
    /// </summary>
    public static void SetShakeX(Control element, double milliseconds) =>
        Run(element, milliseconds, (ctrl, visual, duration) => ApplyShake(ctrl, visual, duration, horizontal: true));

    /// <summary>
    /// Applies a vertical shake animation to the element when loaded.
    /// </summary>
    public static void SetShakeY(Control element, double milliseconds) =>
        Run(element, milliseconds, (ctrl, visual, duration) => ApplyShake(ctrl, visual, duration, horizontal: false));

    /// <summary>
    /// Applies a head shake animation to the element when loaded.
    /// </summary>
    public static void SetHeadShake(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyHeadShake, ensureCenterPoint: true);

    /// <summary>
    /// Applies a swing animation to the element when loaded.
    /// </summary>
    public static void SetSwing(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplySwing, anchor: new Vector2(0.5f, 0f));

    /// <summary>
    /// Applies a tada animation (scale and rotation) to the element when loaded.
    /// </summary>
    public static void SetTada(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyTada, ensureCenterPoint: true);

    /// <summary>
    /// Applies a wobble animation to the element when loaded.
    /// </summary>
    public static void SetWobble(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyWobble, ensureCenterPoint: true);

    /// <summary>
    /// Applies a jello animation to the element when loaded.
    /// </summary>
    public static void SetJello(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyJello, ensureCenterPoint: true);

    /// <summary>
    /// Applies a heart beat animation to the element when loaded.
    /// </summary>
    public static void SetHeartBeat(Control element, double milliseconds) =>
        Run(element, milliseconds, ApplyHeartBeat, ensureCenterPoint: true);

    private static void Run(Control element, double milliseconds, Action<Control, CompositionVisual, TimeSpan> animation, bool ensureCenterPoint = false, Vector2? anchor = null)
    {
        element.Loaded += (_, _) =>
        {
            var visual = ElementComposition.GetElementVisual(element);
            if (visual is null)
            {
                return;
            }

            if (anchor.HasValue)
            {
                CompositionAnimationHelpers.SetNormalizedCenterPoint(element, visual, anchor.Value);
            }
            else if (ensureCenterPoint)
            {
                CompositionAnimationHelpers.EnsureCenterPoint(element, visual);
            }

            animation(element, visual, TimeSpan.FromMilliseconds(milliseconds));
        };
    }

    private static void ApplyBounce(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Offset",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.Zero),
                new(0.2f, Vector3.Zero),
                new(0.4f, new Vector3(0f, -24f, 0f)),
                new(0.43f, new Vector3(0f, -24f, 0f)),
                new(0.53f, Vector3.Zero),
                new(0.7f, new Vector3(0f, -12f, 0f)),
                new(0.8f, Vector3.Zero),
                new(0.9f, new Vector3(0f, -4f, 0f)),
                new(1.0f, Vector3.Zero)
            });
    }

    private static void ApplyFlash(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartScalarAnimation(
            visual,
            "Opacity",
            duration,
            new CompositionAnimationHelpers.ScalarKeyFrame[]
            {
                new(0.0f, 1.0f),
                new(0.25f, 0.0f),
                new(0.5f, 1.0f),
                new(0.75f, 0.0f),
                new(1.0f, 1.0f)
            });
    }

    private static void ApplyPulse(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Scale",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.One),
                new(0.5f, new Vector3(1.05f, 1.05f, 1f)),
                new(1.0f, Vector3.One)
            });
    }

    private static void ApplyRubberBand(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Scale",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.One),
                new(0.3f, new Vector3(1.25f, 0.75f, 1f)),
                new(0.4f, new Vector3(0.75f, 1.25f, 1f)),
                new(0.5f, new Vector3(1.15f, 0.85f, 1f)),
                new(0.65f, new Vector3(0.95f, 1.05f, 1f)),
                new(0.75f, new Vector3(1.05f, 0.95f, 1f)),
                new(1.0f, Vector3.One)
            });
    }

    private static void ApplyShake(Control element, CompositionVisual visual, TimeSpan duration, bool horizontal)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Offset",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.Zero),
                new(0.1f, horizontal ? new Vector3(-8f, 0f, 0f) : new Vector3(0f, -8f, 0f)),
                new(0.2f, horizontal ? new Vector3(8f, 0f, 0f) : new Vector3(0f, 8f, 0f)),
                new(0.3f, horizontal ? new Vector3(-6f, 0f, 0f) : new Vector3(0f, -6f, 0f)),
                new(0.4f, horizontal ? new Vector3(6f, 0f, 0f) : new Vector3(0f, 6f, 0f)),
                new(0.5f, horizontal ? new Vector3(-4f, 0f, 0f) : new Vector3(0f, -4f, 0f)),
                new(0.6f, horizontal ? new Vector3(4f, 0f, 0f) : new Vector3(0f, 4f, 0f)),
                new(0.7f, horizontal ? new Vector3(-2f, 0f, 0f) : new Vector3(0f, -2f, 0f)),
                new(0.8f, horizontal ? new Vector3(2f, 0f, 0f) : new Vector3(0f, 2f, 0f)),
                new(1.0f, Vector3.Zero)
            });
    }

    private static void ApplyHeadShake(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Offset",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.Zero),
                new(0.065f, new Vector3(-6f, 0f, 0f)),
                new(0.185f, new Vector3(5f, 0f, 0f)),
                new(0.315f, new Vector3(-3f, 0f, 0f)),
                new(0.435f, new Vector3(2f, 0f, 0f)),
                new(0.5f, Vector3.Zero),
                new(1.0f, Vector3.Zero)
            });

        CompositionAnimationHelpers.StartScalarAnimation(
            visual,
            "RotationAngle",
            duration,
            new CompositionAnimationHelpers.ScalarKeyFrame[]
            {
                new(0.0f, 0f),
                new(0.065f, CompositionAnimationHelpers.DegreesToRadians(-6f)),
                new(0.185f, CompositionAnimationHelpers.DegreesToRadians(4f)),
                new(0.315f, CompositionAnimationHelpers.DegreesToRadians(-3f)),
                new(0.435f, CompositionAnimationHelpers.DegreesToRadians(2f)),
                new(0.5f, 0f),
                new(1.0f, 0f)
            });
    }

    private static void ApplySwing(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartScalarAnimation(
            visual,
            "RotationAngle",
            duration,
            new CompositionAnimationHelpers.ScalarKeyFrame[]
            {
                new(0.0f, 0f),
                new(0.2f, CompositionAnimationHelpers.DegreesToRadians(15f)),
                new(0.4f, CompositionAnimationHelpers.DegreesToRadians(-10f)),
                new(0.6f, CompositionAnimationHelpers.DegreesToRadians(5f)),
                new(0.8f, CompositionAnimationHelpers.DegreesToRadians(-5f)),
                new(1.0f, 0f)
            });
    }

    private static void ApplyTada(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Scale",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.One),
                new(0.1f, new Vector3(0.9f, 0.9f, 1f)),
                new(0.2f, new Vector3(0.9f, 0.9f, 1f)),
                new(0.3f, new Vector3(1.1f, 1.1f, 1f)),
                new(0.4f, new Vector3(1.1f, 1.1f, 1f)),
                new(0.5f, new Vector3(0.95f, 0.95f, 1f)),
                new(0.6f, new Vector3(1.05f, 1.05f, 1f)),
                new(0.7f, new Vector3(0.95f, 0.95f, 1f)),
                new(0.8f, new Vector3(1.05f, 1.05f, 1f)),
                new(1.0f, Vector3.One)
            });

        CompositionAnimationHelpers.StartScalarAnimation(
            visual,
            "RotationAngle",
            duration,
            new CompositionAnimationHelpers.ScalarKeyFrame[]
            {
                new(0.0f, 0f),
                new(0.1f, CompositionAnimationHelpers.DegreesToRadians(-12f)),
                new(0.2f, CompositionAnimationHelpers.DegreesToRadians(12f)),
                new(0.3f, CompositionAnimationHelpers.DegreesToRadians(-10f)),
                new(0.4f, CompositionAnimationHelpers.DegreesToRadians(10f)),
                new(0.5f, CompositionAnimationHelpers.DegreesToRadians(-6f)),
                new(0.6f, CompositionAnimationHelpers.DegreesToRadians(6f)),
                new(0.7f, CompositionAnimationHelpers.DegreesToRadians(-3f)),
                new(0.8f, CompositionAnimationHelpers.DegreesToRadians(3f)),
                new(1.0f, 0f)
            });
    }

    private static void ApplyWobble(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Offset",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.Zero),
                new(0.15f, new Vector3(-20f, 0f, 0f)),
                new(0.3f, new Vector3(16f, 0f, 0f)),
                new(0.45f, new Vector3(-12f, 0f, 0f)),
                new(0.6f, new Vector3(8f, 0f, 0f)),
                new(0.75f, new Vector3(-4f, 0f, 0f)),
                new(1.0f, Vector3.Zero)
            });

        CompositionAnimationHelpers.StartScalarAnimation(
            visual,
            "RotationAngle",
            duration,
            new CompositionAnimationHelpers.ScalarKeyFrame[]
            {
                new(0.0f, 0f),
                new(0.15f, CompositionAnimationHelpers.DegreesToRadians(-5f)),
                new(0.3f, CompositionAnimationHelpers.DegreesToRadians(3f)),
                new(0.45f, CompositionAnimationHelpers.DegreesToRadians(-3f)),
                new(0.6f, CompositionAnimationHelpers.DegreesToRadians(2f)),
                new(0.75f, CompositionAnimationHelpers.DegreesToRadians(-1f)),
                new(1.0f, 0f)
            });
    }

    private static void ApplyJello(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Scale",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.One),
                new(0.22f, new Vector3(1.02f, 0.98f, 1f)),
                new(0.33f, new Vector3(0.98f, 1.02f, 1f)),
                new(0.44f, new Vector3(1.015f, 0.985f, 1f)),
                new(0.55f, new Vector3(0.99f, 1.01f, 1f)),
                new(0.66f, new Vector3(1.01f, 0.99f, 1f)),
                new(0.77f, new Vector3(0.995f, 1.005f, 1f)),
                new(1.0f, Vector3.One)
            });
    }

    private static void ApplyHeartBeat(Control element, CompositionVisual visual, TimeSpan duration)
    {
        CompositionAnimationHelpers.StartVector3Animation(
            visual,
            "Scale",
            duration,
            new CompositionAnimationHelpers.Vector3KeyFrame[]
            {
                new(0.0f, Vector3.One),
                new(0.14f, Vector3.One),
                new(0.28f, new Vector3(1.3f, 1.3f, 1f)),
                new(0.42f, Vector3.One),
                new(0.7f, new Vector3(1.3f, 1.3f, 1f)),
                new(1.0f, Vector3.One)
            });
    }
}
