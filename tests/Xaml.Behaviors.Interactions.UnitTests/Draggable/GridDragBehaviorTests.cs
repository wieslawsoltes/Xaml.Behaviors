using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Draggable;

public class GridDragBehaviorTests
{
    [AvaloniaFact]
    public void GridDragBehavior_Respects_Bounds()
    {
        var window = new GridDragBehavior001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("GridDragBehavior_001_0.png");

        // Drag over the right button but outside bounding border
        window.MouseDown(window.DragGrid, new Point(25, 25), MouseButton.Left);
        window.MouseMove(window.DragGrid, new Point(75, 25), RawInputModifiers.LeftMouseButton);
        window.MouseUp(window.DragGrid, new Point(75, 25), MouseButton.Left);

        window.CaptureRenderedFrame()?.Save("GridDragBehavior_001_1.png");

        Assert.Equal(0, Grid.GetColumn(window.LeftButton));
        Assert.Equal(1, Grid.GetColumn(window.RightButton));
    }
}
