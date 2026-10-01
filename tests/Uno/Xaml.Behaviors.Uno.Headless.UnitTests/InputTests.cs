// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

public class InputTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void Keyboard_Raises_Preview_Key_And_Character_Events_On_Focused_Element()
    {
        var events = new List<string>();
        var button = new Button { Content = "b" };
        var panel = new StackPanel { Children = { button } };
        panel.PreviewKeyDown += (_, e) => events.Add($"preview:{e.Key}");
        button.KeyDown += (_, e) => events.Add($"down:{e.Key}");
        button.KeyUp += (_, e) => events.Add($"up:{e.Key}");
        button.CharacterReceived += (_, e) => events.Add($"char:{e.Character}");
        Session.Show(panel);
        button.Focus(FocusState.Programmatic);

        Session.Keyboard.Press(VirtualKey.A, character: 'a');

        Assert.Equal(["preview:A", "down:A", "char:a", "up:A"], events);
    }

    [UnoHeadlessFact]
    public void Keyboard_Types_Text_Into_TextBox()
    {
        var textBox = new TextBox();
        Session.Show(new StackPanel { Children = { textBox } });
        textBox.Focus(FocusState.Programmatic);

        Session.Keyboard.TypeText("abc");

        Assert.Equal("abc", textBox.Text);
    }

    [UnoHeadlessFact]
    public void Mouse_Moves_To_Absolute_Positions_And_Clicks()
    {
        var pressed = new List<Point>();
        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        var canvas = new Canvas { Children = { target } };
        Canvas.SetLeft(target, 100);
        Canvas.SetTop(target, 60);
        target.PointerPressed += (_, e) => pressed.Add(e.GetCurrentPoint(target).Position);
        Session.Show(canvas);

        Session.Mouse.MoveTo(new Point(300, 300));
        Session.Mouse.Click(target, new Point(10, 20));
        Session.Mouse.Click(target, new Point(40, 5));

        Assert.Equal([new Point(10, 20), new Point(40, 5)], pressed);
    }

    [UnoHeadlessFact]
    public void Mouse_Wheel_Raises_PointerWheelChanged_With_The_Wheel_Delta()
    {
        var wheels = new List<(int Delta, bool IsHorizontal)>();
        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        target.PointerWheelChanged += (_, e) =>
        {
            var properties = e.GetCurrentPoint(target).Properties;
            wheels.Add((properties.MouseWheelDelta, properties.IsHorizontalMouseWheel));
        };
        Session.Show(new StackPanel { Children = { target } });

        Session.Mouse.MoveTo(new Point(25, 25), target);
        Session.Mouse.Wheel(1);
        Session.Mouse.Wheel(-2);
        Session.Mouse.HorizontalWheel(1);

        Assert.Equal([(120, false), (-240, false), (120, true)], wheels);
    }

    [UnoHeadlessFact]
    public void Mouse_Events_Carry_The_Given_Modifiers()
    {
        var modifiers = new List<VirtualKeyModifiers>();
        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        target.PointerPressed += (_, e) => modifiers.Add(e.KeyModifiers);
        target.PointerReleased += (_, e) => modifiers.Add(e.KeyModifiers);
        target.PointerWheelChanged += (_, e) => modifiers.Add(e.KeyModifiers);
        Session.Show(new StackPanel { Children = { target } });

        Session.Mouse.Click(target, modifiers: VirtualKeyModifiers.Shift);
        Session.Mouse.Wheel(1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Menu);
        Session.Mouse.Click(target);

        Assert.Equal(
            [
                VirtualKeyModifiers.Shift,
                VirtualKeyModifiers.Shift,
                VirtualKeyModifiers.Control | VirtualKeyModifiers.Menu,
                VirtualKeyModifiers.None,
                VirtualKeyModifiers.None,
            ],
            modifiers);
    }

    [UnoHeadlessFact]
    public void Mouse_Events_Carry_The_Modifiers_Held_On_The_Keyboard()
    {
        var modifiers = new List<VirtualKeyModifiers>();
        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        target.PointerPressed += (_, e) => modifiers.Add(e.KeyModifiers);
        target.PointerWheelChanged += (_, e) => modifiers.Add(e.KeyModifiers);
        Session.Show(new StackPanel { Children = { target } });

        Session.Keyboard.KeyDown(VirtualKey.Control);
        Session.Keyboard.KeyDown(VirtualKey.RightShift);
        Assert.Equal(VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, Session.Keyboard.Modifiers);
        Session.Mouse.Click(target);
        Session.Mouse.MoveTo(new Point(10, 10), target);
        Session.Mouse.Wheel(1, VirtualKeyModifiers.Menu);
        Session.Keyboard.KeyUp(VirtualKey.RightShift);
        Session.Keyboard.KeyUp(VirtualKey.Control);
        Session.Mouse.Wait(TimeSpan.FromSeconds(1));
        Session.Mouse.Click(target);

        Assert.Equal(VirtualKeyModifiers.None, Session.Keyboard.Modifiers);
        Assert.Equal(
            [
                VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
                VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift | VirtualKeyModifiers.Menu,
                VirtualKeyModifiers.None,
            ],
            modifiers);
    }

    [UnoHeadlessFact]
    public void Mouse_Wait_Separates_Clicks_Into_Single_Taps()
    {
        var doubleTaps = 0;
        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        target.DoubleTapped += (_, _) => doubleTaps++;
        Session.Show(new StackPanel { Children = { target } });
        Session.Mouse.Wait(TimeSpan.FromSeconds(10));

        Session.Mouse.Click(target);
        Session.Mouse.Wait(TimeSpan.FromSeconds(10));
        Session.Mouse.Click(target);

        Assert.Equal(0, doubleTaps);

        Session.Mouse.Click(target);

        Assert.Equal(1, doubleTaps);
    }
}
