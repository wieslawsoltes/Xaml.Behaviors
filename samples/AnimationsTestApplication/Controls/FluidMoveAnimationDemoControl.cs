// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactions.Custom;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;
#endif

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Demonstrates direct fluid translation animations.
/// </summary>
public class FluidMoveAnimationDemoControl : ContentControl
{
    private bool _reverse;

#if UNO
    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(TimeSpan),
            typeof(FluidMoveAnimationDemoControl),
            new PropertyMetadata(TimeSpan.FromMilliseconds(450)));

    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register(
            nameof(Distance),
            typeof(double),
            typeof(FluidMoveAnimationDemoControl),
            new PropertyMetadata(180d));
#else
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FluidMoveAnimationDemoControl, TimeSpan>(
            nameof(Duration),
            TimeSpan.FromMilliseconds(450));

    public static readonly StyledProperty<double> DistanceProperty =
        AvaloniaProperty.Register<FluidMoveAnimationDemoControl, double>(nameof(Distance), 180d);
#endif

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public double Distance
    {
        get => (double)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        double offset = _reverse ? -Distance : Distance;
        _reverse = !_reverse;
        FluidMoveAnimation.TryRun(this, offset, 0d, Duration);
        e.Handled = true;
    }
}
