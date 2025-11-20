// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that applies a 3D tilt rotation to the element based on the pointer position.
/// </summary>
public class TiltEffectBehavior : Behavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="TiltStrength"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> TiltStrengthProperty =
        AvaloniaProperty.Register<TiltEffectBehavior, double>(nameof(TiltStrength), 5.0);

    /// <summary>
    /// Gets or sets the maximum tilt angle in degrees.
    /// </summary>
    public double TiltStrength
    {
        get => GetValue(TiltStrengthProperty);
        set => SetValue(TiltStrengthProperty, value);
    }

    private CompositionVisual? _visual;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerExited += OnPointerExited;
            
            UpdateVisual();
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerExited -= OnPointerExited;
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
            
            // Ensure center point is center of the control
            _visual.CenterPoint = new Vector3((float)AssociatedObject.Bounds.Width / 2, (float)AssociatedObject.Bounds.Height / 2, 0);
            
            // We need to update CenterPoint when bounds change
            AssociatedObject.SizeChanged += (s, e) =>
            {
                if (_visual != null)
                {
                    _visual.CenterPoint = new Vector3((float)e.NewSize.Width / 2, (float)e.NewSize.Height / 2, 0);
                }
            };
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is null) return;
        if (_visual is null) UpdateVisual();
        if (_visual is null) return;

        var p = e.GetPosition(AssociatedObject);
        var center = new Point(AssociatedObject.Bounds.Width / 2, AssociatedObject.Bounds.Height / 2);
        
        // Calculate offset from center (-1 to 1 range roughly)
        var xDiff = (p.X - center.X) / center.X;
        var yDiff = (p.Y - center.Y) / center.Y;

        // Invert Y because moving mouse up (negative Y) should tilt "away" (positive rotation around X)
        // Moving mouse right (positive X) should tilt "right" (positive rotation around Y)
        
        var axis = new Vector3((float)-yDiff, (float)xDiff, 0);
        
        if (axis.LengthSquared() > 0.001)
        {
            axis = Vector3.Normalize(axis);
            
            // Distance from center determines angle
            var dist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            // Clamp dist to 1.0
            dist = Math.Min(dist, 1.0);
            
            var angle = (float)(dist * TiltStrength * (Math.PI / 180.0)); // Convert to radians
            
            var quaternion = Quaternion.CreateFromAxisAngle(axis, angle);

            var orientationAnimation = _visual.Compositor.CreateQuaternionKeyFrameAnimation();
            orientationAnimation.InsertKeyFrame(1.0f, quaternion);
            orientationAnimation.Duration = TimeSpan.FromMilliseconds(50);
            _visual.StartAnimation("Orientation", orientationAnimation);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (_visual != null)
        {
            var orientationAnimation = _visual.Compositor.CreateQuaternionKeyFrameAnimation();
            orientationAnimation.InsertKeyFrame(1.0f, Quaternion.Identity);
            orientationAnimation.Duration = TimeSpan.FromMilliseconds(400);
            _visual.StartAnimation("Orientation", orientationAnimation);
        }
    }
}
