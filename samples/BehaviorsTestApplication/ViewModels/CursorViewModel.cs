using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace BehaviorsTestApplication.ViewModels;

public class CursorViewModel : ViewModelBase
{
    public CursorViewModel()
    {
        StandardCursors = Enum.GetValues<StandardCursorType>()
            .Select(x => new StandardCursorModel(x))
            .ToList();

        var size = new PixelSize(32, 32);
        var bitmap = new RenderTargetBitmap(size);
        using (var ctx = bitmap.CreateDrawingContext(clear: true))
        {
            var pen = new Pen(Brushes.Red, 2);
            ctx.DrawLine(pen, new Point(0, 0), new Point(size.Width, size.Height));
            ctx.DrawLine(pen, new Point(size.Width, 0), new Point(0, size.Height));
        }
        CustomCursor = new Cursor(bitmap, new PixelPoint(16, 16));
    }

    public IEnumerable<StandardCursorModel> StandardCursors { get; }

    public Cursor CustomCursor { get; }
}

public class StandardCursorModel
{
    public StandardCursorModel(StandardCursorType type)
    {
        Type = type;
        Cursor = new Cursor(type);
    }

    public StandardCursorType Type { get; }

    public Cursor Cursor { get; }
}
