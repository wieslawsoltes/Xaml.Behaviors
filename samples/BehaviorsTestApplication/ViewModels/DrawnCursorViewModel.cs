using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.ViewModels;

public class DrawnCursorViewModel : ViewModelBase
{
    public DrawnCursorViewModel()
    {
        CursorProvider = new CrossCursorProvider();
    }

    public ICursorProvider CursorProvider { get; }

    private class CrossCursorProvider : ICursorProvider
    {
        public Cursor CreateCursor()
        {
            var size = new PixelSize(32, 32);
            var bitmap = new RenderTargetBitmap(size);
            using (var ctx = bitmap.CreateDrawingContext(true))
            {
                var pen = new Pen(Brushes.Blue, 2);
                ctx.DrawLine(pen, new Point(0, 0), new Point(size.Width, size.Height));
                ctx.DrawLine(pen, new Point(size.Width, 0), new Point(0, size.Height));
            }
            return new Cursor(bitmap, new PixelPoint(16, 16));
        }
    }
}
