using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
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

    [AvaloniaFact]
    public void VirtualizedListBoxItem_ReplacementTransfersContextDragLifecycle()
    {
        var window = new VirtualizedContextDragWindow();
        window.TargetListBox.ItemsSource = Enumerable.Range(0, 100).ToList();

        window.Show();
        Dispatcher.UIThread.RunJobs();

        window.TargetListBox.ScrollIntoView(50);
        Dispatcher.UIThread.RunJobs();

        var container = Assert.IsType<ListBoxItem>(window.TargetListBox.ContainerFromIndex(50));
        var oldBehaviors = Assert.IsType<BehaviorCollection>(
            container.GetValue(Interaction.BehaviorsProperty));
        var oldBehavior = Assert.IsType<TestContextDragBehavior>(Assert.Single(oldBehaviors));
        var behavior = new TestContextDragBehavior();
        var behaviors = new BehaviorCollection { behavior };

        Interaction.SetBehaviors(container, behaviors);

        var handled = false;

        container.AddHandler(
            InputElement.PointerPressedEvent,
            (_, e) => handled = e.Handled,
            Avalonia.Interactivity.RoutingStrategies.Bubble,
            handledEventsToo: true);

        window.TargetListBox.SelectedIndex = 50;
        window.MouseDown(container, new Point(5, 5), MouseButton.Left);
        window.MouseUp(container, new Point(5, 5), MouseButton.Left);

        Assert.Null(oldBehaviors.AssociatedObject);
        Assert.Null(oldBehavior.AssociatedObject);
        Assert.Equal(1, oldBehavior.AttachedToVisualTreeCount);
        Assert.Equal(1, oldBehavior.DetachedFromVisualTreeCount);
        Assert.Same(container, behavior.AssociatedObject);
        Assert.Same(container, behaviors.AssociatedObject);
        Assert.Equal(1, behavior.AttachedToVisualTreeCount);
        Assert.Equal(0, behavior.DetachedFromVisualTreeCount);
        Assert.True(handled);
    }
}
