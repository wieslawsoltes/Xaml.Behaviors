using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.WriteableBitmap;

/// <summary>
/// Invokes an <see cref="IWriteableBitmapRenderer"/> to render into a bitmap.
/// </summary>
public class WriteableBitmapRenderAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="Renderer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IWriteableBitmapRenderer?> RendererProperty =
        AvaloniaProperty.Register<WriteableBitmapRenderAction, IWriteableBitmapRenderer?>(nameof(Renderer));

    /// <summary>
    /// Identifies the <see cref="Bitmap"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<WriteableBitmap?> BitmapProperty =
        AvaloniaProperty.Register<WriteableBitmapRenderAction, WriteableBitmap?>(nameof(Bitmap));

    /// <summary>
    /// Gets or sets the renderer used when executing the action. This is an avalonia property.
    /// </summary>
    public IWriteableBitmapRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    /// <summary>
    /// Gets or sets the target bitmap. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public WriteableBitmap? Bitmap
    {
        get => GetValue(BitmapProperty);
        set => SetValue(BitmapProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled || Renderer is null || Bitmap is null)
        {
            return false;
        }

        Renderer.Render(Bitmap);
        return true;
    }
}
