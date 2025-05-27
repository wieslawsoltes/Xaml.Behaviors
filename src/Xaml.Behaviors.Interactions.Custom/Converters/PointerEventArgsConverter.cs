// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Custom.Converters;

/// <summary>
/// Converter for <see cref="PointerEventArgs"/>.
/// </summary>
public class PointerEventArgsConverter : IValueConverter
{
    /// <summary>
    /// Gets the instance of <see cref="PointerEventArgsConverter"/>.
    /// </summary>
    public static readonly PointerEventArgsConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case PointerPressedEventArgs pointerPressedEventArgs:
            {
                if (pointerPressedEventArgs.Source is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }

                var (x, y) = pointerPressedEventArgs.GetPosition(visual);

                return (x, y);
            }
            case PointerReleasedEventArgs pointerReleasedEventArgs:
            {
                if (pointerReleasedEventArgs.Source is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }

                var (x, y) = pointerReleasedEventArgs.GetPosition(visual);

                return (x, y);
            }
            case PointerDeltaEventArgs pointerDeltaEventArgs:
            {
                var (x, y) = pointerDeltaEventArgs.Delta;

                return (x, y);
            }
            case PointerWheelEventArgs pointerWheelEventArgs:
            {
                var (x, y) = pointerWheelEventArgs.Delta;

                return (x, y);
            }
            case PointerEventArgs pointerEventArgs:
            {
                if (pointerEventArgs.Source is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }

                var (x, y) = pointerEventArgs.GetPosition(visual);

                return (x, y);
            }
            default:
                return AvaloniaProperty.UnsetValue;
        }
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }

    /// <summary>
    /// Provides a cached instance of the converter.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The converter instance.</returns>
    public IValueConverter ProvideValue(IServiceProvider serviceProvider) => Instance;
}
