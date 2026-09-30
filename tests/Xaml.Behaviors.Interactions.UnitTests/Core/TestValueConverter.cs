using System;
using System.Globalization;
#if UNO
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
#else
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

internal class TestValueConverter : IValueConverter
{
    public static readonly TestValueConverter Instance = new ();

#if UNO
    public object? Convert(object? value, Type targetType, object? parameter, string language)
#else
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#endif
    {
        if (value is RoutedEventArgs args)
        {
            return args.Source?.GetType().Name;
        }

        throw new ArgumentException("Invalid value type");
    }

#if UNO
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#else
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
#endif
    {
        throw new NotImplementedException();
    }
}
