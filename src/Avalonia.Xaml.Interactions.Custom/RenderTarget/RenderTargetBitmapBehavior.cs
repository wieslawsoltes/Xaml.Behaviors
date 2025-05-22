using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that draws into a <see cref="RenderTargetBitmap"/> and assigns it to the associated <see cref="Image"/>.
/// </summary>
public class RenderTargetBitmapBehavior : StyledElementBehavior<Image>, IRenderTargetBitmapRenderHost
{
    private RenderTargetBitmap? _bitmap;
    private DispatcherTimer? _timer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelWidthProperty =
        AvaloniaProperty.Register<RenderTargetBitmapBehavior, int>(nameof(PixelWidth), 200);

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PixelHeightProperty =
        AvaloniaProperty.Register<RenderTargetBitmapBehavior, int>(nameof(PixelHeight), 200);

    /// <summary>
    /// Identifies the <see cref="Dpi"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Vector> DpiProperty =
        AvaloniaProperty.Register<RenderTargetBitmapBehavior, Vector>(nameof(Dpi), new Vector(96, 96));

    /// <summary>
    /// Identifies the <see cref="Renderer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IRenderTargetBitmapRenderer?> RendererProperty =
        AvaloniaProperty.Register<RenderTargetBitmapBehavior, IRenderTargetBitmapRenderer?>(nameof(Renderer));

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
    public IRenderTargetBitmapRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

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
