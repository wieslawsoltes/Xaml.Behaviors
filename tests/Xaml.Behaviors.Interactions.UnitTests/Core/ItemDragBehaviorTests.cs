using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class ItemDragBehaviorTests
{
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
}
