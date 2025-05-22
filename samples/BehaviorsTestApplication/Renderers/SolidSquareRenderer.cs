using Avalonia.Media;
using Avalonia;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.Renderers;

public class SolidSquareRenderer : IRenderTargetBitmapSimpleRenderer
{
    public void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.DeepSkyBlue, new Rect(50, 50, 100, 100));
    }
}
