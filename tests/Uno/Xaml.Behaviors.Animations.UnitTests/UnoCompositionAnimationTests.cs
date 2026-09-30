// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class UnoCompositionAnimationTests
{
    private const float MovementTolerance = 3f;

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void ParallaxAnimation_CalculatesProportionalOffset()
    {
        Vector3 offset = ParallaxAnimation.CalculateOffset(new Point(20d, 50d), 0.25d);

        Assert.Equal(new Vector3(5f, 12.5f, 0f), offset);
    }

    [UnoHeadlessFact]
    public async Task ParallaxAnimation_AppliesOffsetRelativeToTheLayoutPosition()
    {
        Border target = await ShowPositionedAsync();

        bool applied = ParallaxAnimation.Apply(target, new Point(20d, 50d), 0.25d);

        Assert.True(applied);
        // WinUI composes Visual.Offset on top of the arranged position (30, 40).
        Assert.Equal(new Vector3(5f, 12.5f, 0f), TestHelpers.GetVisual(target).Offset);
        Assert.Equal(new Vector3(30f, 40f, 0f), target.ActualOffset);
    }

    [UnoHeadlessFact]
    public async Task ParallaxAnimation_RetainsCompositionVisualAcrossUpdates()
    {
        Border target = await ShowPositionedAsync();
        ParallaxAnimation? animation = ParallaxAnimation.TryCreate(target);

        Assert.NotNull(animation);
        animation.Apply(new Point(10d, 20d), 0.5d);
        animation.Apply(new Point(20d, 50d), 0.25d);

        Assert.Equal(new Vector3(5f, 12.5f, 0f), TestHelpers.GetVisual(target).Offset);
    }

    [UnoHeadlessFact]
    public void OrbitAnimation_CalculatesNormalizedOrientation()
    {
        Quaternion orientation = OrbitAnimation.CalculateOrientation(Quaternion.Identity, new Point(10d, 5d), 0.5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
        Assert.InRange(orientation.Length(), 0.9999f, 1.0001f);
    }

    [UnoHeadlessFact]
    public async Task OrbitAnimation_RotatesAroundTheOrientationAxis()
    {
        Border target = new() { Width = 100d, Height = 80d };
        await Session.ShowAsync(target);
        OrbitAnimation orbit = new();

        Assert.True(OrbitAnimation.UpdateCenterPoint(target));
        Assert.True(orbit.Rotate(target, new Point(10d, 5d), 0.5d));

        CompositionVisual visual = TestHelpers.GetVisual(target);
        Assert.Equal(new Vector3(50f, 40f, 0f), visual.CenterPoint);
        Assert.True(CompositionAnimationHelpers.TryGetAxisAngle(orbit.Orientation, out Vector3 axis, out float angle));
        TestHelpers.AssertNear(axis, visual.RotationAxis);
        await TestHelpers.WaitUntilAsync(() => MathF.Abs(visual.RotationAngle - angle) < 0.0001f, "the orbit rotation is applied");

        orbit.Reset();
        Assert.Equal(Quaternion.Identity, orbit.Orientation);
    }

    [UnoHeadlessFact]
    public void TiltAnimation_ReturnsIdentityAtCenterAndForEmptySize()
    {
        Quaternion center = TiltAnimation.CalculateOrientation(new Size(100d, 80d), new Point(50d, 40d), 5d);
        Quaternion empty = TiltAnimation.CalculateOrientation(default, new Point(10d, 10d), 5d);

        Assert.Equal(Quaternion.Identity, center);
        Assert.Equal(Quaternion.Identity, empty);
    }

    [UnoHeadlessFact]
    public void TiltAnimation_CalculatesOrientationAwayFromCenter()
    {
        Quaternion orientation = TiltAnimation.CalculateOrientation(new Size(100d, 100d), new Point(100d, 50d), 5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
    }

    [UnoHeadlessFact]
    public void TiltAnimation_DoesNotCalculateOrientationAtCenterOrForEmptySize()
    {
        bool center = TiltAnimation.TryCalculateOrientation(new Size(100d, 80d), new Point(50d, 40d), 5d, out Quaternion centerOrientation);
        bool empty = TiltAnimation.TryCalculateOrientation(default, new Point(10d, 10d), 5d, out Quaternion emptyOrientation);

        Assert.False(center);
        Assert.False(empty);
        Assert.Equal(Quaternion.Identity, centerOrientation);
        Assert.Equal(Quaternion.Identity, emptyOrientation);
    }

    [UnoHeadlessFact]
    public async Task TiltAnimation_TiltsAndAnimatesBackToNeutral()
    {
        Border target = new() { Width = 100d, Height = 80d };
        await Session.ShowAsync(target);
        CompositionVisual visual = TestHelpers.GetVisual(target);

        Assert.True(TiltAnimation.UpdateCenterPoint(target));
        Assert.True(TiltAnimation.Apply(target, new Point(100d, 40d), 5d));
        // Pointer on the right edge: tilt around the vertical axis by the full strength.
        TestHelpers.AssertNear(Vector3.UnitY, visual.RotationAxis);
        await TestHelpers.WaitUntilAsync(
            () => MathF.Abs(visual.RotationAngle - CompositionAnimationHelpers.DegreesToRadians(5f)) < 0.0001f,
            "the tilt is applied");

        Assert.True(TiltAnimation.Reset(target));
        await TestHelpers.WaitUntilAsync(() => visual.RotationAngle == 0f, "the tilt is reset");
    }

    [UnoHeadlessFact]
    public void CompositionEffects_ReturnFalseWithoutTargets()
    {
        OrbitAnimation orbit = new();

        Assert.Null(ParallaxAnimation.TryCreate(null));
        Assert.False(ParallaxAnimation.Apply(null, default, 0.25d));
        Assert.False(orbit.Rotate(null, new Point(10d, 5d), 0.5d));
        Assert.False(TiltAnimation.Apply(null, default, 5d));
        Assert.False(TiltAnimation.Apply(new Border(), default, 5d));
        Assert.False(TiltAnimation.Reset(null));
        Assert.False(TiltAnimation.UpdateCenterPoint(null));
        Assert.False(OrbitAnimation.UpdateCenterPoint(null));
        Assert.Equal(Quaternion.Identity, orbit.Orientation);
    }

    [UnoHeadlessFact]
    public void CompositionAnimationHelpers_ConvertsOrientationsToAxisAngle()
    {
        Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 0.5f);

        Assert.True(CompositionAnimationHelpers.TryGetAxisAngle(orientation, out Vector3 axis, out float angle));
        TestHelpers.AssertNear(Vector3.UnitX, axis);
        Assert.Equal(0.5f, angle, 4);

        Assert.True(CompositionAnimationHelpers.TryGetAxisAngle(Quaternion.Negate(orientation), out axis, out angle));
        TestHelpers.AssertNear(Vector3.UnitX, axis);
        Assert.Equal(0.5f, angle, 4);

        Assert.False(CompositionAnimationHelpers.TryGetAxisAngle(Quaternion.Identity, out _, out angle));
        Assert.Equal(0f, angle);
    }

    [UnoHeadlessFact]
    public async Task AttentionAnimation_StartsAtTheLayoutPosition()
    {
        await AssertCompositionOffsetAsync(target => AttentionAnimations.SetBounce(target, 10_000d), Vector3.Zero);
    }

    [UnoHeadlessFact]
    public async Task EntranceAnimation_StartsFromLayoutRelativeOffset()
    {
        await AssertCompositionOffsetAsync(target => EntranceAnimations.SetSlideInLeft(target, 10_000d), new Vector3(-240f, 0f, 0f));
    }

    [UnoHeadlessFact]
    public async Task ExitAnimation_StartsAtTheLayoutPosition()
    {
        await AssertCompositionOffsetAsync(target => ExitAnimations.SetSlideOutDown(target, 10_000d), Vector3.Zero);
    }

    [UnoHeadlessFact]
    public async Task FramerMotionAnimation_StartsFromLayoutRelativeOffset()
    {
        await AssertCompositionOffsetAsync(target => FramerMotionAnimations.SetSlideInFromLeft(target, 10_000d), new Vector3(-120f, 0f, 0f));
    }

    [UnoHeadlessFact]
    public async Task SlidingAnimation_StartsFromTheElementWidth()
    {
        await AssertCompositionOffsetAsync(target => SlidingAnimation.SetLeft(target, 10_000d), new Vector3(-100f, 0f, 0f));
    }

    [UnoHeadlessFact]
    public async Task CompositionAnimationHelpers_OffsetsAreRelativeToTheLayoutSlot()
    {
        Border target = await ShowPositionedAsync();

        Vector3 offset = CompositionAnimationHelpers.GetLayoutOffset(target, new Vector3(-100f, 25f, 0f));

        Assert.Equal(new Vector3(-100f, 25f, 0f), offset);
        Assert.Equal(new Rect(30d, 40d, 100d, 100d), target.Bounds);
    }

    [UnoHeadlessFact]
    public async Task CompositionAnimations_ReachTheirFinalValues()
    {
        Border target = new() { Width = 20d, Height = 20d };
        FadeAnimation.SetCustomFade(target, 0.2d, 0.6d, 60d);
        await Session.ShowAsync(target);
        CompositionVisual visual = TestHelpers.GetVisual(target);

        await TestHelpers.WaitUntilAsync(() => MathF.Abs(visual.Opacity - 0.6f) < 0.0001f, "the fade completed");
    }

    private static async Task<Border> ShowPositionedAsync()
    {
        Border target = new() { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        await Session.ShowAsync(new Canvas { Width = 300d, Height = 300d, Children = { target } });
        await Session.WaitForIdleAsync();
        return target;
    }

    private static async Task AssertCompositionOffsetAsync(Action<FrameworkElement> configure, Vector3 expectedOffset)
    {
        Border target = new() { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        configure(target);

        await Session.ShowAsync(new Canvas { Width = 300d, Height = 300d, Children = { target } });

        TestHelpers.AssertNear(expectedOffset, TestHelpers.GetVisual(target).Offset, MovementTolerance);
        Assert.Equal(new Vector3(30f, 40f, 0f), target.ActualOffset);
    }
}
