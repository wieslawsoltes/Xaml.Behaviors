using System;
using Avalonia;
using Avalonia.Layout;

namespace Avalonia.Xaml.Interactions.Draggable;

internal static class DragOrientationHelper
{
    public static double Select(double horizontal, double vertical, Orientation orientation)
    {
        return orientation == Orientation.Horizontal ? horizontal : vertical;
    }

    public static double Delta(Point start, Point end, Orientation orientation)
    {
        return Select(end.X - start.X, end.Y - start.Y, orientation);
    }

    public static double Distance(Point start, Point end, Orientation orientation)
    {
        return Math.Abs(Delta(start, end, orientation));
    }

    public static double Start(Rect rect, Orientation orientation)
    {
        return Select(rect.X, rect.Y, orientation);
    }

    public static double Size(Size size, Orientation orientation)
    {
        return Select(size.Width, size.Height, orientation);
    }

    public static double End(Rect rect, Orientation orientation)
    {
        return Start(rect, orientation) + Size(rect.Size, orientation);
    }

    public static double Mid(Rect rect, Orientation orientation)
    {
        return Start(rect, orientation) + Size(rect.Size, orientation) / 2;
    }

    public static double Coordinate(Point point, Orientation orientation)
    {
        return Select(point.X, point.Y, orientation);
    }
}
