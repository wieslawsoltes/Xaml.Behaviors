using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace BehaviorsTestApplication.Converters;

public class ClassesToStringConverter : IMultiValueConverter
{
    public static readonly ClassesToStringConverter Instance = new();

    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count == 2 && values[0] is int && values[1] is Classes classes)
        {
            return string.Join(" ", classes);
        }

        return AvaloniaProperty.UnsetValue;
    }

    public IMultiValueConverter ProvideValue(IServiceProvider serviceProvider) => Instance;
}
