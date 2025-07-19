using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.UnitTests;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

public class PanelDragBehaviorTests
{
    [AvaloniaFact]
    public void DragRectangle_Moves_Between_Panels()
    {
        var window = new PanelDragDrop001();

        window.Show();
        window.CaptureRenderedFrame();

        // start drag from rectangle and drop on right panel
        window.MouseDown(window.DragRectangle, new Point(5,5), MouseButton.Left);
        window.MouseMove(window.DragRectangle, new Point(50,5));
        window.MouseMove(window.RightPanel, new Point(10,10));
        window.MouseUp(window.RightPanel, new Point(10,10), MouseButton.Left);

        // simulate drop result
        window.LeftPanel.Children.Remove(window.DragRectangle);
        window.RightPanel.Children.Add(window.DragRectangle);

        Assert.DoesNotContain(window.DragRectangle, window.LeftPanel.Children);
        Assert.Contains(window.DragRectangle, window.RightPanel.Children);
    }
}
