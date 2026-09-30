// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Runs WinUI storyboards with the Avalonia <c>Animation.RunAsync(Animatable)</c> shape used by the shared sources.
/// </summary>
internal static class StoryboardExtensions
{
    /// <summary>
    /// Targets the storyboard at an object and starts it.
    /// </summary>
    /// <remarks>
    /// Like an Avalonia animation, the storyboard animates the supplied target: every child timeline without a
    /// <see cref="Storyboard.TargetNameProperty"/> is (re)targeted at <paramref name="target"/>. A storyboard is a
    /// single running instance on WinUI, so starting it again restarts it; pending tasks complete with the restarted run.
    /// </remarks>
    /// <param name="storyboard">The storyboard to run.</param>
    /// <param name="target">The animated object.</param>
    /// <returns>A task that completes when the storyboard completes.</returns>
    public static Task RunAsync(this Storyboard storyboard, DependencyObject target)
    {
        ArgumentNullException.ThrowIfNull(storyboard);
        ArgumentNullException.ThrowIfNull(target);

        SetTarget(storyboard, target);

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler<object>? handler = null;
        handler = (_, _) =>
        {
            storyboard.Completed -= handler;
            completion.TrySetResult();
        };
        storyboard.Completed += handler;
        storyboard.Begin();
        return completion.Task;
    }

    private static void SetTarget(Storyboard storyboard, DependencyObject target)
    {
        TimelineCollection children = storyboard.Children;
        for (int i = 0; i < children.Count; i++)
        {
            Timeline child = children[i];
            if (child is Storyboard nested)
            {
                SetTarget(nested, target);
            }
            else if (string.IsNullOrEmpty(Storyboard.GetTargetName(child)))
            {
                Storyboard.SetTarget(child, target);
            }
        }
    }
}
