namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Defines a render host that can update an underlying <see cref="RenderTargetBitmap"/>.
/// </summary>
public interface IRenderTargetBitmapRenderHost
{
    /// <summary>
    /// Requests that the render host renders its content to the bitmap.
    /// </summary>
    void Render();
}
