// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class CompositionAnimationTests
{
    [AvaloniaFact]
    public void ParallaxAnimation_CalculatesProportionalOffset()
    {
        Vector3 offset = ParallaxAnimation.CalculateOffset(new Avalonia.Vector(20d, 50d), 0.25d);

        Assert.Equal(new Vector3(5f, 12.5f, 0f), offset);
    }

    [AvaloniaFact]
    public void ParallaxAnimation_AppliesOffsetToAttachedCompositionVisual()
    {
        var target = new Border { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        var canvas = new Canvas { Width = 300d, Height = 300d, Children = { target } };
        var window = new Window { Content = canvas };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        bool applied = ParallaxAnimation.Apply(target, new Avalonia.Vector(20d, 50d), 0.25d);

        Assert.True(applied);
        Assert.Equal(new Vector3(35f, 52.5f, 0f), ElementComposition.GetElementVisual(target)?.Offset);
        window.Close();
    }

    [AvaloniaFact]
    public void ParallaxAnimation_RetainsAttachedCompositionVisualAcrossUpdates()
    {
        var target = new Border { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        var canvas = new Canvas { Width = 300d, Height = 300d, Children = { target } };
        var window = new Window { Content = canvas };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        ParallaxAnimation? animation = ParallaxAnimation.TryCreate(target);

        Assert.NotNull(animation);
        animation.Apply(new Avalonia.Vector(10d, 20d), 0.5d);
        animation.Apply(new Avalonia.Vector(20d, 50d), 0.25d);

        Assert.Equal(new Vector3(35f, 52.5f, 0f), ElementComposition.GetElementVisual(target)?.Offset);
        window.Close();
    }

    [AvaloniaFact]
    public void OrbitAnimation_CalculatesNormalizedOrientation()
    {
        Quaternion orientation = OrbitAnimation.CalculateOrientation(
            Quaternion.Identity,
            new Avalonia.Vector(10d, 5d),
            0.5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
        Assert.InRange(orientation.Length(), 0.9999f, 1.0001f);
    }

    [AvaloniaFact]
    public void TiltAnimation_ReturnsIdentityAtCenterAndForEmptySize()
    {
        Quaternion center = TiltAnimation.CalculateOrientation(new Size(100d, 80d), new Point(50d, 40d), 5d);
        Quaternion empty = TiltAnimation.CalculateOrientation(default, new Point(10d, 10d), 5d);

        Assert.Equal(Quaternion.Identity, center);
        Assert.Equal(Quaternion.Identity, empty);
    }

    [AvaloniaFact]
    public void TiltAnimation_CalculatesOrientationAwayFromCenter()
    {
        Quaternion orientation = TiltAnimation.CalculateOrientation(
            new Size(100d, 100d),
            new Point(100d, 50d),
            5d);

        Assert.NotEqual(Quaternion.Identity, orientation);
    }

    [AvaloniaFact]
    public void TiltAnimation_DoesNotCalculateOrientationAtCenterOrForEmptySize()
    {
        bool center = TiltAnimation.TryCalculateOrientation(
            new Size(100d, 80d),
            new Point(50d, 40d),
            5d,
            out Quaternion centerOrientation);
        bool empty = TiltAnimation.TryCalculateOrientation(
            default,
            new Point(10d, 10d),
            5d,
            out Quaternion emptyOrientation);

        Assert.False(center);
        Assert.False(empty);
        Assert.Equal(Quaternion.Identity, centerOrientation);
        Assert.Equal(Quaternion.Identity, emptyOrientation);
    }

    [AvaloniaFact]
    public void CompositionEffects_ReturnFalseWithoutTargetVisuals()
    {
        var orbit = new OrbitAnimation();

        Assert.Null(ParallaxAnimation.TryCreate(null));
        Assert.False(ParallaxAnimation.Apply(null, default, 0.25d));
        Assert.False(orbit.Rotate(null, new Avalonia.Vector(10d, 5d), 0.5d));
        Assert.False(TiltAnimation.Apply(null, default, 5d));
        Assert.False(TiltAnimation.Reset(null));
        Assert.Equal(Quaternion.Identity, orbit.Orientation);
    }

    [AvaloniaFact]
    public async Task AttentionAnimation_StartsFromPositionedControlLayoutOffset()
    {
        await AssertCompositionOffsetAsync(
            target => AttentionAnimations.SetBounce(target, 10_000d),
            new Vector3(30f, 40f, 0f));
    }

    [AvaloniaFact]
    public async Task EntranceAnimation_StartsFromLayoutRelativeOffset()
    {
        await AssertCompositionOffsetAsync(
            target => EntranceAnimations.SetSlideInLeft(target, 10_000d),
            new Vector3(-210f, 40f, 0f));
    }

    [AvaloniaFact]
    public async Task ExitAnimation_StartsFromPositionedControlLayoutOffset()
    {
        await AssertCompositionOffsetAsync(
            target => ExitAnimations.SetSlideOutDown(target, 10_000d),
            new Vector3(30f, 40f, 0f));
    }

    [AvaloniaFact]
    public async Task FramerMotionAnimation_StartsFromLayoutRelativeOffset()
    {
        await AssertCompositionOffsetAsync(
            target => FramerMotionAnimations.SetSlideInFromLeft(target, 10_000d),
            new Vector3(-90f, 40f, 0f));
    }

    [AvaloniaFact]
    public async Task SlidingAnimation_DoesNotOverwritePositionedControlLayoutOffset()
    {
        await AssertCompositionOffsetAsync(
            target => SlidingAnimation.SetLeft(target, 10_000d),
            new Vector3(30f, 40f, 0f));
    }

    [AvaloniaFact]
    public void CompositionAnimationHelpers_AddRelativeMotionToLayoutOffset()
    {
        var target = new Border();
        target.Arrange(new Rect(30d, 40d, 100d, 100d));

        Vector3 offset = CompositionAnimationHelpers.GetLayoutOffset(
            target,
            new Vector3(-100f, 25f, 0f));

        Assert.Equal(new Vector3(-70f, 65f, 0f), offset);
    }

    private static async Task AssertCompositionOffsetAsync(Action<Control> configure, Vector3 expectedOffset)
    {
        var target = new Border { Width = 100d, Height = 100d };
        Canvas.SetLeft(target, 30d);
        Canvas.SetTop(target, 40d);
        var canvas = new Canvas { Width = 300d, Height = 300d, Children = { target } };
        var window = new Window { Content = canvas };
        configure(target);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            await Task.Delay(20);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(expectedOffset, ElementComposition.GetElementVisual(target)?.Offset);
        }
        finally
        {
            window.Close();
        }
    }
}
