// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Behavior that draws into a <see cref="RenderTargetBitmap"/> and assigns it to the associated <see cref="Image"/>.
/// </summary>
public partial class RenderTargetBitmapBehavior : StyledElementBehavior<Image>, IRenderTargetBitmapRenderHost
{
    private RenderTargetBitmap? _bitmap;
    private DispatcherTimer? _timer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Gets or sets the pixel width of the bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 200)]
    public partial int PixelWidth { get; set; }

    /// <summary>
    /// Gets or sets the pixel height of the bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 200)]
    public partial int PixelHeight { get; set; }

    /// <summary>
    /// Gets or sets the DPI vector of the bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "new Vector(96, 96)")]
    public partial Vector Dpi { get; set; }

    /// <summary>
    /// Gets or sets the renderer used to draw the bitmap. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IRenderTargetBitmapRenderer? Renderer { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (AssociatedObject is null)
        {
            return;
        }

        _bitmap = new RenderTargetBitmap(new PixelSize(PixelWidth, PixelHeight), Dpi);
        AssociatedObject.Source = _bitmap;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();

        _timer?.Stop();
        if (_timer is not null)
        {
            _timer.Tick -= OnTick;
        }
        _timer = null;

        _bitmap?.Dispose();
        _bitmap = null;
    }

    /// <inheritdoc />
    public void Render()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        if (_bitmap is null)
        {
            return;
        }

        using (var ctx = _bitmap.CreateDrawingContext())
        {
            Renderer?.Render(ctx, _stopwatch.Elapsed);
        }

        AssociatedObject.InvalidateVisual();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        Render();
    }
}
