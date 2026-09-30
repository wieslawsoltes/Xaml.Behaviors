// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Globalization;
#if UNO
using Microsoft.UI.Xaml.Data;
#else
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Platform.Storage;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// Converts a <seealso cref="IStorageFile"/> to a path.
/// </summary>
public class StorageFileToWriteStreamConverter : IValueConverter
{
    /// <summary>
    /// Gets a static instance of <see cref="StorageFileToWriteStreamConverter"/>.
    /// </summary>
    public static StorageFileToWriteStreamConverter Instance { get; } = new ();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IStorageFile storageFile)
        {
            return storageFile.OpenWriteAsync();
        }

        return BindingOperations.DoNothing;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
