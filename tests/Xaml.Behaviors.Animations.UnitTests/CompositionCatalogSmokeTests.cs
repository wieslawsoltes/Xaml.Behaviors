// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class CompositionCatalogSmokeTests
{
    private const double DurationMilliseconds = 10_000d;

    [AvaloniaFact]
    public void AttentionCatalog_StartsEveryAnimation()
    {
        AssertCatalogStarts(
            target => AttentionAnimations.SetBounce(target, DurationMilliseconds),
            target => AttentionAnimations.SetFlash(target, DurationMilliseconds),
            target => AttentionAnimations.SetPulse(target, DurationMilliseconds),
            target => AttentionAnimations.SetRubberBand(target, DurationMilliseconds),
            target => AttentionAnimations.SetShakeX(target, DurationMilliseconds),
            target => AttentionAnimations.SetShakeY(target, DurationMilliseconds),
            target => AttentionAnimations.SetHeadShake(target, DurationMilliseconds),
            target => AttentionAnimations.SetSwing(target, DurationMilliseconds),
            target => AttentionAnimations.SetTada(target, DurationMilliseconds),
            target => AttentionAnimations.SetWobble(target, DurationMilliseconds),
            target => AttentionAnimations.SetJello(target, DurationMilliseconds),
            target => AttentionAnimations.SetHeartBeat(target, DurationMilliseconds));
    }

    [AvaloniaFact]
    public void EntranceCatalog_StartsEveryAnimation()
    {
        AssertCatalogStarts(
            target => EntranceAnimations.SetBackInDown(target, DurationMilliseconds),
            target => EntranceAnimations.SetBackInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetBackInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetBackInUp(target, DurationMilliseconds),
            target => EntranceAnimations.SetBounceIn(target, DurationMilliseconds),
            target => EntranceAnimations.SetBounceInDown(target, DurationMilliseconds),
            target => EntranceAnimations.SetBounceInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetBounceInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetBounceInUp(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeIn(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInDown(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInDownBig(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInLeftBig(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInRightBig(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInUp(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInUpBig(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInTopLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInTopRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInBottomLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInBottomRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetFadeInZoom(target, DurationMilliseconds),
            target => EntranceAnimations.SetFlipInX(target, DurationMilliseconds),
            target => EntranceAnimations.SetFlipInY(target, DurationMilliseconds),
            target => EntranceAnimations.SetLightSpeedInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetLightSpeedInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetRotateIn(target, DurationMilliseconds),
            target => EntranceAnimations.SetRotateInDownLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetRotateInDownRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetRotateInUpLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetRotateInUpRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetSlideInDown(target, DurationMilliseconds),
            target => EntranceAnimations.SetSlideInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetSlideInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetSlideInUp(target, DurationMilliseconds),
            target => EntranceAnimations.SetZoomIn(target, DurationMilliseconds),
            target => EntranceAnimations.SetZoomInDown(target, DurationMilliseconds),
            target => EntranceAnimations.SetZoomInLeft(target, DurationMilliseconds),
            target => EntranceAnimations.SetZoomInRight(target, DurationMilliseconds),
            target => EntranceAnimations.SetZoomInUp(target, DurationMilliseconds),
            target => EntranceAnimations.SetJackInTheBox(target, DurationMilliseconds),
            target => EntranceAnimations.SetRollIn(target, DurationMilliseconds));
    }

    [AvaloniaFact]
    public void ExitCatalog_StartsEveryAnimation()
    {
        AssertCatalogStarts(
            target => ExitAnimations.SetBackOutDown(target, DurationMilliseconds),
            target => ExitAnimations.SetBackOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetBackOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetBackOutUp(target, DurationMilliseconds),
            target => ExitAnimations.SetBounceOut(target, DurationMilliseconds),
            target => ExitAnimations.SetBounceOutDown(target, DurationMilliseconds),
            target => ExitAnimations.SetBounceOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetBounceOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetBounceOutUp(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOut(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutDown(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutDownBig(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutLeftBig(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutRightBig(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutUp(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutUpBig(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutTopLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutTopRight(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutBottomLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetFadeOutBottomRight(target, DurationMilliseconds),
            target => ExitAnimations.SetFlipOutX(target, DurationMilliseconds),
            target => ExitAnimations.SetFlipOutY(target, DurationMilliseconds),
            target => ExitAnimations.SetLightSpeedOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetLightSpeedOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetRotateOut(target, DurationMilliseconds),
            target => ExitAnimations.SetRotateOutDownLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetRotateOutDownRight(target, DurationMilliseconds),
            target => ExitAnimations.SetRotateOutUpLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetRotateOutUpRight(target, DurationMilliseconds),
            target => ExitAnimations.SetSlideOutDown(target, DurationMilliseconds),
            target => ExitAnimations.SetSlideOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetSlideOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetSlideOutUp(target, DurationMilliseconds),
            target => ExitAnimations.SetZoomOut(target, DurationMilliseconds),
            target => ExitAnimations.SetZoomOutDown(target, DurationMilliseconds),
            target => ExitAnimations.SetZoomOutLeft(target, DurationMilliseconds),
            target => ExitAnimations.SetZoomOutRight(target, DurationMilliseconds),
            target => ExitAnimations.SetZoomOutUp(target, DurationMilliseconds),
            target => ExitAnimations.SetHinge(target, DurationMilliseconds),
            target => ExitAnimations.SetRollOut(target, DurationMilliseconds));
    }

    [AvaloniaFact]
    public void FramerMotionCatalog_StartsEveryAnimation()
    {
        AssertCatalogStarts(
            target => FramerMotionAnimations.SetFadeIn(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetFadeInUp(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetFadeInDown(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetFadeInLeft(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetFadeInRight(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSlideInFromLeft(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSlideInFromRight(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSlideInFromTop(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSlideInFromBottom(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetPopIn(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetPopOut(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSpringIn(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetSpringOut(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetRotateIn(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetRotateOut(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetScaleIn(target, DurationMilliseconds),
            target => FramerMotionAnimations.SetScaleOut(target, DurationMilliseconds));
    }

    [AvaloniaFact]
    public void PrimitiveCatalogs_StartEveryAnimation()
    {
        AssertCatalogStarts(
            target => FadeAnimation.SetFadeIn(target, DurationMilliseconds),
            target => FadeAnimation.SetFadeOut(target, DurationMilliseconds),
            target => FadeAnimation.SetCustomFade(target, 0.2d, 0.8d, DurationMilliseconds),
            target => SlidingAnimation.SetLeft(target, DurationMilliseconds),
            target => SlidingAnimation.SetRight(target, DurationMilliseconds),
            target => SlidingAnimation.SetTop(target, DurationMilliseconds),
            target => SlidingAnimation.SetBottom(target, DurationMilliseconds),
            target => ScaleAnimation.SetScaleIn(target, DurationMilliseconds),
            target => ScaleAnimation.SetScaleOut(target, DurationMilliseconds),
            target => ScaleAnimation.SetZoomIn(target, DurationMilliseconds),
            target => ScaleAnimation.SetZoomOut(target, DurationMilliseconds),
            target => ScaleAnimation.SetBounce(target, DurationMilliseconds),
            target => ScaleAnimation.SetCustomScale(target, 0.8d, 0.9d, 1d, 1d, DurationMilliseconds),
            target => RotateAnimation.SetRotateClockwise(target, DurationMilliseconds),
            target => RotateAnimation.SetRotateCounterClockwise(target, DurationMilliseconds),
            target => RotateAnimation.SetRotateIn(target, DurationMilliseconds),
            target => RotateAnimation.SetRotateOut(target, DurationMilliseconds),
            target => RotateAnimation.SetFlip(target, DurationMilliseconds),
            target => RotateAnimation.SetSwing(target, DurationMilliseconds),
            target => RotateAnimation.SetCustomRotate(target, -30d, 30d, DurationMilliseconds),
            target => SpecialAnimations.SetFlip(target, DurationMilliseconds));
    }

    [AvaloniaFact]
    public void ZeroDuration_AppliesFinalCompositionStatesWithoutStartingInvalidAnimations()
    {
        var attention = new Border { Width = 32d, Height = 32d };
        var entrance = new Border { Width = 32d, Height = 32d };
        var exit = new Border { Width = 32d, Height = 32d };
        var fade = new Border { Width = 32d, Height = 32d };
        var scale = new Border { Width = 32d, Height = 32d };
        var rotate = new Border { Width = 32d, Height = 32d };
        var sliding = new Border { Width = 32d, Height = 32d };
        var special = new Border { Width = 32d, Height = 32d };
        var framerMotion = new Border { Width = 32d, Height = 32d };
        var targets = new[] { attention, entrance, exit, fade, scale, rotate, sliding, special, framerMotion };
        var canvas = new Canvas { Width = 300d, Height = 400d };
        foreach (Border target in targets)
        {
            Canvas.SetLeft(target, 30d);
            Canvas.SetTop(target, 40d);
            canvas.Children.Add(target);
        }

        AttentionAnimations.SetBounce(attention, 0d);
        EntranceAnimations.SetSlideInLeft(entrance, 0d);
        ExitAnimations.SetSlideOutDown(exit, 0d);
        FadeAnimation.SetCustomFade(fade, 0.2d, 0.8d, 0d);
        ScaleAnimation.SetScaleOut(scale, 0d);
        RotateAnimation.SetCustomRotate(rotate, -30d, 30d, 0d);
        SlidingAnimation.SetLeft(sliding, 0d);
        SpecialAnimations.SetFlip(special, 0d);
        FramerMotionAnimations.SetSlideInFromLeft(framerMotion, 0d);

        var window = new Window { Content = canvas };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new Vector3(30f, 40f, 0f), GetVisual(attention).Offset);
            Assert.Equal(new Vector3(30f, 40f, 0f), GetVisual(entrance).Offset);
            Assert.Equal(new Vector3(30f, 280f, 0f), GetVisual(exit).Offset);
            Assert.Equal(0.8f, GetVisual(fade).Opacity);
            Assert.Equal(Vector3.Zero, GetVisual(scale).Scale);
            Assert.Equal(CompositionAnimationHelpers.DegreesToRadians(30f), GetVisual(rotate).RotationAngle);
            Assert.Equal(new Vector3(30f, 40f, 0f), GetVisual(sliding).Offset);
            Assert.Equal(Vector3.One, GetVisual(special).Scale);
            Assert.Equal(0f, GetVisual(special).RotationAngle);
            Assert.Equal(new Vector3(30f, 40f, 0f), GetVisual(framerMotion).Offset);
        }
        finally
        {
            window.Close();
        }
    }

    private static void AssertCatalogStarts(params Action<Control>[] configureAnimations)
    {
        var panel = new StackPanel();
        foreach (Action<Control> configure in configureAnimations)
        {
            var target = new Border { Width = 32d, Height = 32d };
            configure(target);
            panel.Children.Add(target);
        }

        var window = new Window { Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.All(
                panel.Children,
                target => Assert.NotNull(ElementComposition.GetElementVisual(target)));
        }
        finally
        {
            window.Close();
        }
    }

    private static CompositionVisual GetVisual(Visual target)
    {
        return Assert.IsAssignableFrom<CompositionVisual>(ElementComposition.GetElementVisual(target));
    }
}
