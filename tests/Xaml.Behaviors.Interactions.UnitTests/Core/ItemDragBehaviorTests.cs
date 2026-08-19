using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Draggable;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class ItemDragBehaviorTests
{
    private static void AssertDragCanStart(
        ListReorderDragBehaviorWindow window,
        int index)
    {
        var container = Assert.IsType<ListBoxItem>(window.TargetListBox.ContainerFromIndex(index));
        var behaviors = Assert.IsType<BehaviorCollection>(
            container.GetValue(Interaction.BehaviorsProperty));
        var behavior = Assert.IsType<ListReorderDragBehavior>(Assert.Single(behaviors));

        Assert.Same(container, behaviors.AssociatedObject);
        Assert.Same(container, behavior.AssociatedObject);

        window.TargetListBox.SelectedIndex = index;

        window.MouseDown(container, new Point(5, 5), MouseButton.Left);

        Assert.Contains(":dragging", container.Classes);

        window.MouseUp(container, new Point(5, 5), MouseButton.Left);

        Assert.DoesNotContain(":dragging", container.Classes);
    }

    private static void Drag(TopLevel window, Control parent, Control container, bool horizontal)
    {
        var bounds = container.Bounds;
        var startLocal = new Point(bounds.Width / 2, bounds.Height / 2);
        var start = container.TranslatePoint(startLocal, parent) ?? new Point();
        window.MouseDown(parent, start, MouseButton.Left);

        var step = horizontal ? bounds.Width / 3 : bounds.Height / 3;
        var total = horizontal ? bounds.Width * 3 : bounds.Height * 3;

        double moved = step;
        while (moved <= total)
        {
            var point = horizontal ? new Point(start.X + moved, start.Y) : new Point(start.X, start.Y + moved);
            window.MouseMove(parent, point);
            moved += step;
        }

        var end = horizontal ? new Point(start.X + total, start.Y) : new Point(start.X, start.Y + total);
        window.MouseUp(parent, end, MouseButton.Left);
    }

    [AvaloniaFact(Skip = "Drag not supported in headless environment")]
    public void ItemDragBehavior_Reorders_Vertical()
    {
        var window = new ItemDragBehaviorVertical();

        window.Show();
        window.CaptureRenderedFrame();

        var containers = window.TargetListBox.GetRealizedContainers().Cast<ListBoxItem>().ToList();
        Assert.Equal(new[] { "Item1", "Item2", "Item3" }, window.Items.ToArray());

        Drag(window, window.TargetListBox, containers[0], false);

        Assert.Equal(new[] { "Item2", "Item3", "Item1" }, window.Items.ToArray());
    }

    [AvaloniaFact(Skip = "Drag not supported in headless environment")]
    public void ItemDragBehavior_Reorders_Horizontal()
    {
        var window = new ItemDragBehaviorHorizontal();

        window.Show();
        window.CaptureRenderedFrame();

        var containers = window.TargetItemsControl.GetRealizedContainers().Cast<ContentPresenter>().ToList();
        Assert.Equal(new[] { "Item1", "Item2", "Item3" }, window.Items.ToArray());

        Drag(window, window.TargetItemsControl, containers[0], true);

        Assert.Equal(new[] { "Item2", "Item3", "Item1" }, window.Items.ToArray());
    }

    [AvaloniaFact]
    public void ListReorderDragBehavior_CanStartAgainAfterVirtualizedItemMoves()
    {
        var items = new ObservableCollection<int>(Enumerable.Range(0, 100));
        var window = new ListReorderDragBehaviorWindow();
        window.TargetListBox.ItemsSource = items;

        window.Show();
        window.CaptureRenderedFrame();

        AssertDragCanStart(window, 0);

        var movedItem = items[0];
        items.Move(0, 5);
        Dispatcher.UIThread.RunJobs();
        window.CaptureRenderedFrame();

        Assert.Equal(movedItem, items[5]);
        AssertDragCanStart(window, 5);
    }

    [AvaloniaFact]
    public void ListReorderDragBehavior_CanStartAfterListReattaches()
    {
        var items = new ObservableCollection<int>(Enumerable.Range(0, 100));
        var window = new ListReorderDragBehaviorWindow();
        var listBox = window.TargetListBox;
        listBox.ItemsSource = items;

        window.Show();
        window.CaptureRenderedFrame();

        AssertDragCanStart(window, 0);

        window.Content = null;
        Dispatcher.UIThread.RunJobs();
        window.Content = listBox;
        Dispatcher.UIThread.RunJobs();
        window.CaptureRenderedFrame();

        AssertDragCanStart(window, 0);
    }
}
