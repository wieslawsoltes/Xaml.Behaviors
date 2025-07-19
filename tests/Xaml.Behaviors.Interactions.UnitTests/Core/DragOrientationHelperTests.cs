using Avalonia;
using Avalonia.Layout;
using Avalonia.Xaml.Interactions.Draggable;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class DragOrientationHelperTests
{
    [Fact]
    public void Delta_Is_Symmetric_For_Axes()
    {
        var start = new Point(0, 0);
        var endHorizontal = new Point(10, 0);
        var endVertical = new Point(0, 10);

        var dx = DragOrientationHelper.Delta(start, endHorizontal, Orientation.Horizontal);
        var dy = DragOrientationHelper.Delta(start, endVertical, Orientation.Vertical);

        Assert.Equal(10, dx);
        Assert.Equal(10, dy);
    }
}
