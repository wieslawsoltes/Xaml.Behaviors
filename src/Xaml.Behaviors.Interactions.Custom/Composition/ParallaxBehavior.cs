// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
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
public partial class ParallaxBehavior : Behavior<Control>, IObserver<Avalonia.Vector>
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
        _animation = null;
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
        _animation ??= ParallaxAnimation.TryCreate(AssociatedObject);
        _animation?.Apply(value, ParallaxRatio);
    }
}
