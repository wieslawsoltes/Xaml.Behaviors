namespace Avalonia.Xaml.Interactions.Custom;

using Avalonia.Media;

/// <summary>
/// Provides a method used by <see cref="StaticRenderTargetBitmapBehavior"/> to draw onto a render target.
/// </summary>
public interface IRenderTargetBitmapSimpleRenderer
{
    /// <summary>
    /// Draws into the provided <see cref="DrawingContext"/>.
    /// </summary>
    /// <param name="context">The drawing context to use.</param>
    void Render(DrawingContext context);
}
