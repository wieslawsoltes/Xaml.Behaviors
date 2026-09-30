// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace Xaml.Interactions.Core;

/// <summary>
/// <see cref="IClipboard"/> implementation over <see cref="Clipboard"/> (Windows.ApplicationModel.DataTransfer).
/// </summary>
public sealed class SystemClipboard : IClipboard
{
    /// <summary>The <c>Text</c> format identifier.</summary>
    public const string TextFormat = "Text";

    /// <summary>The <c>Files</c> format identifier.</summary>
    public const string FilesFormat = "Files";

    /// <summary>The <c>FileNames</c> format identifier.</summary>
    public const string FileNamesFormat = "FileNames";

    private SystemClipboard()
    {
    }

    /// <summary>Gets the shared instance.</summary>
    public static SystemClipboard Instance { get; } = new();

    /// <inheritdoc />
    public Task SetTextAsync(string text)
    {
        var package = new DataPackage();
        package.SetText(text);
        Clipboard.SetContent(package);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<string?> GetTextAsync()
    {
        var content = Clipboard.GetContent();
        return content.Contains(StandardDataFormats.Text) ? await content.GetTextAsync() : null;
    }

    /// <inheritdoc />
    public Task ClearAsync()
    {
        Clipboard.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> GetFormatsAsync()
    {
        IReadOnlyList<string> formats = Clipboard.GetContent().AvailableFormats.Select(ToFormatIdentifier).ToArray();
        return Task.FromResult(formats);
    }

    /// <inheritdoc />
    public Task SetDataAsync(DataPackage data)
    {
        Clipboard.SetContent(data);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<object?> GetDataAsync(string format)
    {
        var content = Clipboard.GetContent();
        if (IsFormat(format, TextFormat))
        {
            return content.Contains(StandardDataFormats.Text) ? await content.GetTextAsync() : null;
        }

        if (IsFormat(format, FilesFormat) || IsFormat(format, FileNamesFormat))
        {
            if (!content.Contains(StandardDataFormats.StorageItems))
            {
                return null;
            }

            var items = await content.GetStorageItemsAsync();
            return IsFormat(format, FilesFormat) ? items : items.Select(static item => item.Path).Where(static path => !string.IsNullOrEmpty(path)).ToArray();
        }

        return content.Contains(format) ? await content.GetDataAsync(format) : null;
    }

    private static string ToFormatIdentifier(string format)
    {
        if (format == StandardDataFormats.Text)
        {
            return TextFormat;
        }

        return format == StandardDataFormats.StorageItems ? FilesFormat : format;
    }

    private static bool IsFormat(string format, string expected) => string.Equals(format, expected, StringComparison.OrdinalIgnoreCase);
}
