// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;

namespace AnimationsTestApplication.Controls;

/// <summary>
/// Demonstrates direct fluid translation animations.
/// </summary>
public class FluidMoveAnimationDemoControl : ContentControl
{
    private bool _reverse;

    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<FluidMoveAnimationDemoControl, TimeSpan>(
            nameof(Duration),
            TimeSpan.FromMilliseconds(450));

    public static readonly StyledProperty<double> DistanceProperty =
        AvaloniaProperty.Register<FluidMoveAnimationDemoControl, double>(nameof(Distance), 180d);

    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public double Distance
    {
        get => GetValue(DistanceProperty);
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
