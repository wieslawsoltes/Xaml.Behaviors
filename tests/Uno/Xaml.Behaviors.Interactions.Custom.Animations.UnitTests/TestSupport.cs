// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Xunit;

namespace Xaml.Interactions.Custom.Animations.UnitTests;

public partial class RecordingAction : Xaml.Interactivity.Action
{
    public List<object?> Senders { get; } = [];

    public override object? Execute(object? sender, object? parameter)
    {
        Senders.Add(sender);
        return null;
    }
}

public sealed class OpacityAnimationBuilder(double opacity) : IAnimationBuilder
{
    public FrameworkElement? Control { get; private set; }

    public Storyboard? Build(FrameworkElement control)
    {
        Control = control;
        return TestSupport.CreateOpacityStoryboard(opacity);
    }
}

internal static class TestSupport
{
    public static Storyboard CreateOpacityStoryboard(double opacity, int milliseconds = 30)
    {
        DoubleAnimation animation = new()
        {
            To = opacity,
            Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
        };
        Storyboard.SetTargetProperty(animation, "Opacity");
        return new Storyboard { Children = { animation } };
    }

    public static async Task WaitUntilAsync(Func<bool> condition, string because, int timeoutMilliseconds = 5_000)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            Assert.True(stopwatch.ElapsedMilliseconds < timeoutMilliseconds, $"Timed out waiting until {because}.");
            await Task.Delay(10);
        }
    }
}
