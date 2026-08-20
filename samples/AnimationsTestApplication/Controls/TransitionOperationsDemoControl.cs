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
/// Demonstrates direct transition collection operations.
/// </summary>
public class TransitionOperationsDemoControl : ContentControl
{
    private IDisposable? _transitionSubscription;
    private DoubleTransition? _transition;
    private int _transitionChangeCount;
    private bool _dimmed;

    /// <summary>
    /// Identifies the <see cref="TransitionChangeCount"/> direct property.
    /// </summary>
    public static readonly DirectProperty<TransitionOperationsDemoControl, int> TransitionChangeCountProperty =
        AvaloniaProperty.RegisterDirect<TransitionOperationsDemoControl, int>(
            nameof(TransitionChangeCount),
            control => control.TransitionChangeCount);

    /// <summary>
    /// Gets the number of transition collection values reported by the active observation.
    /// </summary>
    public int TransitionChangeCount
    {
        get => _transitionChangeCount;
        private set => SetAndRaise(TransitionChangeCountProperty, ref _transitionChangeCount, value);
    }

    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<TransitionOperationsDemoControl, TimeSpan>(
            nameof(Duration),
            TimeSpan.FromMilliseconds(300));

    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TransitionChangeCount = 0;
        _transitionSubscription = TransitionOperations.Observe(
            this,
            _ => TransitionChangeCount++);
        _transition = new DoubleTransition
        {
            Property = OpacityProperty,
            Duration = Duration
        };
        TransitionOperations.Add(this, _transition);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        TransitionOperations.Remove(this, _transition);
        _transitionSubscription?.Dispose();
        _transitionSubscription = null;
        _transition = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _dimmed = !_dimmed;
        Opacity = _dimmed ? 0.25d : 1d;
        e.Handled = true;
    }
}
