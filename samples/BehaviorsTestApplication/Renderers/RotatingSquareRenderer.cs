using System;
using Avalonia.Media;
using Avalonia;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Renderers;

public class RotatingSquareRenderer : IRenderTargetBitmapRenderer
{
    public void Render(DrawingContext context, TimeSpan elapsed)
    {
        using (context.PushTransform(Matrix.CreateTranslation(-100, -100)
                                      * Matrix.CreateRotation(elapsed.TotalSeconds)
                                      * Matrix.CreateTranslation(100, 100)))
        {
            context.FillRectangle(Brushes.Fuchsia, new Rect(50, 50, 100, 100));
        }
    }
}
