// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Adapts pointer input to the standalone orbit animation API.
/// </summary>
public class OrbitAnimationDemoControl : ContentControl
{
    private readonly OrbitAnimation _animation = new();
    private bool _isPressed;
    private Point _lastPosition;

    public static readonly StyledProperty<double> SensitivityProperty =
        AvaloniaProperty.Register<OrbitAnimationDemoControl, double>(nameof(Sensitivity), 0.5d);

    public double Sensitivity
    {
        get => GetValue(SensitivityProperty);
        set => SetValue(SensitivityProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SizeChanged += OnSizeChanged;
        OrbitAnimation.UpdateCenterPoint(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        _isPressed = false;
        _animation.Reset();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _isPressed = true;
        _lastPosition = e.GetPosition(this);
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isPressed)
        {
            return;
        }

        Point position = e.GetPosition(this);
        Vector delta = position - _lastPosition;
        _lastPosition = position;
        _animation.Rotate(this, delta, Sensitivity);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isPressed = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        OrbitAnimation.UpdateCenterPoint(this);
    }
}
