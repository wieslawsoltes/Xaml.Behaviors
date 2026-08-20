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
    private DoubleTransition? _transition;
    private bool _dimmed;

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
