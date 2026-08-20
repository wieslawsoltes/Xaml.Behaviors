// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Demonstrates direct use of animation factories, builders, and runners.
/// </summary>
public class KeyFrameAnimationDemoControl : ContentControl
{
    private sealed class FadeAnimationBuilder(TimeSpan initialDelay, TimeSpan duration) : IAnimationBuilder
    {
        public Animation? Build(Control control)
        {
            return AnimationFactory.CreateFadeIn(initialDelay, duration);
        }
    }

    public static readonly StyledProperty<TimeSpan> InitialDelayProperty =
        AvaloniaProperty.Register<KeyFrameAnimationDemoControl, TimeSpan>(
            nameof(InitialDelay),
            TimeSpan.FromMilliseconds(150));

    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<KeyFrameAnimationDemoControl, TimeSpan>(
            nameof(Duration),
            TimeSpan.FromMilliseconds(500));

    public static readonly StyledProperty<bool> UseBuilderProperty =
        AvaloniaProperty.Register<KeyFrameAnimationDemoControl, bool>(nameof(UseBuilder));

    public TimeSpan InitialDelay
    {
        get => GetValue(InitialDelayProperty);
        set => SetValue(InitialDelayProperty, value);
    }

    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public bool UseBuilder
    {
        get => GetValue(UseBuilderProperty);
        set => SetValue(UseBuilderProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Play();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Play();
        e.Handled = true;
    }

    private void Play()
    {
        if (UseBuilder)
        {
            var builder = new FadeAnimationBuilder(InitialDelay, Duration);
            AnimationRunner.TryBuildAndRun(this, animation: null, builder);
            return;
        }

        Animation animation = AnimationFactory.CreateFadeIn(InitialDelay, Duration);
        AnimationRunner.TryRun(animation, this);
    }
}
