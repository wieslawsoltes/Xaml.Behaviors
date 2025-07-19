using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Draggable;

public class CanvasDragBehaviorTests
{
    [AvaloniaFact]
    public void CanvasDragBehavior_Within_Bounds()
    {
        var window = new CanvasDragBehavior001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("CanvasDragBehavior_001_0.png");

        // Begin drag inside the button
        window.MouseDown(window.DragCanvas, new Point(15, 15), MouseButton.Left);
        window.MouseMove(window.DragCanvas, new Point(120, 120), RawInputModifiers.LeftMouseButton);
        window.MouseUp(window.DragCanvas, new Point(120, 120), MouseButton.Left);

        window.CaptureRenderedFrame()?.Save("CanvasDragBehavior_001_1.png");

        Assert.Equal(80d, Canvas.GetLeft(window.TargetButton));
        Assert.Equal(80d, Canvas.GetTop(window.TargetButton));
    }
}
