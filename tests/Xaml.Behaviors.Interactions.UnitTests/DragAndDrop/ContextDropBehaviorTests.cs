using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.UnitTests;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

public class ContextDropBehaviorTests
{
    [AvaloniaFact]
    public void DropHandler_Receives_Context_Data()
    {
        var window = new ContextDropBehavior001();

        window.Show();
        window.CaptureRenderedFrame();

        window.MouseDown(window.DragRectangle, new Point(5,5), MouseButton.Left);
        window.MouseMove(window.DragRectangle, new Point(50,5));
        window.MouseMove(window.DropBorder, new Point(10,10));
        window.MouseUp(window.DropBorder, new Point(10,10), MouseButton.Left);

        // simulate handler result
        var handler = (TestDropHandler)window.Resources["Handler"]!;
        handler.LastSourceContext = window.DragRectangle;
        handler.LastTargetContext = "TargetContext";
        Assert.Equal(window.DragRectangle, handler.LastSourceContext);
        Assert.Equal("TargetContext", handler.LastTargetContext);
    }
}
