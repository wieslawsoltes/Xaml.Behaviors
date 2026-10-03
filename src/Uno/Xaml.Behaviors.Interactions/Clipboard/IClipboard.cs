// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace Xaml.Interactions.Core;

/// <summary>
/// Clipboard used by the clipboard actions.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia <c>IClipboard</c> used by the clipboard actions. The default
/// implementation is <see cref="SystemClipboard"/>; assign another implementation (for example in tests) through the
/// <c>Clipboard</c> property of the actions.
/// </remarks>
public interface IClipboard
{
    /// <summary>Places text on the clipboard.</summary>
    /// <param name="text">The text.</param>
    Task SetTextAsync(string text);

    /// <summary>Gets the text on the clipboard.</summary>
    /// <returns>The text, or <c>null</c> when the clipboard holds no text.</returns>
    Task<string?> GetTextAsync();

    /// <summary>Clears the clipboard.</summary>
    Task ClearAsync();

    /// <summary>Gets the formats available on the clipboard (<c>Text</c>, <c>Files</c> and platform formats).</summary>
    /// <returns>The format identifiers.</returns>
    Task<IReadOnlyList<string>> GetFormatsAsync();

    /// <summary>Places a data package on the clipboard.</summary>
    /// <param name="data">The data package.</param>
    Task SetDataAsync(DataPackage data);

    /// <summary>
    /// Gets clipboard data for a format: <c>Text</c> (string), <c>Files</c> (storage items), <c>FileNames</c> (paths)
    /// or any platform format identifier.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The data, or <c>null</c> when the format is not available.</returns>
    Task<object?> GetDataAsync(string format);
}
