// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactivity;
using Vector = Windows.Foundation.Point;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that moves the associated element at a different speed than the scrolling container, creating a parallax effect.
/// </summary>
public partial class ParallaxBehavior : Behavior<Control>, IObserver<Vector>
{

    /// <summary>
    /// Gets or sets the source ScrollViewer. If not set, the behavior will attempt to find a parent ScrollViewer.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ScrollViewer? SourceScrollViewer { get; set; }

    /// <summary>
    /// Gets or sets the parallax ratio. 
    /// 0.0 means no movement (static).
    /// 1.0 means moves with scroll (normal).
    /// Values between 0 and 1 create a "far away" depth effect.
    /// Negative values move in reverse.
    /// </summary>
    [StyledProperty(DefaultValue = 0.2)]
    public partial double ParallaxRatio { get; set; }

    private IDisposable? _scrollSubscription;
    private ParallaxAnimation? _animation;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        _animation = ParallaxAnimation.TryCreate(AssociatedObject);
        if (SourceScrollViewer == null)
        {
            // Try to find parent ScrollViewer
#if UNO
            // WinUI elements inside templates have no logical parent: walk the visual tree.
            var parent = AssociatedObject is null ? null : VisualTreeHelper.GetParent(AssociatedObject);
            while (parent != null)
            {
                if (parent is ScrollViewer sv)
                {
                    SourceScrollViewer = sv;
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
#else
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
#endif
        }

        if (SourceScrollViewer != null)
        {
#if UNO
            _scrollSubscription = SubscribeToOffset(SourceScrollViewer);
#else
            _scrollSubscription = SourceScrollViewer.GetObservable(ScrollViewer.OffsetProperty)
                .Subscribe(this);
#endif
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        _scrollSubscription?.Dispose();
        _scrollSubscription = null;
        _animation = null;
    }

#if UNO
    private IDisposable SubscribeToOffset(ScrollViewer scrollViewer)
    {
        // WinUI has no ScrollViewer.Offset property: report the offsets when the view changes.
        OnNext(new Vector(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset));

        void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
            => OnNext(new Vector(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset));

        scrollViewer.ViewChanged += OnViewChanged;
        return DisposableAction.Create(() => scrollViewer.ViewChanged -= OnViewChanged);
    }

#endif
    /// <inheritdoc />
    public void OnCompleted()
    {
    }

    /// <inheritdoc />
    public void OnError(Exception error)
    {
    }

    /// <inheritdoc />
    public void OnNext(Vector value)
    {
        _animation ??= ParallaxAnimation.TryCreate(AssociatedObject);
        _animation?.Apply(value, ParallaxRatio);
    }
}
