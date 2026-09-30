// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Custom;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
#endif
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

#if UNO
        // WinUI storyboard: the duration and key frames belong to its opacity key frame animation.
        DoubleAnimationUsingKeyFrames opacity = GetSingleKeyFrameAnimation(animation);
        Assert.Equal(TimeSpan.FromMilliseconds(500), opacity.Duration.TimeSpan);
        Assert.Collection(
            opacity.KeyFrames,
            keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime.TimeSpan),
            keyFrame => Assert.Equal(delay, keyFrame.KeyTime.TimeSpan),
            keyFrame => Assert.Equal(delay + duration, keyFrame.KeyTime.TimeSpan));
#else
        Assert.Equal(TimeSpan.FromMilliseconds(500), animation.Duration);
        Assert.Collection(
            animation.Children,
            keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime),
            keyFrame => Assert.Equal(delay, keyFrame.KeyTime),
            keyFrame => Assert.Equal(delay + duration, keyFrame.KeyTime));
#endif
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

#if UNO
        Storyboard animation = AnimationFactory.CreateFadeInTimeline(
            delay,
            completion,
            totalDuration);

        // WinUI storyboard: the duration and key frames belong to its opacity key frame animation.
        DoubleAnimationUsingKeyFrames opacity = GetSingleKeyFrameAnimation(animation);
        Assert.Equal(totalDuration, opacity.Duration.TimeSpan);
        Assert.Collection(
            opacity.KeyFrames,
            keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime.TimeSpan),
            keyFrame => Assert.Equal(delay, keyFrame.KeyTime.TimeSpan),
            keyFrame => Assert.Equal(completion, keyFrame.KeyTime.TimeSpan));
#else
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
#endif
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

#if UNO
        // WinUI storyboard: FillBehavior.HoldEnd is Avalonia FillMode.Forward; each translation setter is a key frame
        // animation (X, Y) with the key frames at the cues 0 and 1.
        Assert.Equal(FillBehavior.HoldEnd, animation.FillBehavior);
        Assert.Equal(2, animation.Children.Count);
        Assert.All(
            animation.Children,
            timeline =>
            {
                DoubleAnimationUsingKeyFrames translation = Assert.IsType<DoubleAnimationUsingKeyFrames>(timeline);
                Assert.Equal(duration, translation.Duration.TimeSpan);
                Assert.Collection(
                    translation.KeyFrames,
                    keyFrame => Assert.Equal(TimeSpan.Zero, keyFrame.KeyTime.TimeSpan),
                    keyFrame => Assert.Equal(duration, keyFrame.KeyTime.TimeSpan));
            });
#else
        Assert.Equal(duration, animation.Duration);
        Assert.Equal(FillMode.Forward, animation.FillMode);
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
#endif
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
        TranslateTransform transform = Assert.IsType<TranslateTransform>(target.RenderTransform);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(0d, transform.X);
        Assert.Equal(0d, transform.Y);
        Assert.False(FluidMoveAnimation.TryRun(null, 0d, 0d, TimeSpan.Zero));
    }
#if UNO

    private static DoubleAnimationUsingKeyFrames GetSingleKeyFrameAnimation(Storyboard storyboard)
    {
        return Assert.IsType<DoubleAnimationUsingKeyFrames>(Assert.Single(storyboard.Children));
    }
#endif
}
