using System;
using Avalonia.Data.Converters;

namespace SourceGeneratorSample.Converters
{
    public sealed class BooleanNegationConverter : IValueConverter
    {
        public static readonly BooleanNegationConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool b ? !b : true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool b ? !b : false;
        }
    }
}
