// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class AnimationFactoryTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void CreateFadeIn_CreatesDelayedOpacityStoryboard()
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(200);
        TimeSpan duration = TimeSpan.FromMilliseconds(300);

        Storyboard storyboard = AnimationFactory.CreateFadeIn(delay, duration);

        DoubleAnimationUsingKeyFrames animation = Assert.IsType<DoubleAnimationUsingKeyFrames>(Assert.Single(storyboard.Children));
        Assert.Equal("Opacity", Storyboard.GetTargetProperty(animation));
        Assert.Equal(TimeSpan.FromMilliseconds(500), animation.Duration.TimeSpan);
        Assert.Equal(FillBehavior.Stop, storyboard.FillBehavior);
        Assert.Collection(
            animation.KeyFrames,
            keyFrame => AssertKeyFrame(keyFrame, TimeSpan.Zero, 0d),
            keyFrame => AssertKeyFrame(keyFrame, delay, 0d),
            keyFrame => AssertKeyFrame(keyFrame, delay + duration, 1d));
    }

    [UnoHeadlessTheory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void CreateFadeIn_RejectsNegativeTimes(double delayMilliseconds, double durationMilliseconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AnimationFactory.CreateFadeIn(
            TimeSpan.FromMilliseconds(delayMilliseconds),
            TimeSpan.FromMilliseconds(durationMilliseconds)));
    }

    [UnoHeadlessFact]
    public void CreateFadeInTimeline_PreservesExplicitKeyTimes()
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(500);
        TimeSpan completion = TimeSpan.FromMilliseconds(650);
        TimeSpan totalDuration = TimeSpan.FromMilliseconds(750);

        Storyboard storyboard = AnimationFactory.CreateFadeInTimeline(delay, completion, totalDuration);

        DoubleAnimationUsingKeyFrames animation = Assert.IsType<DoubleAnimationUsingKeyFrames>(Assert.Single(storyboard.Children));
        Assert.Equal(totalDuration, animation.Duration.TimeSpan);
        Assert.Collection(
            animation.KeyFrames,
            keyFrame => AssertKeyFrame(keyFrame, TimeSpan.Zero, 0d),
            keyFrame => AssertKeyFrame(keyFrame, delay, 0d),
            keyFrame => AssertKeyFrame(keyFrame, completion, 1d));
    }

    [UnoHeadlessTheory]
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

    [UnoHeadlessFact]
    public async Task CreateFadeIn_RunsAndReleasesOpacity()
    {
        Border target = new() { Width = 20d, Height = 20d };
        await Session.ShowAsync(target);
        Storyboard storyboard = AnimationFactory.CreateFadeIn(TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(40));

        Task run = AnimationRunner.RunAsync(storyboard, target);
        await TestHelpers.CompletesAsync(run);

        Assert.Equal(1d, target.Opacity);
    }

    [UnoHeadlessFact]
    public void FluidMoveAnimation_CreatesTranslationStoryboard()
    {
        TimeSpan duration = TimeSpan.FromMilliseconds(250);

        Storyboard storyboard = FluidMoveAnimation.Create(12d, -8d, duration);

        Assert.Equal(FillBehavior.HoldEnd, storyboard.FillBehavior);
        Assert.Collection(
            storyboard.Children,
            timeline => AssertTranslation(timeline, "(UIElement.RenderTransform).(TranslateTransform.X)", 12d),
            timeline => AssertTranslation(timeline, "(UIElement.RenderTransform).(TranslateTransform.Y)", -8d));

        void AssertTranslation(Timeline timeline, string path, double offset)
        {
            DoubleAnimationUsingKeyFrames animation = Assert.IsType<DoubleAnimationUsingKeyFrames>(timeline);
            Assert.Equal(path, Storyboard.GetTargetProperty(animation));
            Assert.Equal(duration, animation.Duration.TimeSpan);
            Assert.Collection(
                animation.KeyFrames,
                keyFrame => AssertKeyFrame(keyFrame, TimeSpan.Zero, offset),
                keyFrame => AssertKeyFrame(keyFrame, duration, 0d));
        }
    }

    [UnoHeadlessFact]
    public void FluidMoveAnimation_RejectsNegativeDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FluidMoveAnimation.Create(0d, 0d, TimeSpan.FromMilliseconds(-1d)));
    }

    [UnoHeadlessFact]
    public async Task FluidMoveAnimation_TryRunPreparesControlTransformAndMovesBack()
    {
        Border target = new() { Width = 20d, Height = 20d, RenderTransform = new RotateTransform() };
        await Session.ShowAsync(target);

        bool started = FluidMoveAnimation.TryRun(target, 12d, -8d, TimeSpan.FromMilliseconds(60));

        Assert.True(started);
        TranslateTransform transform = Assert.IsType<TranslateTransform>(target.RenderTransform);
        await TestHelpers.WaitUntilAsync(() => transform.X == 0d && transform.Y == 0d, "the element moved back");
        Assert.False(FluidMoveAnimation.TryRun(null, 0d, 0d, TimeSpan.Zero));
    }

    private static void AssertKeyFrame(DoubleKeyFrame keyFrame, TimeSpan keyTime, double value)
    {
        Assert.IsType<LinearDoubleKeyFrame>(keyFrame);
        Assert.Equal(keyTime, keyFrame.KeyTime.TimeSpan);
        Assert.Equal(value, keyFrame.Value);
    }
}
