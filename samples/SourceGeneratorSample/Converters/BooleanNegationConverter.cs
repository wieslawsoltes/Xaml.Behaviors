using System;
#if UNO
using Microsoft.UI.Xaml.Data;
#else
using Avalonia.Data.Converters;
#endif

namespace SourceGeneratorSample.Converters
{
    public sealed class BooleanNegationConverter : IValueConverter
    {
        public static readonly BooleanNegationConverter Instance = new();

#if UNO
        // WinUI converters receive a language name instead of a culture.
        public object? Convert(object? value, Type targetType, object? parameter, string language)
#else
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
#endif
        {
            return value is bool b ? !b : true;
        }

#if UNO
        public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#else
        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
#endif
        {
            return value is bool b ? !b : false;
        }
    }
}
