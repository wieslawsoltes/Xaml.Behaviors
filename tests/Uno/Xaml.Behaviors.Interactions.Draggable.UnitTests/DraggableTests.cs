// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Draggable;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Draggable.UnitTests;

public class DraggableTests
{
    private const string ItemTemplateXaml =
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Border Width="100" Height="30" Background="Blue">
            <TextBlock Text="{Binding}" />
          </Border>
        </DataTemplate>
        """;

    // The headless session has no theme resources: give the templated controls a minimal template.
    private const string ItemsControlTemplateXaml =
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" TargetType="ItemsControl">
          <ItemsPresenter />
        </ControlTemplate>
        """;

    private const string ScrollViewerTemplateXaml =
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="ScrollViewer">
          <ScrollContentPresenter x:Name="ScrollContentPresenter" />
        </ControlTemplate>
        """;

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static Border CreateBox(double size = 40)
        => new() { Width = size, Height = size, Background = new SolidColorBrush(Colors.Red), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

    private static MouseInput RequireInput()
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");
        return input!;
    }

    [UnoHeadlessFact]
    public async Task MouseDragElementBehavior_Moves_Element_With_TranslateTransform()
    {
        var input = RequireInput();
        var box = CreateBox();
        Interaction.GetBehaviors(box).Add(new MouseDragElementBehavior());
        await Session.ShowAsync(new Grid { Width = 300, Height = 300, Children = { box } });

        var start = MouseInput.Center(box);
        await input.DragAsync(start, new Point(start.X + 40, start.Y + 20));

        var transform = Assert.IsType<TranslateTransform>(box.RenderTransform);
        Assert.Equal(40, transform.X, 0.5);
        Assert.Equal(20, transform.Y, 0.5);
    }

    [UnoHeadlessFact]
    public async Task MouseDragElementBehavior_Keeps_Moving_Outside_The_Element_While_Captured()
    {
        var input = RequireInput();
        var box = CreateBox(20);
        Interaction.GetBehaviors(box).Add(new MouseDragElementBehavior());
        await Session.ShowAsync(new Grid { Width = 300, Height = 300, Children = { box } });

        var start = MouseInput.Center(box);
        // One large jump: the pointer leaves the element before the element follows it.
        await input.DragAsync(start, new Point(start.X + 150, start.Y + 100), steps: 1);

        var transform = Assert.IsType<TranslateTransform>(box.RenderTransform);
        Assert.Equal(150, transform.X, 0.5);
        Assert.Equal(100, transform.Y, 0.5);
    }

    [UnoHeadlessFact]
    public async Task MouseDragElementBehavior_Constrains_To_Parent_Bounds()
    {
        var input = RequireInput();
        var box = CreateBox();
        Interaction.GetBehaviors(box).Add(new MouseDragElementBehavior { ConstrainToParentBounds = true });
        await Session.ShowAsync(new Grid { Width = 100, Height = 100, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Children = { box } });

        var start = MouseInput.Center(box);
        await input.DragAsync(start, new Point(start.X + 200, start.Y + 200));

        var transform = Assert.IsType<TranslateTransform>(box.RenderTransform);
        Assert.Equal(60, transform.X, 0.5);
        Assert.Equal(60, transform.Y, 0.5);
    }

    [UnoHeadlessFact]
    public async Task MouseDragElementBehavior_Disabled_Does_Not_Move()
    {
        var input = RequireInput();
        var box = CreateBox();
        Interaction.GetBehaviors(box).Add(new MouseDragElementBehavior { IsEnabled = false });
        await Session.ShowAsync(new Grid { Width = 300, Height = 300, Children = { box } });

        var start = MouseInput.Center(box);
        await input.DragAsync(start, new Point(start.X + 40, start.Y + 20));

#if WINUI
        // Native WinUI returns an identity MatrixTransform when no render transform is set (Uno Platform returns null).
        Assert.Equal(DependencyProperty.UnsetValue, box.ReadLocalValue(UIElement.RenderTransformProperty));
#else
        Assert.Null(box.RenderTransform);
#endif
    }

    [UnoHeadlessFact]
    public async Task MultiMouseDragElementBehavior_Moves_All_Targets()
    {
        var input = RequireInput();
        var box = CreateBox();
        var other = CreateBox();
        other.Margin = new Thickness(100, 0, 0, 0);
        var behavior = new MultiMouseDragElementBehavior();
        behavior.TargetControls.Add(other);
        Interaction.GetBehaviors(box).Add(behavior);
        await Session.ShowAsync(new Grid { Width = 300, Height = 300, Children = { box, other } });

        var start = MouseInput.Center(box);
        await input.DragAsync(start, new Point(start.X + 30, start.Y + 10));

        var boxTransform = Assert.IsType<TranslateTransform>(box.RenderTransform);
        var otherTransform = Assert.IsType<TranslateTransform>(other.RenderTransform);
        Assert.Equal(30, boxTransform.X, 0.5);
        Assert.Equal(30, otherTransform.X, 0.5);
        Assert.Equal(10, otherTransform.Y, 0.5);
    }

    [UnoHeadlessFact]
    public async Task CanvasDragBehavior_Updates_Canvas_Position_And_Dragging_Class()
    {
        var input = RequireInput();
        var box = CreateBox();
        Canvas.SetLeft(box, 10);
        Canvas.SetTop(box, 10);
        Interaction.GetBehaviors(box).Add(new CanvasDragBehavior());
        await Session.ShowAsync(new Canvas { Width = 300, Height = 300, Children = { box } });

        var start = MouseInput.Center(box);
        await input.PressAndMoveAsync(start, new Point(start.X + 50, start.Y + 30));
        Assert.True(box.Classes.Contains(":dragging"));
        await input.UpAsync();

        Assert.Equal(60, Canvas.GetLeft(box), 0.5);
        Assert.Equal(40, Canvas.GetTop(box), 0.5);
        Assert.False(box.Classes.Contains(":dragging"));
    }

    [UnoHeadlessFact]
    public async Task GridDragBehavior_Swaps_Cells()
    {
        var input = RequireInput();
        var first = new Border { Background = new SolidColorBrush(Colors.Red) };
        var second = new Border { Background = new SolidColorBrush(Colors.Blue) };
        Grid.SetColumn(second, 1);
        var grid = new Grid
        {
            Width = 200,
            Height = 100,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
            Children = { first, second },
        };
        Interaction.GetBehaviors(first).Add(new GridDragBehavior());
        await Session.ShowAsync(grid);

        var start = MouseInput.Center(first);
        await input.DragAsync(start, MouseInput.Center(second));

        Assert.Equal(1, Grid.GetColumn(first));
        Assert.Equal(0, Grid.GetColumn(second));
    }

    [UnoHeadlessFact]
    public void ItemDragBehavior_Defaults_Match_Avalonia()
    {
        var behavior = new ItemDragBehavior();

        Assert.Equal(Orientation.Horizontal, behavior.Orientation);
        Assert.Equal(3, behavior.HorizontalDragThreshold);
        Assert.Equal(3, behavior.VerticalDragThreshold);
    }

    private static async Task<(ItemsControl ItemsControl, ObservableCollection<string> Items)> ShowItemsAsync(System.Func<ItemDragBehavior> createBehavior)
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var itemsControl = new ItemsControl
        {
            ItemsSource = items,
            ItemTemplate = (DataTemplate)XamlReader.Load(ItemTemplateXaml),
            Template = (ControlTemplate)XamlReader.Load(ItemsControlTemplateXaml),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        await Session.ShowAsync(new Grid { Children = { itemsControl } });
        for (var attempt = 0; attempt < 20 && itemsControl.ContainerFromIndex(items.Count - 1) is null; attempt++)
        {
            await Session.WaitForIdleAsync();
        }

        Assert.True(itemsControl.ContainerFromIndex(0) is not null, $"No container: items={itemsControl.Items.Count}, panel={itemsControl.ItemsPanelRoot?.GetType().Name}, children={itemsControl.ItemsPanelRoot?.Children.Count}, actual={itemsControl.ActualHeight}");

        for (var i = 0; i < items.Count; i++)
        {
            var container = Assert.IsAssignableFrom<FrameworkElement>(itemsControl.ContainerFromIndex(i));
            Interaction.GetBehaviors(container).Add(createBehavior());
        }

        return (itemsControl, items);
    }

    [UnoHeadlessFact]
    public async Task ItemDragBehavior_Reorders_Items()
    {
        var input = RequireInput();
        var (itemsControl, items) = await ShowItemsAsync(() => new ItemDragBehavior { Orientation = Orientation.Vertical });

        var first = (FrameworkElement)itemsControl.ContainerFromIndex(0);
        var start = MouseInput.Center(first);
        await input.PressAndMoveAsync(start, new Point(start.X, start.Y + 70));
        Assert.True(first.Classes.Contains(":dragging"));
        Assert.IsType<TranslateTransform>(first.RenderTransform);
        await input.UpAsync();

        Assert.Equal(["b", "c", "a"], items.ToArray());
    }

    [UnoHeadlessFact]
    public async Task ItemDragBehavior_Below_Threshold_Does_Not_Reorder()
    {
        var input = RequireInput();
        var (itemsControl, items) = await ShowItemsAsync(() => new ItemDragBehavior { Orientation = Orientation.Vertical, VerticalDragThreshold = 50 });

        var start = MouseInput.Center((FrameworkElement)itemsControl.ContainerFromIndex(0));
        await input.DragAsync(start, new Point(start.X, start.Y + 40));

        Assert.Equal(["a", "b", "c"], items.ToArray());
    }

    [UnoHeadlessFact]
    public async Task ListReorderDragBehavior_Shows_Placeholder_While_Dragging()
    {
        var input = RequireInput();
        var template = (DataTemplate)XamlReader.Load(
            """
            <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
              <Border Background="Green" />
            </DataTemplate>
            """);
        var (itemsControl, _) = await ShowItemsAsync(() => new ListReorderDragBehavior { Orientation = Orientation.Vertical, PlaceholderTemplate = template });

        var first = (FrameworkElement)itemsControl.ContainerFromIndex(0);
        var start = MouseInput.Center(first);
        // Moves before and past the middle of the next item: a move without a target only hides the placeholder.
        await input.PressAndMoveAsync(start, new Point(start.X, start.Y + 40), steps: 4);

        var popups = VisualTreeHelper.GetOpenPopupsForXamlRoot(first.XamlRoot);
        Assert.Contains(popups, p => p.Child is Border { Background: SolidColorBrush });

        await input.UpAsync();

        Assert.DoesNotContain(VisualTreeHelper.GetOpenPopupsForXamlRoot(first.XamlRoot), p => p.Child is Border);
    }

    [UnoHeadlessFact]
    public async Task AutoScrollDuringDragBehavior_Scrolls_Near_Edges()
    {
        var input = RequireInput();
        var content = new Border { Width = 100, Height = 1000, Background = new SolidColorBrush(Colors.Red) };
        var scrollViewer = new ScrollViewer
        {
            Width = 100,
            Height = 100,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Content = content,
            Template = (ControlTemplate)XamlReader.Load(ScrollViewerTemplateXaml),
        };
        Interaction.GetBehaviors(scrollViewer).Add(new AutoScrollDuringDragBehavior { EdgeDistance = 20, ScrollDelta = 10 });
        await Session.ShowAsync(new Grid { Children = { scrollViewer } });

        var origin = scrollViewer.TransformToVisual(null).TransformPoint(new Point(0, 0));
        await input.PressAndMoveAsync(new Point(origin.X + 50, origin.Y + 50), new Point(origin.X + 50, origin.Y + 95), steps: 2);
        await input.MoveAsync(new Point(origin.X + 50, origin.Y + 96));
        await input.UpAsync();
        await Session.WaitForIdleAsync();

        Assert.True(scrollViewer.VerticalOffset > 0, $"VerticalOffset = {scrollViewer.VerticalOffset}");
    }

    [UnoHeadlessFact]
    public async Task Detached_Behavior_Stops_Dragging()
    {
        var input = RequireInput();
        var box = CreateBox();
        var behavior = new MouseDragElementBehavior();
        Interaction.GetBehaviors(box).Add(behavior);
        await Session.ShowAsync(new Grid { Width = 300, Height = 300, Children = { box } });
        Interaction.GetBehaviors(box).Remove(behavior);

        var start = MouseInput.Center(box);
        await input.DragAsync(start, new Point(start.X + 40, start.Y + 20));

#if WINUI
        // Native WinUI returns an identity MatrixTransform when no render transform is set (Uno Platform returns null).
        Assert.Equal(DependencyProperty.UnsetValue, box.ReadLocalValue(UIElement.RenderTransformProperty));
#else
        Assert.Null(box.RenderTransform);
#endif
    }
}
