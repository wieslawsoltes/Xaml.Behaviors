// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Builds and runs Avalonia animations independently of behaviors, actions, and triggers.
/// </summary>
public static class AnimationRunner
{
    /// <summary>
    /// Runs an animation on an animatable target.
    /// </summary>
    /// <param name="animation">The animation to run.</param>
    /// <param name="target">The target of the animation.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static Task RunAsync(Animation.Animation animation, Animatable target)
    {
        return animation.RunAsync(target);
    }

    /// <summary>
    /// Starts an animation when both the animation and target are available.
    /// </summary>
    /// <param name="animation">The animation to run.</param>
    /// <param name="target">The target of the animation.</param>
    /// <returns><c>true</c> when the animation was started; otherwise, <c>false</c>.</returns>
    public static bool TryRun(Animation.Animation? animation, Animatable? target)
    {
        return TryRunAsync(animation, target) is not null;
    }

    /// <summary>
    /// Starts an animation when both the animation and target are available and returns its completion task.
    /// </summary>
    /// <param name="animation">The animation to run.</param>
    /// <param name="target">The target of the animation.</param>
    /// <returns>The animation completion task, or <c>null</c> when the animation could not be started.</returns>
    public static Task? TryRunAsync(Animation.Animation? animation, Animatable? target)
    {
        return animation is not null && target is not null
            ? RunAsync(animation, target)
            : null;
    }

    /// <summary>
    /// Selects an explicit animation or builds one and starts it on a control.
    /// </summary>
    /// <param name="control">The target control.</param>
    /// <param name="animation">The preferred animation.</param>
    /// <param name="animationBuilder">The fallback animation builder.</param>
    /// <returns><c>true</c> when an animation was started; otherwise, <c>false</c>.</returns>
    public static bool TryBuildAndRun(
        Control? control,
        Animation.Animation? animation,
        IAnimationBuilder? animationBuilder)
    {
        return TryBuildAndRunAsync(control, animation, animationBuilder) is not null;
    }

    /// <summary>
    /// Selects an explicit animation or builds one, starts it on a control, and returns its completion task.
    /// </summary>
    /// <param name="control">The target control.</param>
    /// <param name="animation">The preferred animation.</param>
    /// <param name="animationBuilder">The fallback animation builder.</param>
    /// <returns>The animation completion task, or <c>null</c> when no animation could be started.</returns>
    public static Task? TryBuildAndRunAsync(
        Control? control,
        Animation.Animation? animation,
        IAnimationBuilder? animationBuilder)
    {
        if (control is null)
        {
            return null;
        }

        Animation.Animation? selectedAnimation = animation ?? animationBuilder?.Build(control);
        return TryRunAsync(selectedAnimation, control);
    }
}
