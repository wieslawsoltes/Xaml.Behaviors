using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;

namespace Avalonia.Xaml.Interactions.Custom;

internal class VisualDebugAdorner : Control
{
    private readonly IBrush _brush;

    public VisualDebugAdorner(Control adornedElement, IBrush brush)
    {
        AdornedElement = adornedElement;
        _brush = brush;
        IsHitTestVisible = false;
    }

    public Control AdornedElement { get; }

    public override void Render(DrawingContext context)
    {
        var rect = AdornedElement.Bounds;
        // Draw a border around the element
        var pen = new Pen(_brush, 2);
        context.DrawRectangle(null, pen, new Rect(0, 0, Bounds.Width, Bounds.Height));
        
        // Draw a semi-transparent overlay
        var overlayBrush = new SolidColorBrush(((SolidColorBrush)_brush).Color, 0.3);
        context.DrawRectangle(overlayBrush, null, new Rect(0, 0, Bounds.Width, Bounds.Height));
    }
}
