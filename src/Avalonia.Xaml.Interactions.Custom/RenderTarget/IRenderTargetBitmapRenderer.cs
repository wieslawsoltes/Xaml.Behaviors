namespace Avalonia.Xaml.Interactions.Custom;

using System;
using Avalonia.Media;

/// <summary>
/// Provides a method used by <see cref="RenderTargetBitmapBehavior"/> to draw onto a render target.
/// </summary>
public interface IRenderTargetBitmapRenderer
{
    /// <summary>
    /// Draws into the provided <see cref="DrawingContext"/>.
    /// </summary>
    /// <param name="context">The drawing context to use.</param>
    /// <param name="elapsed">The elapsed time since the behavior was attached.</param>
    void Render(DrawingContext context, TimeSpan elapsed);
}
