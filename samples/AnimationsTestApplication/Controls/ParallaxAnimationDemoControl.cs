// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Connects a scroll offset to the standalone parallax animation API.
/// </summary>
public class ParallaxAnimationDemoControl : ContentControl, IObserver<Vector>
{
    private IDisposable? _subscription;
    private ParallaxAnimation? _animation;
    private bool _isAttached;

#if UNO
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(ScrollViewer),
            typeof(ParallaxAnimationDemoControl),
            new PropertyMetadata(null, OnSourceChanged));

    public static readonly DependencyProperty RatioProperty =
        DependencyProperty.Register(
            nameof(Ratio),
            typeof(double),
            typeof(ParallaxAnimationDemoControl),
            new PropertyMetadata(0.25d));

    public ParallaxAnimationDemoControl()
    {
        // WinUI has no visual tree attachment notifications: Loaded/Unloaded are raised for the live tree.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
#else
    public static readonly StyledProperty<ScrollViewer?> SourceProperty =
        AvaloniaProperty.Register<ParallaxAnimationDemoControl, ScrollViewer?>(nameof(Source));

    public static readonly StyledProperty<double> RatioProperty =
        AvaloniaProperty.Register<ParallaxAnimationDemoControl, double>(nameof(Ratio), 0.25d);
#endif

#if !UNO
    // WinUI XAML has no name resolution attribute: the Uno view binds the scroll viewer with x:Bind.
    [ResolveByName]
#endif
    public ScrollViewer? Source
    {
        get => (ScrollViewer?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public double Ratio
    {
        get => (double)GetValue(RatioProperty);
        set => SetValue(RatioProperty, value);
    }

#if UNO
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isAttached = true;
        _animation = ParallaxAnimation.TryCreate(this);
        Subscribe();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isAttached = false;
        _subscription?.Dispose();
        _subscription = null;
        _animation = null;
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ParallaxAnimationDemoControl { _isAttached: true } control)
        {
            control.Subscribe();
        }
    }
#else
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        _animation = ParallaxAnimation.TryCreate(this);
        Subscribe();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _subscription?.Dispose();
        _subscription = null;
        _animation = null;
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
#endif

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(Vector value)
    {
        _animation ??= ParallaxAnimation.TryCreate(this);
        _animation?.Apply(value, Ratio);
    }

    private void Subscribe()
    {
        _subscription?.Dispose();
#if UNO
        _subscription = Source is { } source ? new ScrollOffsetSubscription(source, this) : null;
#else
        _subscription = Source?.GetObservable(ScrollViewer.OffsetProperty).Subscribe(this);
#endif
    }
#if UNO

    /// <summary>
    /// Reports the current and subsequent scroll offsets of a WinUI scroll viewer (the counterpart of observing
    /// Avalonia's <c>ScrollViewer.Offset</c> property).
    /// </summary>
    private sealed class ScrollOffsetSubscription : IDisposable
    {
        private readonly ScrollViewer _source;
        private readonly IObserver<Vector> _observer;

        public ScrollOffsetSubscription(ScrollViewer source, IObserver<Vector> observer)
        {
            _source = source;
            _observer = observer;
            _source.ViewChanged += OnViewChanged;
            Publish();
        }

        public void Dispose()
        {
            _source.ViewChanged -= OnViewChanged;
        }

        private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
        {
            Publish();
        }

        private void Publish()
        {
            _observer.OnNext(new Vector(_source.HorizontalOffset, _source.VerticalOffset));
        }
    }
#endif
}
