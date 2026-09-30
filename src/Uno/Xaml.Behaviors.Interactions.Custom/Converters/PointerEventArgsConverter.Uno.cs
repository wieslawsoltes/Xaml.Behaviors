// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactions.Custom.Converters;

/// <content>
/// Uno Platform conversion.
/// </content>
/// <remarks>
/// WinUI raises every pointer event with <see cref="PointerRoutedEventArgs"/>. Wheel events convert to the wheel
/// delta in notches (Avalonia <c>PointerWheelEventArgs.Delta</c>: one unit per notch, horizontal wheel on x), the
/// other pointer events to the position relative to the element that raised the event.
/// </remarks>
public partial class PointerEventArgsConverter
{
    private const double WheelDeltaPerNotch = 120.0;

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not PointerRoutedEventArgs pointerEventArgs)
        {
            return DependencyProperty.UnsetValue;
        }

        if (pointerEventArgs.OriginalSource is not UIElement visual)
        {
            return DependencyProperty.UnsetValue;
        }

        var point = pointerEventArgs.GetCurrentPoint(visual);
        var wheelDelta = point.Properties.MouseWheelDelta;
        if (wheelDelta != 0)
        {
            var delta = wheelDelta / WheelDeltaPerNotch;
            return point.Properties.IsHorizontalMouseWheel ? (delta, 0.0) : (0.0, delta);
        }

        return (point.Position.X, point.Position.Y);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}
