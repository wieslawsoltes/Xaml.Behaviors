// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Interactivity.UnitTests;

public class RoutedEventCompatTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static SolidColorBrush Brush(Windows.UI.Color color) => new(color);

    [UnoHeadlessFact]
    public async Task Plain_RoutedEventArgs_Handler_Receives_Pointer_Events()
    {
        // Fills the window.
        var border = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        var received = 0;
        EventHandler<RoutedEventArgs> handler = (_, _) => received++;
        border.AddHandler(UIElement.PointerPressedEvent, handler, RoutingStrategies.Bubble);
        await Session.ShowAsync(border);

        Session.Mouse.Click(border, new Point(20, 20));
        await Session.WaitForIdleAsync();

        Assert.Equal(1, received);

        border.RemoveRoutedEventHandler(UIElement.PointerPressedEvent, handler);
        Session.Mouse.Click(border, new Point(20, 20));
        await Session.WaitForIdleAsync();

        Assert.Equal(1, received);
    }

    // Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391: a Direct subscription only
    // receives the events raised on the element itself.

    [UnoHeadlessFact]
    public void Direct_Key_Handler_Ignores_Events_Raised_By_A_Child()
    {
        var textBox = new TextBox();
        var border = new Border { Child = textBox };
        var direct = new List<VirtualKey>();
        var bubble = new List<VirtualKey>();
        EventHandler<KeyRoutedEventArgs> directHandler = (_, e) => direct.Add(e.Key);
        border.AddHandler(UIElement.KeyDownEvent, directHandler, RoutingStrategies.Direct);
        border.AddHandler(UIElement.KeyDownEvent, (EventHandler<KeyRoutedEventArgs>)((_, e) => bubble.Add(e.Key)), RoutingStrategies.Direct | RoutingStrategies.Bubble);
        Session.Show(border);
        textBox.Focus(FocusState.Programmatic);

        Session.Keyboard.Press(VirtualKey.F5);

        Assert.Empty(direct);
        Assert.Equal([VirtualKey.F5], bubble);
        border.RemoveRoutedEventHandler(UIElement.KeyDownEvent, directHandler);
    }

    [UnoHeadlessFact]
    public void Direct_Key_Handler_Receives_Events_Raised_By_The_Element()
    {
        var textBox = new TextBox();
        var direct = new List<VirtualKey>();
        EventHandler<KeyRoutedEventArgs> handler = (_, e) => direct.Add(e.Key);
        textBox.AddHandler(UIElement.KeyDownEvent, handler, RoutingStrategies.Direct);
        Session.Show(new Border { Child = textBox });
        textBox.Focus(FocusState.Programmatic);

        Session.Keyboard.Press(VirtualKey.F5);
        textBox.RemoveRoutedEventHandler(UIElement.KeyDownEvent, handler);
        Session.Keyboard.Press(VirtualKey.F6);

        Assert.Equal([VirtualKey.F5], direct);
    }

    [UnoHeadlessFact]
    public void Direct_Typed_Disposable_Handler_Filters_By_Source()
    {
        var textBox = new TextBox();
        var border = new Border { Child = textBox };
        var received = new List<object>();
        RoutedEvent<KeyRoutedEventArgs> keyDown = UIElement.KeyDownEvent;
        using var subscription = border.AddDisposableHandler(keyDown, (_, e) => received.Add(e.OriginalSource), RoutingStrategies.Direct);
        Session.Show(border);
        textBox.Focus(FocusState.Programmatic);

        Session.Keyboard.Press(VirtualKey.F5);

        Assert.Empty(received);
    }

    [UnoHeadlessFact]
    public void Direct_Focus_Handler_Only_Receives_The_Focus_Of_The_Element()
    {
        var child = new Button { Content = "child" };
        var host = new ContentControl { Content = child, IsTabStop = true };
        var other = new Button { Content = "other" };
        var direct = new List<object>();
        var bubble = new List<object>();
        RoutedEvent<RoutedEventArgs> gotFocus = UIElement.GotFocusEvent;
        using var directSubscription = host.AddDisposableHandler(gotFocus, (_, e) => direct.Add(e.OriginalSource), RoutingStrategies.Direct);
        using var bubbleSubscription = host.AddDisposableHandler(gotFocus, (_, e) => bubble.Add(e.OriginalSource), RoutingStrategies.Bubble);
        Session.Show(new StackPanel { Children = { host, other } });

        child.Focus(FocusState.Programmatic);
        Session.RunJobs();
        other.Focus(FocusState.Programmatic);
        Session.RunJobs();
        host.Focus(FocusState.Programmatic);
        Session.RunJobs();

        Assert.Equal([host], direct);
        Assert.Equal([child, host], bubble);
    }

    [UnoHeadlessFact]
    public void Direct_Pointer_Handler_Filters_By_Source()
    {
        var child = new Border { Width = 40, Height = 40, Background = Brush(Microsoft.UI.Colors.Red) };
        var parent = new Border { Width = 100, Height = 100, Background = Brush(Microsoft.UI.Colors.Green), Child = child };
        var direct = new List<object>();
        parent.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, e) => direct.Add(e.OriginalSource)), RoutingStrategies.Direct);
        Session.Show(new StackPanel { Children = { parent } });

        Session.Mouse.Click(child);
        Assert.Empty(direct);

        Session.Mouse.Click(parent, new Point(5, 5));
        Assert.Equal([parent], direct);
    }

    [UnoHeadlessFact]
    public void Direct_PointerEntered_And_Exited_Are_Raised_Once_Per_Element()
    {
        // The child fills the corner of the parent: the pointer enters both at once, with the child as the source.
        var child = new Border
        {
            Width = 40,
            Height = 40,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Background = Brush(Microsoft.UI.Colors.Red),
        };
        var parent = new Border { Width = 100, Height = 100, Background = Brush(Microsoft.UI.Colors.Green), Child = child };
        var events = new List<string>();
        parent.AddHandler(UIElement.PointerEnteredEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => events.Add("entered")), RoutingStrategies.Direct);
        parent.AddHandler(UIElement.PointerExitedEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => events.Add("exited")), RoutingStrategies.Direct);
        Session.Show(new StackPanel { Children = { parent } });
        Session.Mouse.MoveTo(new Point(500, 500));

        Session.Mouse.MoveTo(new Point(10, 10), parent);
        Session.Mouse.MoveTo(new Point(80, 80), parent);
        Session.Mouse.MoveTo(new Point(10, 10), parent);
        Session.Mouse.MoveTo(new Point(500, 500));

        Assert.Equal(["entered", "exited"], events);
    }

    [UnoHeadlessFact]
    public void Direct_PointerCaptureLost_Is_Delivered_For_The_Captures_Of_The_Element()
    {
        // The button captures the pointer while pressed; the original source of its capture lost event is its text.
        var button = new Button { Content = "button", Width = 80, Height = 40 };
        var host = new Border { Background = Brush(Microsoft.UI.Colors.Green), Padding = new Thickness(10), Child = button };
        var buttonLost = 0;
        var hostLost = 0;
        var hostBubbleLost = 0;
        EventHandler<PointerRoutedEventArgs> buttonHandler = (_, _) => buttonLost++;
        button.AddHandler(UIElement.PointerCaptureLostEvent, buttonHandler, RoutingStrategies.Direct, handledEventsToo: true);
        host.AddHandler(UIElement.PointerCaptureLostEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => hostLost++), RoutingStrategies.Direct, handledEventsToo: true);
        host.AddHandler(UIElement.PointerCaptureLostEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => hostBubbleLost++), RoutingStrategies.Bubble, handledEventsToo: true);
        Session.Show(new StackPanel { Children = { host } });

        Session.Mouse.Click(button);

        Assert.Equal(1, buttonLost);
        Assert.Equal(0, hostLost);
        Assert.Equal(1, hostBubbleLost);

        button.RemoveRoutedEventHandler(UIElement.PointerCaptureLostEvent, buttonHandler);
        Session.Mouse.Click(button);

        Assert.Equal(1, buttonLost);
    }

    // Regression tests for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/395: the emulated tunnel route of
    // the pointer events presents the events the controls handled as not handled yet, and never un-handles them.

    [UnoHeadlessFact]
    public void Emulated_Tunnel_Handler_Sees_A_Handled_Press_As_Not_Handled_And_Keeps_It_Handled()
    {
        var button = new Button { Content = "button", Width = 80, Height = 40 };
        var host = new Border { Child = button };
        var seen = new List<string>();
        button.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, e) =>
        {
            seen.Add($"tunnel:{e.Handled}");
            e.Handled = false;
        }), RoutingStrategies.Tunnel);
        button.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, e) => seen.Add($"tunnel-handled-too:{e.Handled}")), RoutingStrategies.Tunnel, handledEventsToo: true);
        var hostPressed = 0;
        host.PointerPressed += (_, _) => hostPressed++;
        Session.Show(new StackPanel { Children = { host } });

        Session.Mouse.Click(button);

        Assert.Equal(["tunnel:False", "tunnel-handled-too:True"], seen);
        Assert.Equal(0, hostPressed);
    }

    [UnoHeadlessFact]
    public void Emulated_Tunnel_Handler_That_Handles_The_Event_Skips_The_Next_Tunnel_Handlers_Of_The_Element()
    {
        var target = new Border { Width = 80, Height = 40, Background = Brush(Microsoft.UI.Colors.Red) };
        var host = new Border { Child = target };
        var seen = new List<string>();
        target.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, e) =>
        {
            seen.Add("first");
            e.Handled = true;
        }), RoutingStrategies.Tunnel);
        target.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => seen.Add("second")), RoutingStrategies.Tunnel);
        target.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => seen.Add("handled-too")), RoutingStrategies.Tunnel, handledEventsToo: true);
        host.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => seen.Add("host-tunnel")), RoutingStrategies.Tunnel);
        host.AddHandler(UIElement.PointerPressedEvent, (EventHandler<PointerRoutedEventArgs>)((_, _) => seen.Add("host-bubble")), RoutingStrategies.Bubble);
        Session.Show(new StackPanel { Children = { host } });

        Session.Mouse.Click(target);

        Assert.Equal(["first", "handled-too", "host-tunnel"], seen);
    }
}
