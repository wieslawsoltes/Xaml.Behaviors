// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

internal static class TestHelpers
{
    public static async Task WaitUntilAsync(Func<bool> condition, string because, int timeoutMilliseconds = 5_000)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            Assert.True(stopwatch.ElapsedMilliseconds < timeoutMilliseconds, $"Timed out waiting until {because}.");
            await Task.Delay(10);
        }
    }

    public static async Task CompletesAsync(Task task, int timeoutMilliseconds = 5_000)
    {
        Task completed = await Task.WhenAny(task, Task.Delay(timeoutMilliseconds));
        Assert.Same(task, completed);
        await task;
    }

    public static CompositionVisual GetVisual(UIElement element) => ElementCompositionPreview.GetElementVisual(element);

    public static void AssertNear(System.Numerics.Vector3 expected, System.Numerics.Vector3 actual, float tolerance = 0.001f)
    {
        Assert.True(
            System.Numerics.Vector3.Distance(expected, actual) <= tolerance,
            $"Expected {expected} but was {actual}.");
    }
}
