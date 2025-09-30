// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Provides helpers for resolving picker suggested start locations.
/// </summary>
internal static class PickerSuggestedStartLocationHelper
{
    public static IStorageFolder? Resolve(
        IStorageFolder? suggestedStartLocation,
        string? suggestedStartLocationPath,
        bool createSuggestedStartLocationDirectory,
        IStorageProvider? provider)
    {
        if (suggestedStartLocation is not null || provider is null)
        {
            return suggestedStartLocation;
        }

        if (string.IsNullOrWhiteSpace(suggestedStartLocationPath))
        {
            return null;
        }

        if (!TryCreateUri(suggestedStartLocationPath, out var uri))
        {
            return null;
        }

        try
        {
            var folder = provider.TryGetFolderFromPathAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();

            if (folder is null && createSuggestedStartLocationDirectory)
            {
                try
                {
                    var directoryPath = uri.IsFile
                        ? uri.LocalPath
                        : Path.GetFullPath(suggestedStartLocationPath);

                    if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                        folder = provider.TryGetFolderFromPathAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }
                catch
                {
                    // Gracefully ignore failures to create the directory.
                }
            }

            return folder;
        }
        catch
        {
            return null;
        }
    }

    private static bool TryCreateUri(string path, out Uri uri)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out uri))
        {
            return true;
        }

        try
        {
            var fullPath = Path.GetFullPath(path);
            if (Path.IsPathRooted(fullPath))
            {
                uri = new Uri(fullPath);
                return true;
            }
        }
        catch
        {
            // ignored
        }

        uri = null!;
        return false;
    }
}
