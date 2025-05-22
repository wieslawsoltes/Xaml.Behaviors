using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.WriteableBitmap;

/// <summary>
/// Creates a <see cref="WriteableBitmap"/> and optionally renders it once using a renderer.
/// </summary>
public class WriteableBitmapBehavior : StyledElementBehavior<Image>
{
    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelWidthProperty =
        AvaloniaProperty.Register<WriteableBitmapBehavior, int>(nameof(PixelWidth), 256);

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelHeightProperty =
        AvaloniaProperty.Register<WriteableBitmapBehavior, int>(nameof(PixelHeight), 256);

    /// <summary>
    /// Identifies the <see cref="Renderer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IWriteableBitmapRenderer?> RendererProperty =
        AvaloniaProperty.Register<WriteableBitmapBehavior, IWriteableBitmapRenderer?>(nameof(Renderer));

    /// <summary>
    /// Identifies the <see cref="Bitmap"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<WriteableBitmapBehavior, WriteableBitmap?> BitmapProperty =
        AvaloniaProperty.RegisterDirect<WriteableBitmapBehavior, WriteableBitmap?>(nameof(Bitmap), o => o.Bitmap);

    private WriteableBitmap? _bitmap;

    /// <summary>
    /// Gets or sets the width of the bitmap in pixels. This is an avalonia property.
    /// </summary>
    public int PixelWidth
    {
        get => GetValue(PixelWidthProperty);
        set => SetValue(PixelWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the bitmap in pixels. This is an avalonia property.
    /// </summary>
    public int PixelHeight
    {
        get => GetValue(PixelHeightProperty);
        set => SetValue(PixelHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the renderer used to update the bitmap. This is an avalonia property.
    /// </summary>
    public IWriteableBitmapRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    /// <summary>
    /// Gets the created bitmap.
    /// </summary>
    public WriteableBitmap? Bitmap
    {
        get => _bitmap;
        private set => SetAndRaise(BitmapProperty, ref _bitmap, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        Bitmap = new WriteableBitmap(
            new PixelSize(PixelWidth, PixelHeight),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Unpremul);

        if (AssociatedObject is not null)
        {
            AssociatedObject.Source = Bitmap;
        }

        Renderer?.Render(Bitmap);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Source = null;
        }

        Bitmap?.Dispose();
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
        AssociatedObject?.InvalidateVisual();
    }
}
