// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Adapts pointer input to the standalone orbit animation API.
/// </summary>
public class OrbitAnimationDemoControl : ContentControl
{
    private readonly OrbitAnimation _animation = new();
    private bool _isPressed;
    private Point _lastPosition;

#if UNO
    public static readonly DependencyProperty SensitivityProperty =
        DependencyProperty.Register(
            nameof(Sensitivity),
            typeof(double),
            typeof(OrbitAnimationDemoControl),
            new PropertyMetadata(0.5d));

    public OrbitAnimationDemoControl()
    {
        // WinUI has no visual tree attachment notifications: Loaded/Unloaded are raised for the live tree.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
#else
    public static readonly StyledProperty<double> SensitivityProperty =
        AvaloniaProperty.Register<OrbitAnimationDemoControl, double>(nameof(Sensitivity), 0.5d);
#endif

    public double Sensitivity
    {
        get => (double)GetValue(SensitivityProperty);
        set => SetValue(SensitivityProperty, value);
    }

#if UNO
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
        OrbitAnimation.UpdateCenterPoint(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        _isPressed = false;
        _animation.Reset();
    }
#else
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
#endif

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _isPressed = true;
#if UNO
        _lastPosition = e.GetCurrentPoint(this).Position;
        CapturePointer(e.Pointer);
#else
        _lastPosition = e.GetPosition(this);
        e.Pointer.Capture(this);
#endif
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isPressed)
        {
            return;
        }

#if UNO
        // Vector is Windows.Foundation.Point on Uno Platform (no subtraction operator).
        Point position = e.GetCurrentPoint(this).Position;
        Vector delta = new(position.X - _lastPosition.X, position.Y - _lastPosition.Y);
#else
        Point position = e.GetPosition(this);
        Vector delta = position - _lastPosition;
#endif
        _lastPosition = position;
        _animation.Rotate(this, delta, Sensitivity);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isPressed = false;
#if UNO
        ReleasePointerCapture(e.Pointer);
#else
        e.Pointer.Capture(null);
#endif
        e.Handled = true;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        OrbitAnimation.UpdateCenterPoint(this);
    }
}
