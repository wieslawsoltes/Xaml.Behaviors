// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.DragAndDrop;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.DragAndDrop.DataGridTests;

public sealed class Item(string name)
{
    public string Name { get; } = name;

    public override string ToString() => Name;
}

public sealed class ItemsDropHandler(ObservableCollection<Item> items) : BaseDataGridDropHandler<Item>
{
    public List<string> Rows { get; } = [];

    protected override Item MakeCopy(ObservableCollection<Item> parentCollection, Item item) => new(item.Name + "*");

    protected override bool Validate(DataGrid dg, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        // The element under the pointer (the original source of a native WinUI drag event is the drop target, here
        // the data grid).
        if (sourceContext is not Item source || (e.Source as FrameworkElement)?.DataContext is not Item target)
        {
            return false;
        }

        return RunDropAction(dg, e, execute, source, target, items);
    }
}

public class DataGridDropHandlerTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static IEnumerable<DataGridRow> GetRows(DependencyObject root)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is DataGridRow row)
            {
                yield return row;
            }

            foreach (var nested in GetRows(child))
            {
                yield return nested;
            }
        }
    }

    [UnoHeadlessTheory]
    [InlineData(0, 2, 0.75, "DraggingDown", "b,c,a", 2)]
    [InlineData(2, 0, 0.25, "DraggingUp", "c,a,b", 0)]
    public async Task Dropping_A_Row_Moves_The_Item_And_Updates_Row_Classes(int fromIndex, int toIndex, double offset, string rowClass, string expected, int selectedIndex)
    {
        var input = MouseInput.TryCreate();
        Assert.SkipWhen(input is null, "Input injection is not available.");

#if !WINUI
        // The headless session has no generated application initialization: register the toolkit resources and
        // default styles (DataGrid templates) like an app head does (native WinUI loads them from the toolkit itself).
        CommunityToolkit.WinUI.UI.Controls.DG.GlobalStaticResources.Initialize();
        CommunityToolkit.WinUI.UI.Controls.DG.GlobalStaticResources.RegisterResourceDictionariesBySource();
        CommunityToolkit.WinUI.UI.Controls.DG.GlobalStaticResources.RegisterDefaultStyles();
#endif

        var items = new ObservableCollection<Item> { new("a"), new("b"), new("c") };
        var handler = new ItemsDropHandler(items);
        var dataGrid = new DataGrid
        {
            Width = 300,
            Height = 300,
            AutoGenerateColumns = false,
            ItemsSource = items,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding { Path = new PropertyPath("Name") } });
        var dragHandler = new DragHandler();
        dataGrid.LoadingRow += (_, e) =>
        {
            var behaviors = Interaction.GetBehaviors(e.Row);
            if (behaviors.Count == 0)
            {
                behaviors.Add(new ContextDragBehavior { Handler = dragHandler });
            }
        };
        Interaction.GetBehaviors(dataGrid).Add(new ContextDropBehavior { Handler = handler });

        var host = new Grid { Children = { dataGrid } };
        host.Resources.MergedDictionaries.Add(new XamlControlsResources());
        await Session.ShowAsync(host);
        for (var i = 0; i < 20 && GetRows(dataGrid).Count() < items.Count; i++)
        {
            await Session.WaitForIdleAsync();
        }

        var rows = GetRows(dataGrid).OrderBy(r => r.GetIndex()).ToArray();
        Assert.True(rows.Length >= 3, "The data grid rows were not realized.");

        var from = rows[fromIndex];
        var to = rows[toIndex];
        var start = from.TransformToVisual(null).TransformPoint(new Point(from.ActualWidth / 2, from.ActualHeight / 2));
        var end = to.TransformToVisual(null).TransformPoint(new Point(to.ActualWidth / 2, to.ActualHeight * offset));
        await input!.PressAndMoveAsync(start, end, steps: 10);
        for (var i = 0; i < 100 && !to.Classes.Contains(rowClass); i++)
        {
            await Task.Delay(5);
        }

        Assert.True(to.Classes.Contains(rowClass));

        await input.UpAsync();
        for (var i = 0; i < 400 && dragHandler.Completed == 0; i++)
        {
            await Task.Delay(5);
        }

        Assert.Equal(expected, string.Join(",", items.Select(x => x.Name)));
        Assert.Equal(selectedIndex, dataGrid.SelectedIndex);
        Assert.DoesNotContain(GetRows(dataGrid), r => r.Classes.Contains("DraggingDown") || r.Classes.Contains("DraggingUp"));
    }

    private sealed class DragHandler : IDragHandler
    {
        public int Completed { get; private set; }

        public void BeforeDragDrop(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e, object? context)
        {
        }

        public void AfterDragDrop(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e, object? context) => Completed++;
    }
}
