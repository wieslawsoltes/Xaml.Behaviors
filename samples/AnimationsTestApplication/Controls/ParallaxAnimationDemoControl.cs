// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Connects a scroll offset to the standalone parallax animation API.
/// </summary>
public class ParallaxAnimationDemoControl : ContentControl, IObserver<Vector>
{
    private IDisposable? _subscription;
    private bool _isAttached;

    public static readonly StyledProperty<ScrollViewer?> SourceProperty =
        AvaloniaProperty.Register<ParallaxAnimationDemoControl, ScrollViewer?>(nameof(Source));

    public static readonly StyledProperty<double> RatioProperty =
        AvaloniaProperty.Register<ParallaxAnimationDemoControl, double>(nameof(Ratio), 0.25d);

    [ResolveByName]
    public ScrollViewer? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public double Ratio
    {
        get => GetValue(RatioProperty);
        set => SetValue(RatioProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        Subscribe();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _subscription?.Dispose();
        _subscription = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty && _isAttached)
        {
            Subscribe();
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(Vector value)
    {
        ParallaxAnimation.Apply(this, value, Ratio);
    }

    private void Subscribe()
    {
        _subscription?.Dispose();
        _subscription = Source?.GetObservable(ScrollViewer.OffsetProperty).Subscribe(this);
    }
}
