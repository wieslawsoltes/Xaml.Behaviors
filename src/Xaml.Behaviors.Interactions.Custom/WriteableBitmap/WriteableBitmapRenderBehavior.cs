using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Creates a <see cref="WriteableBitmap"/> and updates it using a renderer on a timer.
/// </summary>
public class WriteableBitmapRenderBehavior : StyledElementBehavior<Image>
{
    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelWidthProperty =
        AvaloniaProperty.Register<WriteableBitmapRenderBehavior, int>(nameof(PixelWidth), 256);

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelHeightProperty =
        AvaloniaProperty.Register<WriteableBitmapRenderBehavior, int>(nameof(PixelHeight), 256);

    /// <summary>
    /// Identifies the <see cref="Renderer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IWriteableBitmapRenderer?> RendererProperty =
        AvaloniaProperty.Register<WriteableBitmapRenderBehavior, IWriteableBitmapRenderer?>(nameof(Renderer));

    /// <summary>
    /// Identifies the <see cref="Bitmap"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<WriteableBitmapRenderBehavior, WriteableBitmap?> BitmapProperty =
        AvaloniaProperty.RegisterDirect<WriteableBitmapRenderBehavior, WriteableBitmap?>(nameof(Bitmap), o => o.Bitmap);

    private DispatcherTimer? _timer;
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

        Bitmap?.Dispose();
        Bitmap = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (Bitmap is null || Renderer is null)
        {
            return;
        }

        Renderer.Render(Bitmap);
        AssociatedObject?.InvalidateVisual();
    }
}
