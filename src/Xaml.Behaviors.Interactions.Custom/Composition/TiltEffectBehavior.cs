// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
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

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerExited += OnPointerExited;
            AssociatedObject.SizeChanged += OnSizeChanged;
            TiltAnimation.UpdateCenterPoint(AssociatedObject);
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
            AssociatedObject.SizeChanged -= OnSizeChanged;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is not null)
        {
            TiltAnimation.Apply(AssociatedObject, e.GetPosition(AssociatedObject), TiltStrength);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        TiltAnimation.Reset(AssociatedObject);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        TiltAnimation.UpdateCenterPoint(AssociatedObject);
    }
}
