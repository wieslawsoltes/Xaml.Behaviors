using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows rotating the attached control in 3D space using pointer manipulation.
/// </summary>
public class OrbitEffectBehavior : StyledElementBehavior<Control>
{
    private CompositionVisual? _visual;
    private bool _isPressed;
    private Point _lastPosition;
    private Quaternion _currentOrientation = Quaternion.Identity;

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
            UpdateVisual();
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
        }
        _visual = null;
    }

    private void UpdateVisual()
    {
        if (AssociatedObject is null) return;

        var visual = ElementComposition.GetElementVisual(AssociatedObject);
        if (visual != null && _visual != visual)
        {
            _visual = visual;
            _visual.CenterPoint = new Vector3((float)AssociatedObject.Bounds.Width / 2, (float)AssociatedObject.Bounds.Height / 2, 0);

            AssociatedObject.SizeChanged += (s, e) =>
            {
                if (_visual != null)
                {
                    _visual.CenterPoint = new Vector3((float)AssociatedObject.Bounds.Width / 2, (float)AssociatedObject.Bounds.Height / 2, 0);
                }
            };
        }
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
        if (!_isPressed || _visual == null) return;

        var currentPosition = e.GetPosition(AssociatedObject);
        var delta = currentPosition - _lastPosition;
        _lastPosition = currentPosition;

        float rotX = (float)(delta.Y * Sensitivity * 0.01);
        float rotY = (float)(delta.X * Sensitivity * 0.01);

        var rotationX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -rotX);
        var rotationY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotY);
        
        _currentOrientation = _currentOrientation * rotationX * rotationY;
        _currentOrientation = Quaternion.Normalize(_currentOrientation);

        var orientationAnimation = _visual.Compositor.CreateQuaternionKeyFrameAnimation();
        orientationAnimation.InsertKeyFrame(1.0f, _currentOrientation);
        orientationAnimation.Duration = TimeSpan.FromMilliseconds(1);
        _visual.StartAnimation("Orientation", orientationAnimation);
    }
}
