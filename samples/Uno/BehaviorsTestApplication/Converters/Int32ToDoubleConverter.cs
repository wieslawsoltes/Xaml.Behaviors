// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Converts <see cref="int"/> view model values to the <see cref="double"/> values of WinUI controls and back
/// (Avalonia binds an <c>int</c> to <c>NumericUpDown.Value</c> directly; WinUI's <c>NumberBox.Value</c> is a
/// <see cref="double"/> and bindings do not convert numeric types).
/// </summary>
public sealed class Int32ToDoubleConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is int number ? (double)number : DependencyProperty.UnsetValue;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is double number && !double.IsNaN(number)
            ? (int)Math.Round(number, MidpointRounding.AwayFromZero)
            : DependencyProperty.UnsetValue;
}
