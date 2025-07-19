using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

public class ContextDragBehaviorTests
{
    [AvaloniaFact]
    public void Escape_Cancels_Drag()
    {
        var window = new ContextDragEscapeWindow();

        window.Show();
        window.TargetBorder.Focus();

        var start = new Point(5, 5);
        window.MouseDown(window.TargetBorder, start, MouseButton.Left);

        window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);

        var move = new Point(20, 20);
        window.MouseMove(window.TargetBorder, move);
        window.MouseUp(window.TargetBorder, move, MouseButton.Left);

        Assert.False(window.TestBehavior.BeforeCalled);
        Assert.False(window.TestBehavior.AfterCalled);
    }
}
