using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Converts a <seealso cref="IStorageFile"/> to a read stream.
/// </summary>
public class StorageFileToReadStreamConverter : IValueConverter
{
    /// <summary>
    /// Gets a static instance of <see cref="StorageFileToReadStreamConverter"/>.
    /// </summary>
    public static StorageFileToReadStreamConverter Instance { get; } = new ();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IStorageFile storageFile)
        {
            return storageFile.OpenReadAsync();
        }

        return BindingOperations.DoNothing;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
