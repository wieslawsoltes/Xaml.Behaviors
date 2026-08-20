// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Adapts pointer input to the standalone tilt animation API.
/// </summary>
public class TiltAnimationDemoControl : ContentControl
{
    public static readonly StyledProperty<double> TiltStrengthProperty =
        AvaloniaProperty.Register<TiltAnimationDemoControl, double>(nameof(TiltStrength), 12d);

    public double TiltStrength
    {
        get => GetValue(TiltStrengthProperty);
        set => SetValue(TiltStrengthProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SizeChanged += OnSizeChanged;
        TiltAnimation.UpdateCenterPoint(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        TiltAnimation.Apply(this, e.GetPosition(this), TiltStrength);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        TiltAnimation.Reset(this);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        TiltAnimation.UpdateCenterPoint(this);
    }
}
