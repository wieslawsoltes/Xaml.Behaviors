// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows rotating the attached control in 3D space using pointer manipulation.
/// </summary>
public class OrbitEffectBehavior : StyledElementBehavior<Control>
{
    private readonly OrbitAnimation _animation = new();
    private bool _isPressed;
    private Point _lastPosition;

    /// <summary>
    /// Identifies the <seealso cref="Sensitivity"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> SensitivityProperty =
        AvaloniaProperty.Register<OrbitEffectBehavior, double>(nameof(Sensitivity), 0.5);

    /// <summary>
    /// Gets or sets the sensitivity of the rotation.
    /// </summary>
    public double Sensitivity
    {
        get => GetValue(SensitivityProperty);
        set => SetValue(SensitivityProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerReleased += OnPointerReleased;
            AssociatedObject.SizeChanged += OnSizeChanged;
            OrbitAnimation.UpdateCenterPoint(AssociatedObject);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerReleased -= OnPointerReleased;
            AssociatedObject.SizeChanged -= OnSizeChanged;
        }
        _isPressed = false;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isPressed = true;
        _lastPosition = e.GetPosition(AssociatedObject);
        e.Pointer.Capture(AssociatedObject);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isPressed = false;
        e.Pointer.Capture(null);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPressed || AssociatedObject is null)
        {
            return;
        }

        var currentPosition = e.GetPosition(AssociatedObject);
        var delta = currentPosition - _lastPosition;
        _lastPosition = currentPosition;

        _animation.Rotate(AssociatedObject, delta, Sensitivity);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        OrbitAnimation.UpdateCenterPoint(AssociatedObject);
    }
}
