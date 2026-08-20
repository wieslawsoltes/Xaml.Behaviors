// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Plays a simple fade in animation when the associated control is attached.
/// </summary>
public class FadeInBehavior : AttachedToVisualTreeBehavior<Visual>
{
    /// <summary>
    /// Gets or sets the delay before the animation starts.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> InitialDelayProperty =
        AvaloniaProperty.Register<FadeInBehavior, TimeSpan>(nameof(InitialDelay), TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Gets or sets the duration of the fade in animation.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FadeInBehavior, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan InitialDelay
    {
        get => GetValue(InitialDelayProperty);
        set => SetValue(InitialDelayProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Called when the behavior is attached to the visual tree.
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        Animation.Animation animation = AnimationFactory.CreateFadeIn(InitialDelay, Duration);
        AnimationRunner.TryRun(animation, AssociatedObject);

        return DisposableAction.Empty;
    }
}
