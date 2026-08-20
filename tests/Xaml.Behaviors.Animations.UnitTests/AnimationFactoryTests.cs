// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class AnimationFactoryTests
{
    [AvaloniaFact]
    public void CreateFadeIn_CreatesDelayedOpacityAnimation()
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(200);
        TimeSpan duration = TimeSpan.FromMilliseconds(300);

        var animation = AnimationFactory.CreateFadeIn(delay, duration);

        Assert.Equal(TimeSpan.FromMilliseconds(500), animation.Duration);
        Assert.Collection(
            animation.Children,
            keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime),
            keyFrame => Assert.Equal(delay, keyFrame.KeyTime),
            keyFrame => Assert.Equal(delay + duration, keyFrame.KeyTime));
    }

    [AvaloniaTheory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void CreateFadeIn_RejectsNegativeTimes(double delayMilliseconds, double durationMilliseconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AnimationFactory.CreateFadeIn(
            TimeSpan.FromMilliseconds(delayMilliseconds),
            TimeSpan.FromMilliseconds(durationMilliseconds)));
    }

    [AvaloniaFact]
    public void CreateFadeInTimeline_PreservesExplicitKeyTimes()
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(500);
        TimeSpan completion = TimeSpan.FromMilliseconds(650);
        TimeSpan totalDuration = TimeSpan.FromMilliseconds(750);

        Avalonia.Animation.Animation animation = AnimationFactory.CreateFadeInTimeline(
            delay,
            completion,
            totalDuration);

        Assert.Equal(totalDuration, animation.Duration);
        Assert.Collection(
            animation.Children,
            keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime),
            keyFrame => Assert.Equal(delay, keyFrame.KeyTime),
            keyFrame => Assert.Equal(completion, keyFrame.KeyTime));
    }

    [AvaloniaTheory]
    [InlineData(500, 250, 750)]
    [InlineData(500, 800, 750)]
    public void CreateFadeInTimeline_RejectsNonChronologicalKeyTimes(
        double delayMilliseconds,
        double completionMilliseconds,
        double durationMilliseconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AnimationFactory.CreateFadeInTimeline(
            TimeSpan.FromMilliseconds(delayMilliseconds),
            TimeSpan.FromMilliseconds(completionMilliseconds),
            TimeSpan.FromMilliseconds(durationMilliseconds)));
    }

    [AvaloniaFact]
    public void FluidMoveAnimation_CreatesTranslationAnimation()
    {
        TimeSpan duration = TimeSpan.FromMilliseconds(250);

        var animation = FluidMoveAnimation.Create(12d, -8d, duration);

        Assert.Equal(duration, animation.Duration);
        Assert.Collection(
            animation.Children,
            keyFrame =>
            {
                Assert.Equal(new Cue(0d), keyFrame.Cue);
                Assert.Equal(2, keyFrame.Setters.Count);
            },
            keyFrame =>
            {
                Assert.Equal(new Cue(1d), keyFrame.Cue);
                Assert.Equal(2, keyFrame.Setters.Count);
            });
    }

    [AvaloniaFact]
    public void FluidMoveAnimation_RejectsNegativeDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FluidMoveAnimation.Create(0d, 0d, TimeSpan.FromMilliseconds(-1d)));
    }

    [AvaloniaFact]
    public void FluidMoveAnimation_TryRunPreparesControlTransform()
    {
        var target = new Border { RenderTransform = new RotateTransform() };

        bool started = FluidMoveAnimation.TryRun(
            target,
            12d,
            -8d,
            TimeSpan.Zero);

        Assert.True(started);
        Assert.IsType<TranslateTransform>(target.RenderTransform);
        Assert.False(FluidMoveAnimation.TryRun(null, 0d, 0d, TimeSpan.Zero));
    }
}
