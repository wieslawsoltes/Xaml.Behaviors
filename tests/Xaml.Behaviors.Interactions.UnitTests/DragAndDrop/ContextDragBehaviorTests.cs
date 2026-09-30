using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;
#endif

public class ContextDragBehaviorTests
{
    [AvaloniaFact]
    public void PointerPressed_Is_Not_Handled_For_Selected_Item_Content()
    {
        var window = new ContextDragEscapeWindow();
        var pointerPressed = false;
        window.SelectedItemContent.PointerPressed += (_, _) => pointerPressed = true;

        window.Show();
        window.MouseDown(window.SelectedItemContent, new Point(5, 5), MouseButton.Left);

        Assert.True(pointerPressed);
    }

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

#if UNO
        // The Uno page uses a ListView: the Uno Platform ListBox generates no ListBoxItem containers.
        var container = Assert.IsType<ListViewItem>(window.TargetListBox.ContainerFromIndex(50));
#else
        var container = Assert.IsType<ListBoxItem>(window.TargetListBox.ContainerFromIndex(50));
#endif
        var oldBehaviors = Assert.IsType<BehaviorCollection>(
            container.GetValue(Interaction.BehaviorsProperty));
        var oldBehavior = Assert.IsType<TestContextDragBehavior>(Assert.Single(oldBehaviors));
        var behavior = new TestContextDragBehavior();
        var behaviors = new BehaviorCollection { behavior };
#if UNO
        // Uno Platform recycles containers through the visual tree (unloaded, then loaded again for the scrolled item),
        // Avalonia keeps them attached: the old behavior may already have seen attach/detach pairs.
        var recycledAttachments = oldBehavior.DetachedFromVisualTreeCount;
#endif

        Interaction.SetBehaviors(container, behaviors);

        var handled = false;

        container.AddHandler(
            InputElement.PointerPressedEvent,
            (_, e) => handled = e.Handled,
#if UNO
            RoutingStrategies.Bubble,
#else
            Avalonia.Interactivity.RoutingStrategies.Bubble,
#endif
            handledEventsToo: true);

        window.TargetListBox.SelectedIndex = 50;
        window.MouseDown(container, new Point(5, 5), MouseButton.Left);
        window.MouseUp(container, new Point(5, 5), MouseButton.Left);

        Assert.Null(oldBehaviors.AssociatedObject);
        Assert.Null(oldBehavior.AssociatedObject);
#if UNO
        Assert.Equal(1 + recycledAttachments, oldBehavior.AttachedToVisualTreeCount);
        Assert.Equal(1 + recycledAttachments, oldBehavior.DetachedFromVisualTreeCount);
#else
        Assert.Equal(1, oldBehavior.AttachedToVisualTreeCount);
        Assert.Equal(1, oldBehavior.DetachedFromVisualTreeCount);
#endif
        Assert.Same(container, behavior.AssociatedObject);
        Assert.Same(container, behaviors.AssociatedObject);
        Assert.Equal(1, behavior.AttachedToVisualTreeCount);
        Assert.Equal(0, behavior.DetachedFromVisualTreeCount);
        Assert.True(handled);
    }
}
