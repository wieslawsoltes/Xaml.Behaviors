// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that moves the associated element at a different speed than the scrolling container, creating a parallax effect.
/// </summary>
public class ParallaxBehavior : Behavior<Control>, IObserver<Avalonia.Vector>
{
    /// <summary>
    /// Identifies the <seealso cref="SourceScrollViewer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ScrollViewer?> SourceScrollViewerProperty =
        AvaloniaProperty.Register<ParallaxBehavior, ScrollViewer?>(nameof(SourceScrollViewer));

    /// <summary>
    /// Identifies the <seealso cref="ParallaxRatio"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> ParallaxRatioProperty =
        AvaloniaProperty.Register<ParallaxBehavior, double>(nameof(ParallaxRatio), 0.2);

    /// <summary>
    /// Gets or sets the source ScrollViewer. If not set, the behavior will attempt to find a parent ScrollViewer.
    /// </summary>
    [ResolveByName]
    public ScrollViewer? SourceScrollViewer
    {
        get => GetValue(SourceScrollViewerProperty);
        set => SetValue(SourceScrollViewerProperty, value);
    }

    /// <summary>
    /// Gets or sets the parallax ratio. 
    /// 0.0 means no movement (static).
    /// 1.0 means moves with scroll (normal).
    /// Values between 0 and 1 create a "far away" depth effect.
    /// Negative values move in reverse.
    /// </summary>
    public double ParallaxRatio
    {
        get => GetValue(ParallaxRatioProperty);
        set => SetValue(ParallaxRatioProperty, value);
    }

    private IDisposable? _scrollSubscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (SourceScrollViewer == null)
        {
            // Try to find parent ScrollViewer
            var parent = AssociatedObject?.Parent;
            while (parent != null)
            {
                if (parent is ScrollViewer sv)
                {
                    SourceScrollViewer = sv;
                    break;
                }
                parent = parent.Parent;
            }
        }

        if (SourceScrollViewer != null)
        {
            _scrollSubscription = SourceScrollViewer.GetObservable(ScrollViewer.OffsetProperty)
                .Subscribe(this);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        _scrollSubscription?.Dispose();
        _scrollSubscription = null;
    }

    /// <inheritdoc />
    public void OnCompleted()
    {
    }

    /// <inheritdoc />
    public void OnError(Exception error)
    {
    }

    /// <inheritdoc />
    public void OnNext(Avalonia.Vector value)
    {
        ParallaxAnimation.Apply(AssociatedObject, value, ParallaxRatio);
    }
}
