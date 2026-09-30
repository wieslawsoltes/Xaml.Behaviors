// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Creates a <see cref="WriteableBitmap"/> and updates it using a renderer on a timer.
/// </summary>
public partial class WriteableBitmapRenderBehavior : StyledElementBehavior<Image>
{

    private DispatcherTimer? _timer;

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

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (_timer is not null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }

        if (AssociatedObject is not null)
        {
            AssociatedObject.Source = null;
        }

#if !UNO
        Bitmap?.Dispose();
#endif
        Bitmap = null;
    }

#if UNO
    private void OnTick(object? sender, object e)
#else
    private void OnTick(object? sender, EventArgs e)
#endif
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
