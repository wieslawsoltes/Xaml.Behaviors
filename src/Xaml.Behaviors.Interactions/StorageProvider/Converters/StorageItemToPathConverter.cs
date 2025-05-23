using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Converts a <seealso cref="IStorageItem"/> to a path.
/// </summary>
public class StorageItemToPathConverter : IValueConverter
{
    /// <summary>
    /// Gets a static instance of <see cref="StorageItemToPathConverter"/>.
    /// </summary>
    public static StorageItemToPathConverter Instance { get; } = new ();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IStorageItem storageItem)
        {
            return storageItem.Path;
        }

        return BindingOperations.DoNothing;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
