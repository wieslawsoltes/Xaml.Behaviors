using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using Xaml.Interactions.Draggable;
using Xaml.Interactivity;
// The test windows (HeadlessTestWindow) are elements shown in the headless session window.
using TopLevel = Microsoft.UI.Xaml.UIElement;
#else
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Draggable;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

public class ItemDragBehaviorTests
{
    private static void AssertDragCanStart(
        ListReorderDragBehaviorWindow window,
        int index)
    {
#if UNO
        // The Uno test window uses the virtualizing WinUI ListView (Uno Platform's ListBox has no ListBoxItem containers).
        var container = Assert.IsType<ListViewItem>(window.TargetListBox.ContainerFromIndex(index));
#else
        var container = Assert.IsType<ListBoxItem>(window.TargetListBox.ContainerFromIndex(index));
#endif
        var behaviors = Assert.IsType<BehaviorCollection>(
            container.GetValue(Interaction.BehaviorsProperty));
        var behavior = Assert.IsType<ListReorderDragBehavior>(Assert.Single(behaviors));

        Assert.Same(container, behaviors.AssociatedObject);
        Assert.Same(container, behavior.AssociatedObject);

        window.TargetListBox.SelectedIndex = index;

#if WINUI
        // Real input: a second press at the same place within the double click time is a double click, which does not
        // start a drag.
        UnoHeadlessSession.Current.Mouse.Wait(System.TimeSpan.FromSeconds(1));
#endif
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

#if UNO
    // The Uno headless session injects pointer input, so the drag runs.
    [AvaloniaFact]
#else
    [AvaloniaFact(Skip = "Drag not supported in headless environment")]
#endif
    public void ItemDragBehavior_Reorders_Vertical()
    {
        var window = new ItemDragBehaviorVertical();

        window.Show();
        window.CaptureRenderedFrame();

#if UNO
        var containers = window.TargetListBox.GetRealizedContainers().Cast<ListViewItem>().ToList();
#else
        var containers = window.TargetListBox.GetRealizedContainers().Cast<ListBoxItem>().ToList();
#endif
        Assert.Equal(new[] { "Item1", "Item2", "Item3" }, window.Items.ToArray());

        Drag(window, window.TargetListBox, containers[0], false);

        Assert.Equal(new[] { "Item2", "Item3", "Item1" }, window.Items.ToArray());
    }

#if UNO
    [AvaloniaFact]
#else
    [AvaloniaFact(Skip = "Drag not supported in headless environment")]
#endif
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

    [AvaloniaFact]
    public void ListReorderDragBehavior_Placeholder_Follows_The_Pointer_Across_Targets()
    {
        var items = new ObservableCollection<string>(Enumerable.Range(0, 5).Select(i => $"Item{i}"));
        var window = new ListReorderDragBehaviorWindow();
        var listBox = window.TargetListBox;
        listBox.ItemsSource = items;

        window.Show();
        window.CaptureRenderedFrame();

        var containers = Enumerable.Range(0, items.Count)
            .Select(i => Assert.IsAssignableFrom<Control>(listBox.ContainerFromIndex(i)))
            .ToList();
        foreach (var container in containers)
        {
            var behaviors = Assert.IsType<BehaviorCollection>(container.GetValue(Interaction.BehaviorsProperty));
            Assert.IsType<ListReorderDragBehavior>(Assert.Single(behaviors)).PlaceholderTemplate = CreatePlaceholderTemplate();
        }

        var height = containers[0].Bounds.Height;
        var start = containers[0].TranslatePoint(new Point(containers[0].Bounds.Width / 2, height / 2), listBox) ?? new Point();

        void MoveBy(double factor)
        {
            window.MouseMove(listBox, new Point(start.X, start.Y + height * factor), RawInputModifiers.LeftMouseButton);
        }

        window.MouseDown(listBox, start, MouseButton.Left);

        // Past the drag threshold, before the middle of the next item: no target yet, the drag goes on.
        MoveBy(0.25);
        Assert.Null(FindPlaceholderTarget(listBox, containers));

        // The placeholder follows the pointer across the targets (and back).
        MoveBy(0.75);
        Assert.Same(containers[1], FindPlaceholderTarget(listBox, containers));

        MoveBy(1.75);
        Assert.Same(containers[2], FindPlaceholderTarget(listBox, containers));

        MoveBy(2.75);
        Assert.Same(containers[3], FindPlaceholderTarget(listBox, containers));

        MoveBy(1.75);
        Assert.Same(containers[2], FindPlaceholderTarget(listBox, containers));

        window.MouseUp(listBox, new Point(start.X, start.Y + height * 1.75), MouseButton.Left);
        window.CaptureRenderedFrame();

        Assert.Null(FindPlaceholderTarget(listBox, containers));
        Assert.Equal(new[] { "Item1", "Item2", "Item0", "Item3", "Item4" }, items.ToArray());
    }

    private const string PlaceholderTag = "ReorderPlaceholder";

#if UNO
    private static DataTemplate CreatePlaceholderTemplate()
    {
        return (DataTemplate)XamlReader.Load(
            $"""
            <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
              <Border Tag="{PlaceholderTag}" />
            </DataTemplate>
            """);
    }

    // The placeholder is shown in a popup placed over its target container (PlaceholderOverlay).
    private static Control? FindPlaceholderTarget(Control listBox, IReadOnlyList<Control> containers)
    {
        var popup = VisualTreeHelper.GetOpenPopupsForXamlRoot(listBox.XamlRoot)
            .SingleOrDefault(p => p.Child is FrameworkElement { Tag: PlaceholderTag });
        if (popup is null)
        {
            return null;
        }

        return containers.Single(c =>
        {
            var origin = c.TransformToVisual(null).TransformPoint(new Point(0, 0));
            return System.Math.Abs(origin.X - popup.HorizontalOffset) < 0.5
                && System.Math.Abs(origin.Y - popup.VerticalOffset) < 0.5;
        });
    }
#else
    private static FuncTemplate<Control> CreatePlaceholderTemplate()
    {
        return new FuncTemplate<Control>(() => new Border { Tag = PlaceholderTag });
    }

    // The placeholder is an adorner of its target container.
    private static Control? FindPlaceholderTarget(Control listBox, IReadOnlyList<Control> containers)
    {
        var placeholder = AdornerLayer.GetAdornerLayer(listBox)?.Children
            .OfType<Control>()
            .SingleOrDefault(c => Equals(c.Tag, PlaceholderTag));
        return placeholder is null ? null : Assert.IsAssignableFrom<Control>(AdornerLayer.GetAdornedElement(placeholder));
    }
#endif
}
