// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class AnimationRunnerTests
{
    private sealed class NullAnimationBuilder : IAnimationBuilder
    {
        public int BuildCount { get; private set; }

        public Animation? Build(Control control)
        {
            BuildCount++;
            return null;
        }
    }

    [AvaloniaFact]
    public void TryRun_ReturnsFalseForMissingInputs()
    {
        Assert.False(AnimationRunner.TryRun(null, null));
        Assert.False(AnimationRunner.TryRun(null, new Border()));
    }

    [AvaloniaFact]
    public void TryBuildAndRun_UsesBuilderWhenExplicitAnimationIsMissing()
    {
        var builder = new NullAnimationBuilder();

        bool started = AnimationRunner.TryBuildAndRun(new Border(), null, builder);

        Assert.False(started);
        Assert.Equal(1, builder.BuildCount);
    }

    [AvaloniaFact]
    public void TryBuildAndRun_DoesNotUseBuilderWithoutTarget()
    {
        var builder = new NullAnimationBuilder();

        bool started = AnimationRunner.TryBuildAndRun(null, null, builder);

        Assert.False(started);
        Assert.Equal(0, builder.BuildCount);
    }
}
