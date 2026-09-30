// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;
using static Xaml.Interactions.Custom.Controls.UnitTests.TestHelpers;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

public class InputTests
{
    private static InputInjector? CreateInjector()
    {
        var injector = InputInjector.TryCreate();
        injector?.InitializeTouchInjection(InjectedInputVisualizationMode.None);
        return injector;
    }

    private static void MoveTo(InputInjector injector, UIElement element, Point point)
    {
        var position = element.TransformToVisual(null).TransformPoint(point);
        injector.InjectMouseInput([new InjectedInputMouseInfo
        {
            DeltaX = (int)position.X,
            DeltaY = (int)position.Y,
            MouseOptions = InjectedInputMouseOptions.Absolute | InjectedInputMouseOptions.Move,
        }]);
    }

    private static void Click(InputInjector injector)
    {
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);
    }

    /// <summary>
    /// Counts the key presses that reach the element (the headless host may not deliver injected keyboard input).
    /// </summary>
    private static System.Func<int> CountKeyDowns(UIElement element)
    {
        var count = 0;
        element.AddHandler(UIElement.PreviewKeyDownEvent, new Microsoft.UI.Xaml.Input.KeyEventHandler((_, _) => count++), true);
        return () => count;
    }

    private static void PressKey(InputInjector injector, VirtualKey key)
    {
        injector.InjectKeyboardInput([new InjectedInputKeyboardInfo { VirtualKey = (ushort)key }]);
        injector.InjectKeyboardInput([new InjectedInputKeyboardInfo { VirtualKey = (ushort)key, KeyOptions = InjectedInputKeyOptions.KeyUp }]);
    }

    [UnoHeadlessFact]
    public async Task CarouselKeyNavigationBehavior_Navigates_With_Arrow_Keys()
    {
        var injector = CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var flipView = new FlipView { Width = 200, Height = 100 };
        flipView.Items.Add("a");
        flipView.Items.Add("b");
        flipView.Items.Add("c");
        new CarouselKeyNavigationBehavior { Orientation = Orientation.Vertical }.AttachTo(flipView);
        await Session.ShowAsync(flipView);
        flipView.SelectedIndex = 0;
        flipView.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        var keyDowns = CountKeyDowns(flipView);

        PressKey(injector!, VirtualKey.Down);
        await Session.WaitForIdleAsync();
        Assert.SkipWhen(keyDowns() == 0, "Injected keyboard input is not delivered by the headless host.");
        Assert.True(await WaitUntilAsync(() => flipView.SelectedIndex == 1));

        PressKey(injector!, VirtualKey.Up);
        Assert.True(await WaitUntilAsync(() => flipView.SelectedIndex == 0));
    }

    [UnoHeadlessFact]
    public async Task TabControlKeyNavigationBehavior_Navigates_With_Arrow_Keys()
    {
        var injector = CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var tabView = new TabView();
        tabView.TabItems.Add(new TabViewItem { Header = "a" });
        tabView.TabItems.Add(new TabViewItem { Header = "b" });
        new TabControlKeyNavigationBehavior { Orientation = Orientation.Horizontal }.AttachTo(tabView);
        await Session.ShowAsync(tabView);
        tabView.SelectedIndex = 0;
        ((TabViewItem)tabView.TabItems[0]).Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        var keyDowns = CountKeyDowns(tabView);

        PressKey(injector!, VirtualKey.Right);
        await Session.WaitForIdleAsync();
        Assert.SkipWhen(keyDowns() == 0, "Injected keyboard input is not delivered by the headless host.");
        Assert.True(await WaitUntilAsync(() => tabView.SelectedIndex == 1));

        PressKey(injector!, VirtualKey.Right);
        await Session.WaitForIdleAsync();
        Assert.Equal(1, tabView.SelectedIndex);

        PressKey(injector!, VirtualKey.Left);
        Assert.True(await WaitUntilAsync(() => tabView.SelectedIndex == 0));
    }

    [UnoHeadlessFact]
    public async Task SelectListBoxItemOnPointerMovedBehavior_Selects_The_Hovered_Item()
    {
        var injector = CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var first = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        var second = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue) };
        new SelectListBoxItemOnPointerMovedBehavior().AttachTo(second);
        var listView = new ListView { Items = { first, second } };
        await Session.ShowAsync(listView);
        await Session.WaitForIdleAsync();

        MoveTo(injector!, second, new Point(10, 10));
        MoveTo(injector!, second, new Point(20, 15));

        Assert.True(await WaitUntilAsync(() => listView.SelectedIndex == 1));
    }

    [UnoHeadlessFact]
    public async Task ToggleIsExpandedOnDoubleTappedBehavior_Toggles_The_Tree_Item()
    {
        var injector = CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var header = new Border { Height = 30, Width = 100, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        new ToggleIsExpandedOnDoubleTappedBehavior().AttachTo(header);
        var item = new TreeViewItem { Content = header };
        await Session.ShowAsync(new StackPanel { Children = { item } });
        await Session.WaitForIdleAsync();

        MoveTo(injector!, header, new Point(10, 10));
        Click(injector!);
        Click(injector!);

        Assert.True(await WaitUntilAsync(() => item.IsExpanded));
    }

    [UnoHeadlessFact]
    public async Task HorizontalScrollViewerBehavior_Scrolls_Horizontally_On_Wheel()
    {
        var injector = CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

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

        MoveTo(injector!, scrollViewer, new Point(50, 50));
        injector!.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.Wheel, MouseData = unchecked((uint)-120) }]);
        await Session.WaitForIdleAsync();
        Assert.SkipWhen(wheels == 0, "Injected mouse wheel input is not delivered by the headless host.");

        // One page (the viewport width) to the right.
        Assert.True(await WaitUntilAsync(() => scrollViewer.HorizontalOffset >= 199));
    }
}
