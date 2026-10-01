// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Xaml.Interactions.Core;

namespace Xaml.Interactions.Custom;

/// <content>
/// Uno Platform rendering and PNG encoding of the screenshot.
/// </content>
public partial class ScreenshotAction
{
    private const double ScreenshotDpi = 96;

    /// <summary>
    /// Gets the PNG file type offered by the save picker (Avalonia <c>FilePickerFileTypes.ImagePng</c>).
    /// </summary>
    private static FilePickerFileType PngFileType { get; } = new("PNG image")
    {
        Patterns = ["*.png"],
        MimeTypes = ["image/png"],
        AppleUniformTypeIdentifiers = ["public.png"],
    };

    /// <summary>
    /// Renders the element with <see cref="RenderTargetBitmap"/> at its size in device independent pixels (96 DPI),
    /// like the Avalonia implementation.
    /// </summary>
    /// <param name="target">The loaded element to capture.</param>
    /// <returns>The premultiplied BGRA8 pixels, or <c>null</c> when the element has no size or cannot be rendered.</returns>
    private static async Task<RenderedScreenshot?> RenderAsync(FrameworkElement target)
    {
        var width = (int)target.ActualWidth;
        var height = (int)target.ActualHeight;
        if (width <= 0 || height <= 0)
        {
            return null;
        }

        var bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(target, width, height);

        // RenderAsync logs render failures instead of throwing; an empty bitmap means nothing was rendered.
        if (bitmap.PixelWidth <= 0 || bitmap.PixelHeight <= 0)
        {
            return null;
        }

        var pixels = await bitmap.GetPixelsAsync();
        return new RenderedScreenshot(pixels.ToArray(), bitmap.PixelWidth, bitmap.PixelHeight);
    }

    /// <summary>
    /// Encodes the pixels as PNG (Windows.Graphics.Imaging <see cref="BitmapEncoder"/>, SkiaSharp backed on Uno
    /// Platform Skia) and replaces the content of the file.
    /// </summary>
    /// <param name="screenshot">The rendered pixels.</param>
    /// <param name="file">The picked file.</param>
    private static async Task SaveAsync(RenderedScreenshot screenshot, IStorageFile file)
    {
        using var stream = await file.OpenStreamForWriteAsync();
        if (stream.CanSeek)
        {
            stream.SetLength(0);
        }

        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream.AsRandomAccessStream());
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            (uint)screenshot.Width,
            (uint)screenshot.Height,
            ScreenshotDpi,
            ScreenshotDpi,
            screenshot.Pixels);
        await encoder.FlushAsync();
        await stream.FlushAsync();
    }

    /// <summary>
    /// Premultiplied BGRA8 pixels captured by <see cref="RenderTargetBitmap"/>.
    /// </summary>
    /// <param name="Pixels">The pixel data.</param>
    /// <param name="Width">The width in pixels.</param>
    /// <param name="Height">The height in pixels.</param>
    private sealed record RenderedScreenshot(byte[] Pixels, int Width, int Height);
}
