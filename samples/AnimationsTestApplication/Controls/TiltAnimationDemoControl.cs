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
/// Adapts pointer input to the standalone tilt animation API.
/// </summary>
public class TiltAnimationDemoControl : ContentControl
{
#if UNO
    public static readonly DependencyProperty TiltStrengthProperty =
        DependencyProperty.Register(
            nameof(TiltStrength),
            typeof(double),
            typeof(TiltAnimationDemoControl),
            new PropertyMetadata(12d));

    public TiltAnimationDemoControl()
    {
        // WinUI has no visual tree attachment notifications: Loaded/Unloaded are raised for the live tree.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
#else
    public static readonly StyledProperty<double> TiltStrengthProperty =
        AvaloniaProperty.Register<TiltAnimationDemoControl, double>(nameof(TiltStrength), 12d);
#endif

    public double TiltStrength
    {
        get => (double)GetValue(TiltStrengthProperty);
        set => SetValue(TiltStrengthProperty, value);
    }

#if UNO
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
        TiltAnimation.UpdateCenterPoint(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
    }
#else
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
#endif

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
#if UNO
        TiltAnimation.Apply(this, e.GetCurrentPoint(this).Position, TiltStrength);
#else
        TiltAnimation.Apply(this, e.GetPosition(this), TiltStrength);
#endif
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
