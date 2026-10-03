// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Windows.Storage;

namespace Xaml.Interactions.Core;

/// <summary>
/// Converts a <see cref="IStorageFile"/> to a write stream (<c>Task&lt;Stream&gt;</c>).
/// </summary>
public partial class StorageFileToWriteStreamConverter : IValueConverter
{
    /// <summary>
    /// Gets a static instance of <see cref="StorageFileToWriteStreamConverter"/>.
    /// </summary>
    public static StorageFileToWriteStreamConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is IStorageFile storageFile ? storageFile.OpenStreamForWriteAsync() : DependencyProperty.UnsetValue;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => DependencyProperty.UnsetValue;
}
