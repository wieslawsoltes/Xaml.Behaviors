// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class UnoAnimationRunnerTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private sealed class NullAnimationBuilder : IAnimationBuilder
    {
        public int BuildCount { get; private set; }

        public Storyboard? Build(FrameworkElement control)
        {
            BuildCount++;
            return null;
        }
    }

    private sealed class OpacityAnimationBuilder(double opacity) : IAnimationBuilder
    {
        public FrameworkElement? Control { get; private set; }

        public Storyboard? Build(FrameworkElement control)
        {
            Control = control;
            return CreateOpacityStoryboard(opacity);
        }
    }

    [UnoHeadlessFact]
    public void TryRun_ReturnsFalseForMissingInputs()
    {
        Assert.False(AnimationRunner.TryRun(null, null));
        Assert.False(AnimationRunner.TryRun(null, new Border()));
        Assert.False(AnimationRunner.TryRun(new Storyboard(), null));
        Assert.Null(AnimationRunner.TryRunAsync(null, null));
        Assert.Null(AnimationRunner.TryRunAsync(null, new Border()));
    }

    [UnoHeadlessFact]
    public void TryBuildAndRun_UsesBuilderWhenExplicitAnimationIsMissing()
    {
        NullAnimationBuilder builder = new();

        bool started = AnimationRunner.TryBuildAndRun(new Border(), null, builder);

        Assert.False(started);
        Assert.Equal(1, builder.BuildCount);
    }

    [UnoHeadlessFact]
    public void TryBuildAndRun_DoesNotUseBuilderWithoutTarget()
    {
        NullAnimationBuilder builder = new();

        bool started = AnimationRunner.TryBuildAndRun(null, null, builder);

        Assert.False(started);
        Assert.Equal(0, builder.BuildCount);
    }

    [UnoHeadlessFact]
    public async Task TryBuildAndRunAsync_ReturnsCompletionTaskForExplicitAnimation()
    {
        NullAnimationBuilder builder = new();
        Border target = new();
        await Session.ShowAsync(target);

        Task? task = AnimationRunner.TryBuildAndRunAsync(target, new Storyboard(), builder);

        Assert.NotNull(task);
        Assert.Equal(0, builder.BuildCount);
        await TestHelpers.CompletesAsync(task);
    }

    [UnoHeadlessFact]
    public async Task TryBuildAndRunAsync_RunsBuiltStoryboardOnControl()
    {
        OpacityAnimationBuilder builder = new(0.25d);
        Border target = new() { Width = 10d, Height = 10d };
        await Session.ShowAsync(target);

        Task? task = AnimationRunner.TryBuildAndRunAsync(target, null, builder);

        Assert.NotNull(task);
        Assert.Same(target, builder.Control);
        await TestHelpers.CompletesAsync(task);
        Assert.Equal(0.25d, target.Opacity, 3);
    }

    [UnoHeadlessFact]
    public async Task RunAsync_TargetsUntargetedTimelinesAtTheTarget()
    {
        Border first = new() { Width = 10d, Height = 10d };
        Border second = new() { Width = 10d, Height = 10d };
        await Session.ShowAsync(new StackPanel { Children = { first, second } });
        Storyboard storyboard = CreateOpacityStoryboard(0.5d);

        await TestHelpers.CompletesAsync(AnimationRunner.RunAsync(storyboard, first));
        storyboard.Stop();
        await TestHelpers.CompletesAsync(AnimationRunner.RunAsync(storyboard, second));

        Assert.Equal(0.5d, second.Opacity, 3);
    }

    private static Storyboard CreateOpacityStoryboard(double opacity)
    {
        DoubleAnimation animation = new() { To = opacity, Duration = new Duration(TimeSpan.FromMilliseconds(30)) };
        Storyboard.SetTargetProperty(animation, "Opacity");
        return new Storyboard { Children = { animation } };
    }
}
