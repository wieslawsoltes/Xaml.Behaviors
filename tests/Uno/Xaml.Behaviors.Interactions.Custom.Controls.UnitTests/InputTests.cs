// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;
using static Xaml.Interactions.Custom.Controls.UnitTests.TestHelpers;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

public class InputTests
{
    [UnoHeadlessFact]
    public async Task CarouselKeyNavigationBehavior_Navigates_With_Arrow_Keys()
    {
        var flipView = new FlipView { Width = 200, Height = 100 };
        flipView.Items.Add("a");
        flipView.Items.Add("b");
        flipView.Items.Add("c");
        new CarouselKeyNavigationBehavior { Orientation = Orientation.Vertical }.AttachTo(flipView);
        await Session.ShowAsync(flipView);
        flipView.SelectedIndex = 0;
        flipView.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Session.Keyboard.Press(VirtualKey.Down);
        Assert.True(await WaitUntilAsync(() => flipView.SelectedIndex == 1));

        Session.Keyboard.Press(VirtualKey.Up);
        Assert.True(await WaitUntilAsync(() => flipView.SelectedIndex == 0));
    }

    [UnoHeadlessFact]
    public async Task TabControlKeyNavigationBehavior_Navigates_With_Arrow_Keys()
    {
        var tabView = new TabView();
        tabView.TabItems.Add(new TabViewItem { Header = "a" });
        tabView.TabItems.Add(new TabViewItem { Header = "b" });
        new TabControlKeyNavigationBehavior { Orientation = Orientation.Horizontal }.AttachTo(tabView);
        await Session.ShowAsync(tabView);
        tabView.SelectedIndex = 0;
        ((TabViewItem)tabView.TabItems[0]).Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Session.Keyboard.Press(VirtualKey.Right);
        Assert.True(await WaitUntilAsync(() => tabView.SelectedIndex == 1));

        Session.Keyboard.Press(VirtualKey.Right);
        await Session.WaitForIdleAsync();
        Assert.Equal(1, tabView.SelectedIndex);

        Session.Keyboard.Press(VirtualKey.Left);
        Assert.True(await WaitUntilAsync(() => tabView.SelectedIndex == 0));
    }

    [UnoHeadlessFact]
    public async Task SelectListBoxItemOnPointerMovedBehavior_Selects_The_Hovered_Item()
    {
        var first = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        var second = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue) };
        new SelectListBoxItemOnPointerMovedBehavior().AttachTo(second);
        var listView = new ListView { Items = { first, second } };
        await Session.ShowAsync(listView);
        await Session.WaitForIdleAsync();

        Session.Mouse.MoveTo(new Point(10, 10), second);
        Session.Mouse.MoveTo(new Point(20, 15), second);

        Assert.True(await WaitUntilAsync(() => listView.SelectedIndex == 1));
    }

    [UnoHeadlessFact]
    public async Task ToggleIsExpandedOnDoubleTappedBehavior_Toggles_The_Tree_Item()
    {
        var header = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        new ToggleIsExpandedOnDoubleTappedBehavior().AttachTo(header);
        var item = new TreeViewItem { Content = header };
        await Session.ShowAsync(new StackPanel { Children = { item } });
        await Session.WaitForIdleAsync();

        Session.Mouse.Click(header, new Point(10, 10));
        Session.Mouse.Click(header, new Point(10, 10));

        Assert.True(await WaitUntilAsync(() => item.IsExpanded));
    }

    [UnoHeadlessFact]
    public async Task HorizontalScrollViewerBehavior_Scrolls_Horizontally_On_Wheel()
    {
        var scrollViewer = new ScrollViewer
        {
            Width = 200,
            Height = 100,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollMode = ScrollMode.Enabled,
            VerticalScrollMode = ScrollMode.Disabled,
            Content = new Border { Width = 2000, Height = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) },
        };
        new HorizontalScrollViewerBehavior { ScrollChangeSize = HorizontalScrollViewerBehavior.ChangeSize.Page }.AttachTo(scrollViewer);
        await Session.ShowAsync(scrollViewer);
        await Session.WaitForIdleAsync();

        var wheels = 0;
        scrollViewer.AddHandler(UIElement.PointerWheelChangedEvent, new Microsoft.UI.Xaml.Input.PointerEventHandler((_, _) => wheels++), true);

        Session.Mouse.MoveTo(new Point(50, 50), scrollViewer);
        Session.Mouse.Wheel(-1);
        await Session.WaitForIdleAsync();
        Assert.Equal(1, wheels);

        // One page (the viewport width) to the right.
        Assert.True(await WaitUntilAsync(() => scrollViewer.HorizontalOffset >= 199));
    }
}
