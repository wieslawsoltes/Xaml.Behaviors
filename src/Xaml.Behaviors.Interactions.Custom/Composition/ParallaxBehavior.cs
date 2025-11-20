// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
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

    private CompositionVisual? _visual;
    private IDisposable? _scrollSubscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        UpdateVisual();
        
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
        _visual = null;
    }

    private void UpdateVisual()
    {
        if (AssociatedObject is null) return;
        _visual = ElementComposition.GetElementVisual(AssociatedObject);
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
        OnScrollChanged(value);
    }

    private void OnScrollChanged(Vector offset)
    {
        if (_visual == null) UpdateVisual();
        if (_visual == null) return;

        // We want to offset the element based on the scroll position.
        // If ParallaxRatio is 0.2, we want the element to move 0.2 * ScrollOffset.
        // But wait, if we are inside the ScrollViewer, we are already moving with it.
        // If we want to appear "slower", we need to offset it in the direction of scroll?
        // Or opposite?
        
        // Standard Parallax: Background moves slower than foreground.
        // If we scroll down (Offset.Y increases), the content moves UP relative to the viewport.
        // To make something move slower, we need to push it DOWN (positive Y translation).
        
        // Offset = ScrollOffset * Ratio
        
        var yOffset = offset.Y * ParallaxRatio;
        var xOffset = offset.X * ParallaxRatio;

        _visual.Offset = new Vector3((float)xOffset, (float)yOffset, 0);
    }
}
