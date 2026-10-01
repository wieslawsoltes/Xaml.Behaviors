// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
// The library's platform animation type is a WinUI storyboard on Uno Platform (PlatformAnimation, see src/Uno/PORTING.md).
using Animation = Microsoft.UI.Xaml.Media.Animation.Storyboard;
#else
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
#endif

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

#if UNO
    public static readonly DependencyProperty InitialDelayProperty =
        DependencyProperty.Register(
            nameof(InitialDelay),
            typeof(TimeSpan),
            typeof(KeyFrameAnimationDemoControl),
            new PropertyMetadata(TimeSpan.FromMilliseconds(150)));

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(TimeSpan),
            typeof(KeyFrameAnimationDemoControl),
            new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

    public static readonly DependencyProperty UseBuilderProperty =
        DependencyProperty.Register(
            nameof(UseBuilder),
            typeof(bool),
            typeof(KeyFrameAnimationDemoControl),
            new PropertyMetadata(false));

    public KeyFrameAnimationDemoControl()
    {
        // WinUI has no AttachedToVisualTree notification: Loaded is raised when the control enters the live tree.
        Loaded += OnLoaded;
    }
#else
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
#endif

    public TimeSpan InitialDelay
    {
        get => (TimeSpan)GetValue(InitialDelayProperty);
        set => SetValue(InitialDelayProperty, value);
    }

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public bool UseBuilder
    {
        get => (bool)GetValue(UseBuilderProperty);
        set => SetValue(UseBuilderProperty, value);
    }

#if UNO
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Play();
    }
#else
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Play();
    }
#endif

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
