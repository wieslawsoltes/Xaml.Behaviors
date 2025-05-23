using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that draws once into a <see cref="RenderTargetBitmap"/> and assigns it to the associated <see cref="Image"/>.
/// Rendering can be triggered by calling <see cref="IRenderTargetBitmapRenderHost.Render"/>.
/// </summary>
public class StaticRenderTargetBitmapBehavior : StyledElementBehavior<Image>, IRenderTargetBitmapRenderHost
{
    private RenderTargetBitmap? _bitmap;

    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelWidthProperty =
        AvaloniaProperty.Register<StaticRenderTargetBitmapBehavior, int>(nameof(PixelWidth), 200);

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelHeightProperty =
        AvaloniaProperty.Register<StaticRenderTargetBitmapBehavior, int>(nameof(PixelHeight), 200);

    /// <summary>
    /// Identifies the <see cref="Dpi"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Vector> DpiProperty =
        AvaloniaProperty.Register<StaticRenderTargetBitmapBehavior, Vector>(nameof(Dpi), new Vector(96, 96));

    /// <summary>
    /// Identifies the <see cref="Renderer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRenderTargetBitmapSimpleRenderer?> RendererProperty =
        AvaloniaProperty.Register<StaticRenderTargetBitmapBehavior, IRenderTargetBitmapSimpleRenderer?>(nameof(Renderer));

    /// <summary>
    /// Gets or sets the pixel width of the bitmap. This is an avalonia property.
    /// </summary>
    public int PixelWidth
    {
        get => GetValue(PixelWidthProperty);
        set => SetValue(PixelWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the pixel height of the bitmap. This is an avalonia property.
    /// </summary>
    public int PixelHeight
    {
        get => GetValue(PixelHeightProperty);
        set => SetValue(PixelHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the DPI vector of the bitmap. This is an avalonia property.
    /// </summary>
    public Vector Dpi
    {
        get => GetValue(DpiProperty);
        set => SetValue(DpiProperty, value);
    }

    /// <summary>
    /// Gets or sets the renderer used to draw the bitmap. This is an avalonia property.
    /// </summary>
    public IRenderTargetBitmapSimpleRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

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
