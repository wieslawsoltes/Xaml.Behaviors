// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;
using static Xaml.Interactions.Custom.Controls.UnitTests.TestHelpers;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

public class ItemsTests
{
    [UnoHeadlessFact]
    public void ItemsControl_Actions_Edit_The_Items_Source()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var itemsControl = new ItemsControl { ItemsSource = items };

        Assert.Equal(true, new AddItemToItemsControlAction { ItemsControl = itemsControl, Item = "d" }.Execute(null, null));
        Assert.Equal(true, new InsertItemToItemsControlAction { ItemsControl = itemsControl, Item = "z", Index = 0 }.Execute(null, null));
        Assert.Equal(["z", "a", "b", "c", "d"], items);

        Assert.Equal(true, new MoveItemInItemsControlAction { ItemsControl = itemsControl, FromIndex = 0, ToIndex = -1 }.Execute(null, null));
        Assert.Equal(["a", "b", "c", "d", "z"], items);

        Assert.Equal(true, new RemoveItemAtAction { ItemsControl = itemsControl, Index = 1 }.Execute(null, null));
        Assert.Equal(["a", "c", "d", "z"], items);

        Assert.Equal(true, new ClearItemsControlAction { ItemsControl = itemsControl }.Execute(null, null));
        Assert.Empty(items);
    }

    [UnoHeadlessFact]
    public void AddItemToItemsControlAction_Builds_Data_Templates()
    {
        var items = new ObservableCollection<object>();
        var itemsControl = new ItemsControl { ItemsSource = items };
        var template = (DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock Text=\"t\" /></DataTemplate>");

        new AddItemToItemsControlAction { ItemsControl = itemsControl, Item = template }.Execute(null, null);

        Assert.IsType<TextBlock>(Assert.Single(items));
    }

    [UnoHeadlessFact]
    public void Item_Actions_Create_A_New_Item_Per_Execution_From_The_Item_Factory()
    {
        var items = new ObservableCollection<object>();
        var itemsControl = new ItemsControl { ItemsSource = items };
        var addFactory = new CountingItemFactory("added");
        var insertFactory = new CountingItemFactory("inserted");
        var add = new AddItemToItemsControlAction { ItemsControl = itemsControl, Item = "ignored", ItemFactory = addFactory };
        var insert = new InsertItemToItemsControlAction { ItemsControl = itemsControl, Index = 0, ItemFactory = insertFactory };

        Assert.Equal(true, add.Execute(null, null));
        Assert.Equal(true, add.Execute(null, null));
        Assert.Equal(true, insert.Execute(null, null));

        Assert.Equal(["inserted 1", "added 1", "added 2"], items);
        Assert.Equal(2, addFactory.Count);
        Assert.Equal(1, insertFactory.Count);
    }

    [UnoHeadlessFact]
    public void Item_Actions_Add_Nothing_When_The_Item_Factory_Returns_Null()
    {
        var items = new ObservableCollection<object>();
        var itemsControl = new ItemsControl { ItemsSource = items };
        var factory = new CountingItemFactory(null);

        Assert.Equal(false, new AddItemToItemsControlAction { ItemsControl = itemsControl, ItemFactory = factory }.Execute(null, null));
        Assert.Equal(false, new InsertItemToItemsControlAction { ItemsControl = itemsControl, ItemFactory = factory }.Execute(null, null));
        Assert.Empty(items);

        // Without a factory the actions use Item.
        Assert.Equal(true, new AddItemToItemsControlAction { ItemsControl = itemsControl, Item = "item" }.Execute(null, null));
        Assert.Equal(["item"], items);
    }

    private sealed class CountingItemFactory(string? prefix) : IItemFactory
    {
        public int Count { get; private set; }

        public object? CreateItem()
        {
            Count++;
            return prefix is null ? null : $"{prefix} {Count}";
        }
    }

    [UnoHeadlessFact]
    public async Task RemoveItemInItemsControlAction_Removes_The_Data_Context_Of_An_Item()
    {
        var items = new ObservableCollection<string> { "a", "b" };
        var itemsControl = new ItemsControl { ItemsSource = items };
        await Session.ShowAsync(itemsControl);

        var container = (FrameworkElement)itemsControl.ContainerFromIndex(1);
        var element = container.GetVisualDescendantsOrSelf().OfType<FrameworkElement>().Last();

        Assert.Equal(true, new RemoveItemInItemsControlAction().Execute(element, null));
        Assert.Equal(["a"], items);
    }

    [UnoHeadlessFact]
    public async Task RemoveItemInListBoxAction_Removes_Directly_Added_Items()
    {
        var listView = new ListView();
        listView.Items.Add("a");
        listView.Items.Add("b");
        await Session.ShowAsync(listView);

        var container = (FrameworkElement)listView.ContainerFromIndex(0);
        var element = container.GetVisualDescendantsOrSelf().OfType<FrameworkElement>().Last();

        Assert.Equal(true, new RemoveItemInListBoxAction().Execute(element, null));
        Assert.Equal(["b"], listView.Items.Cast<string>());
    }

    [UnoHeadlessFact]
    public async Task ItemsControl_Container_Triggers_Follow_The_ItemsRepeater()
    {
        var items = new ObservableCollection<string> { "a", "b" };
        var repeater = new ItemsRepeater { ItemsSource = items };
        var prepared = new ItemsControlContainerPreparedTrigger().AttachTo(repeater).Record();
        var clearing = new ItemsControlContainerClearingTrigger().AttachTo(repeater).Record();
        var indexChanged = new ItemsControlContainerIndexChangedTrigger().AttachTo(repeater).Record();
        await Session.ShowAsync(new ScrollViewer { Content = repeater });
        await Session.WaitForIdleAsync();

        // WinUI raises Loaded (the Uno visual tree attachment) after the first layout pass: the initial elements are
        // prepared before the triggers subscribe.
        items.Add("c");
        Assert.True(await WaitUntilAsync(() => prepared.Parameters.Count >= 1));
        Assert.All(prepared.Parameters, p => Assert.IsType<ItemsRepeaterElementPreparedEventArgs>(p));

        items.Insert(0, "z");
        items.RemoveAt(items.Count - 1);
        Assert.True(await WaitUntilAsync(() => clearing.Parameters.Count > 0 && indexChanged.Parameters.Count > 0));
        Assert.IsType<ItemsRepeaterElementClearingEventArgs>(clearing.Parameters[0]);
        Assert.IsType<ItemsRepeaterElementIndexChangedEventArgs>(indexChanged.Parameters[0]);
    }

    [UnoHeadlessFact]
    public void ItemsControlContainerEventsBehavior_Is_An_ItemsRepeater_Behavior()
    {
        var behavior = new RecordingContainerEventsBehavior();
        var repeater = new ItemsRepeater();
        behavior.AttachTo(repeater);

        Assert.Same(repeater, behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public async Task ItemNudgeDropBehavior_Attaches_To_Items_Controls()
    {
        var itemsControl = new ItemsControl { ItemsSource = new[] { "a", "b" } };
        var behavior = new ItemNudgeDropBehavior { Orientation = Orientation.Horizontal }.AttachTo(itemsControl);
        await Session.ShowAsync(itemsControl);

        Assert.Same(itemsControl, behavior.AssociatedObject);
        Xaml.Interactivity.Interaction.GetBehaviors(itemsControl).Remove(behavior);
        Assert.Null(behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public void Carousel_Actions_Navigate_The_FlipView()
    {
        var flipView = new FlipView();
        flipView.Items.Add("a");
        flipView.Items.Add("b");
        flipView.SelectedIndex = 0;

        new CarouselNextAction { Carousel = flipView }.Execute(null, null);
        Assert.Equal(1, flipView.SelectedIndex);

        new CarouselNextAction().Execute(flipView, null);
        Assert.Equal(1, flipView.SelectedIndex);

        new CarouselPreviousAction { Carousel = flipView }.Execute(null, null);
        Assert.Equal(0, flipView.SelectedIndex);
    }

    [UnoHeadlessFact]
    public void TabControl_Actions_Navigate_The_TabView()
    {
        var tabView = new TabView();
        tabView.TabItems.Add(new TabViewItem { Header = "a" });
        tabView.TabItems.Add(new TabViewItem { Header = "b" });
        tabView.SelectedIndex = 0;

        new TabControlNextAction { TabControl = tabView }.Execute(null, null);
        Assert.Equal(1, tabView.SelectedIndex);

        new TabControlNextAction { TabControl = tabView }.Execute(null, null);
        Assert.Equal(1, tabView.SelectedIndex);

        new TabControlPreviousAction().Execute(tabView, null);
        Assert.Equal(0, tabView.SelectedIndex);
    }

    [UnoHeadlessFact]
    public async Task SelectingItemsControlSearchBehavior_Filters_And_Sorts_Tabs()
    {
        var searchBox = new TextBox();
        var noMatches = new TextBlock();
        var tabView = new TabView();
        tabView.TabItems.Add(new TabViewItem { Header = "Beta" });
        tabView.TabItems.Add(new TabViewItem { Header = "Alpha" });
        tabView.TabItems.Add(new TabViewItem { Header = "Gamma" });
        new SelectingItemsControlSearchBehavior { SearchBox = searchBox, NoMatchesControl = noMatches, EnableSorting = true }.AttachTo(tabView);
        await Session.ShowAsync(new StackPanel { Children = { searchBox, noMatches, tabView } });

        Assert.Equal(["Alpha", "Beta", "Gamma"], tabView.TabItems.Cast<TabViewItem>().Select(t => (string)t.Header));

        searchBox.Text = "ta";
        Assert.True(await WaitUntilAsync(() => tabView.TabItems.Cast<TabViewItem>().Count(t => t.Visibility == Visibility.Visible) == 1));
        Assert.Equal("Beta", (string)((TabViewItem)tabView.SelectedItem).Header);
        Assert.Equal(Visibility.Collapsed, noMatches.Visibility);

        searchBox.Text = "none";
        Assert.True(await WaitUntilAsync(() => noMatches.Visibility == Visibility.Visible));
    }

    [UnoHeadlessFact]
    public void SelectingItemsControlEventsBehavior_Receives_Selector_Selection_Changes()
    {
        var listView = new ListView();
        listView.Items.Add("a");
        listView.Items.Add("b");
        var behavior = new RecordingSelectionBehavior().AttachTo(listView);

        listView.SelectedIndex = 1;

        Assert.Single(behavior.Changes);
    }

    [UnoHeadlessFact]
    public async Task TreeView_Filter_Collapses_Non_Matching_Nodes()
    {
        var treeView = new TreeView();
        var fruits = new TreeViewNode { Content = "Fruits" };
        fruits.Children.Add(new TreeViewNode { Content = "Apple" });
        fruits.Children.Add(new TreeViewNode { Content = "Banana" });
        var vegetables = new TreeViewNode { Content = "Vegetables" };
        treeView.RootNodes.Add(fruits);
        treeView.RootNodes.Add(vegetables);
        await Session.ShowAsync(treeView);

        Assert.Equal(true, new ApplyTreeViewFilterAction { TreeView = treeView, Query = "apple" }.Execute(null, null));

        Assert.True(fruits.IsExpanded);
        Assert.False(vegetables.IsExpanded);
        Assert.True(await WaitUntilAsync(() => treeView.ContainerFromNode(vegetables) is TreeViewItem { Visibility: Visibility.Collapsed }));
        Assert.Equal(Visibility.Visible, ((TreeViewItem)treeView.ContainerFromNode(fruits)).Visibility);

        new ApplyTreeViewFilterAction { TreeView = treeView, Query = string.Empty }.Execute(null, null);
        Assert.Equal(Visibility.Visible, ((TreeViewItem)treeView.ContainerFromNode(vegetables)).Visibility);
        Assert.False(fruits.IsExpanded);
    }

    [UnoHeadlessFact]
    public async Task TreeViewFilterBehavior_Filters_On_Text_Changes()
    {
        var searchBox = new TextBox();
        var noMatches = new TextBlock { Visibility = Visibility.Collapsed };
        var treeView = new TreeView();
        treeView.RootNodes.Add(new TreeViewNode { Content = "One" });
        new TreeViewFilterBehavior { SearchBox = searchBox, NoMatchesControl = noMatches }.AttachTo(treeView);
        var trigger = new TreeViewFilterTextChangedTrigger { SearchBox = searchBox };
        var action = trigger.Record();
        trigger.AttachTo(treeView);
        await Session.ShowAsync(new StackPanel { Children = { searchBox, noMatches, treeView } });

        searchBox.Text = "zzz";

        Assert.True(await WaitUntilAsync(() => noMatches.Visibility == Visibility.Visible));
        Assert.NotEmpty(action.Parameters);
    }

    private sealed class RecordingSelectionBehavior : SelectingItemsControlEventsBehavior
    {
        public System.Collections.Generic.List<SelectionChangedEventArgs> Changes { get; } = [];

        protected override void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Changes.Add(e);
    }

    private sealed class RecordingContainerEventsBehavior : ItemsControlContainerEventsBehavior
    {
    }
}

internal static class VisualTreeTestExtensions
{
    public static System.Collections.Generic.IEnumerable<DependencyObject> GetVisualDescendantsOrSelf(this DependencyObject element)
    {
        yield return element;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            foreach (var descendant in VisualTreeHelper.GetChild(element, i).GetVisualDescendantsOrSelf())
            {
                yield return descendant;
            }
        }
    }
}
