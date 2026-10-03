// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Windows.Storage;

namespace Xaml.Interactions.Core;

/// <summary>
/// Converts a <see cref="IStorageItem"/> to its path.
/// </summary>
public partial class StorageItemToPathConverter : IValueConverter
{
    /// <summary>
    /// Gets a static instance of <see cref="StorageItemToPathConverter"/>.
    /// </summary>
    public static StorageItemToPathConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is IStorageItem storageItem ? storageItem.Path : DependencyProperty.UnsetValue;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => DependencyProperty.UnsetValue;
}
