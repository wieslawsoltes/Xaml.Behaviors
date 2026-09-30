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
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Behavior that draws once into a <see cref="RenderTargetBitmap"/> and assigns it to the associated <see cref="Image"/>.
/// Rendering can be triggered by calling <see cref="IRenderTargetBitmapRenderHost.Render"/>.
/// </summary>
public partial class StaticRenderTargetBitmapBehavior : StyledElementBehavior<Image>, IRenderTargetBitmapRenderHost
{
    private RenderTargetBitmap? _bitmap;

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
    public partial IRenderTargetBitmapSimpleRenderer? Renderer { get; set; }

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
        Render();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();

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
            Renderer?.Render(ctx);
        }

        AssociatedObject.InvalidateVisual();
    }
}
