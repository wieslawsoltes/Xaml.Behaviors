// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Creates a <see cref="WriteableBitmap"/> and optionally renders it once using a renderer.
/// </summary>
public partial class WriteableBitmapBehavior : StyledElementBehavior<Image>
{

    /// <summary>
    /// Gets or sets the width of the bitmap in pixels. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 256)]
    public partial int PixelWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the bitmap in pixels. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 256)]
    public partial int PixelHeight { get; set; }

    /// <summary>
    /// Gets or sets the renderer used to update the bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IWriteableBitmapRenderer? Renderer { get; set; }

    /// <summary>
    /// Gets the created bitmap.
    /// </summary>
    [DirectProperty]
    public partial WriteableBitmap? Bitmap { get; private set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
#if UNO
        // WinUI writeable bitmaps are always BGRA8 (premultiplied) at 96 DPI.
        Bitmap = new WriteableBitmap(PixelWidth, PixelHeight);
#else
        Bitmap = new WriteableBitmap(
            new PixelSize(PixelWidth, PixelHeight),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Unpremul);
#endif

        if (AssociatedObject is not null)
        {
            AssociatedObject.Source = Bitmap;
        }

        Renderer?.Render(Bitmap);
#if UNO
        Bitmap.Invalidate();
#endif
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Source = null;
        }

#if !UNO
        Bitmap?.Dispose();
#endif
        Bitmap = null;
    }

    /// <summary>
    /// Invokes the renderer to update the bitmap.
    /// </summary>
    public void Render()
    {
        if (Bitmap is null || Renderer is null)
        {
            return;
        }

        Renderer.Render(Bitmap);
#if UNO
        // WinUI presents the pixel buffer changes once the bitmap is invalidated.
        Bitmap.Invalidate();
#else
        AssociatedObject?.InvalidateVisual();
#endif
    }
}
