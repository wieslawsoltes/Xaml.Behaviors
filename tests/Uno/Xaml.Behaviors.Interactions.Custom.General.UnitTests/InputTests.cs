// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

public class InputTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(Button Button, TextBox Other)> ShowFocusablesAsync()
    {
        var button = new Button { Content = "b" };
        var other = new TextBox();
        await Session.ShowAsync(new StackPanel { Children = { button, other } });
        return (button, other);
    }

    // ---------------------------------------------------------------- KeyGesture

    [UnoHeadlessFact]
    public void KeyGesture_Parses_Modifiers_And_Keys()
    {
        var gesture = KeyGesture.Parse("Ctrl+Shift+S");
        Assert.Equal(VirtualKey.S, gesture.Key);
        Assert.Equal(VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift, gesture.KeyModifiers);
        Assert.Equal("Ctrl+Shift+S", gesture.ToString());

        Assert.Equal(new KeyGesture(VirtualKey.Enter, VirtualKeyModifiers.Menu), KeyGesture.Parse("Alt+Return"));
        Assert.Equal(new KeyGesture((VirtualKey)0xBB, VirtualKeyModifiers.Control), KeyGesture.Parse("Ctrl++"));
        Assert.Equal(new KeyGesture(VirtualKey.Number1), KeyGesture.Parse("1"));
        Assert.Equal(new KeyGesture(VirtualKey.F5, VirtualKeyModifiers.Windows), KeyGesture.Parse("Cmd+F5"));
        Assert.Throws<ArgumentException>(() => KeyGesture.Parse("Hyper+A"));
        Assert.Throws<ArgumentException>(() => KeyGesture.Parse("Ctrl+NotAKey"));
    }

    // ---------------------------------------------------------------- ExecuteCommand

    [UnoHeadlessFact]
    public async Task ExecuteCommandOnKeyDownBehavior_Executes_For_The_Key_And_Gesture()
    {
        var (button, _) = await ShowFocusablesAsync();
        var command = new RecordingCommand();
        Interaction.GetBehaviors(button).Add(new ExecuteCommandOnKeyDownBehavior { Command = command, CommandParameter = "key", Key = VirtualKey.F2 });
        Interaction.GetBehaviors(button).Add(new ExecuteCommandOnKeyDownBehavior { Command = command, CommandParameter = "gesture", Gesture = KeyGesture.Parse("Ctrl+G") });
        await Session.WaitForIdleAsync();

        await TestInput.PressKeyAsync(button, VirtualKey.A);
        Assert.Empty(command.Parameters);

        await TestInput.PressKeyAsync(button, VirtualKey.F2);
        await TestInput.PressKeyAsync(button, VirtualKey.G);
        await TestInput.PressKeyAsync(button, VirtualKey.G, VirtualKey.Control);

        Assert.Equal(["key", "gesture"], command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ExecuteCommandOnKeyDownBehavior_Marks_The_Event_Handled()
    {
        var (button, _) = await ShowFocusablesAsync();
        var command = new RecordingCommand();
        Interaction.GetBehaviors(button).Add(new ExecuteCommandOnKeyDownBehavior { Command = command, Key = VirtualKey.F3 });
        await Session.WaitForIdleAsync();

        var args = TestInput.RaiseKey(button, VirtualKey.F3);

        Assert.True(args.Handled);
        Assert.Single(command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ExecuteCommandOnGotFocus_And_LostFocus_Execute()
    {
        var (button, other) = await ShowFocusablesAsync();
        var gotCommand = new RecordingCommand();
        var lostCommand = new RecordingCommand();
        Interaction.GetBehaviors(button).Add(new ExecuteCommandOnGotFocusBehavior { Command = gotCommand });
        Interaction.GetBehaviors(button).Add(new ExecuteCommandOnLostFocusBehavior { Command = lostCommand });
        await Session.WaitForIdleAsync();

        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        other.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Single(gotCommand.Parameters);
        Assert.Single(lostCommand.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ExecuteCommandOnTextInputBehavior_Executes_On_Characters()
    {
        var (_, other) = await ShowFocusablesAsync();
        var command = new RecordingCommand();
        Interaction.GetBehaviors(other).Add(new ExecuteCommandOnTextInputBehavior { Command = command });
        await Session.WaitForIdleAsync();

        TestInput.RaiseCharacter(other, 'x');

        Assert.Single(command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ExecuteCommandBehavior_Respects_CanExecute_And_Focuses_The_FocusControl()
    {
        var (button, other) = await ShowFocusablesAsync();
        var command = new RecordingCommand { CanExecuteResult = false };
        var behavior = new ExecuteCommandOnKeyDownBehavior { Command = command, Key = VirtualKey.F4, FocusControl = other };
        Interaction.GetBehaviors(button).Add(behavior);
        await Session.WaitForIdleAsync();

        await TestInput.PressKeyAsync(button, VirtualKey.F4);
        Assert.Empty(command.Parameters);
        Assert.False(behavior.CanExecuteCommand);

        command.CanExecuteResult = true;
        behavior.CommandParameter = "p";
        await TestInput.PressKeyAsync(button, VirtualKey.F4);
        await Session.WaitForIdleAsync();

        Assert.Equal(["p"], command.Parameters);
        Assert.NotEqual(FocusState.Unfocused, other.FocusState);
    }

    [UnoHeadlessFact]
    public async Task ExecuteCommand_Pointer_Behaviors_Execute_On_Injected_Input()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Green) };
        var pressed = new RecordingCommand();
        var released = new RecordingCommand();
        var tapped = new RecordingCommand();
        Interaction.GetBehaviors(border).Add(new ExecuteCommandOnPointerPressedBehavior { Command = pressed });
        Interaction.GetBehaviors(border).Add(new ExecuteCommandOnPointerReleasedBehavior { Command = released });
        Interaction.GetBehaviors(border).Add(new ExecuteCommandOnTappedBehavior { Command = tapped });
        await Session.ShowAsync(border);

        await TestInput.TapAsync(injector!, border, 50, 50);

        Assert.Single(pressed.Parameters);
        Assert.Single(released.Parameters);
        Assert.Single(tapped.Parameters);
    }

    // ---------------------------------------------------------------- Focus

    [UnoHeadlessFact]
    public async Task FocusOnAttachedToVisualTreeBehavior_Focuses_When_Loaded()
    {
        var textBox = new TextBox();
        Interaction.GetBehaviors(textBox).Add(new FocusOnAttachedToVisualTreeBehavior());

        await Session.ShowAsync(new StackPanel { Children = { new Button { Content = "b" }, textBox } });
        await Session.WaitForIdleAsync();

        Assert.NotEqual(FocusState.Unfocused, textBox.FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusOnAttachedBehavior_Uses_The_Requested_Focus_State()
    {
        var textBox = new TextBox();
        Interaction.GetBehaviors(textBox).Add(new FocusOnAttachedBehavior { NavigationMethod = FocusState.Keyboard });

        await Session.ShowAsync(new StackPanel { Children = { new Button { Content = "b" }, textBox } });
        await Session.WaitForIdleAsync();

        Assert.Equal(FocusState.Keyboard, textBox.FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusBehavior_Focuses_And_Tracks_Focus_Loss()
    {
        var (button, other) = await ShowFocusablesAsync();
        var behavior = new FocusBehavior();
        Interaction.GetBehaviors(other).Add(behavior);

        behavior.IsFocused = true;
        await Session.WaitForIdleAsync();
        Assert.NotEqual(FocusState.Unfocused, other.FocusState);

        button.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        Assert.False(behavior.IsFocused);
    }

    [UnoHeadlessFact]
    public async Task FocusControlBehavior_Focuses_When_The_Flag_Is_Set()
    {
        var (_, other) = await ShowFocusablesAsync();
        var behavior = new FocusControlBehavior();
        Interaction.GetBehaviors(other).Add(behavior);
        await Session.WaitForIdleAsync();

        behavior.FocusFlag = true;
        await Session.WaitForIdleAsync();

        Assert.NotEqual(FocusState.Unfocused, other.FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusOnVisibleBehavior_Focuses_When_Shown()
    {
        var textBox = new TextBox { Visibility = Visibility.Collapsed };
        Interaction.GetBehaviors(textBox).Add(new FocusOnVisibleBehavior());
        await Session.ShowAsync(new StackPanel { Children = { new Button { Content = "b" }, textBox } });
        await Session.WaitForIdleAsync();
        Assert.Equal(FocusState.Unfocused, textBox.FocusState);

        textBox.Visibility = Visibility.Visible;
        await Session.WaitForIdleAsync();
        await Session.WaitForIdleAsync();

        Assert.NotEqual(FocusState.Unfocused, textBox.FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusSelectedItemBehavior_Focuses_The_Selected_Container()
    {
        var items = new[] { new ListBoxItem { Content = "a" }, new ListBoxItem { Content = "b" }, new ListBoxItem { Content = "c" } };
        var list = new ListBox();
        foreach (var item in items)
        {
            list.Items.Add(item);
        }

        Interaction.GetBehaviors(list).Add(new FocusSelectedItemBehavior());
        await Session.ShowAsync(new StackPanel { Children = { new Button { Content = "b" }, list } });
        await Session.WaitForIdleAsync();

        list.SelectedItem = items[2];
        await TestInput.WaitUntilAsync(() => items[2].FocusState != FocusState.Unfocused);

        Assert.NotEqual(FocusState.Unfocused, items[2].FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusTrapBehavior_Wraps_Tab_Navigation_Inside_The_Scope()
    {
        var first = new Button { Content = "1" };
        var last = new Button { Content = "2" };
        var outside = new Button { Content = "outside" };
        var scope = new StackPanel { Children = { first, last } };
        Interaction.GetBehaviors(scope).Add(new FocusTrapBehavior());
        await Session.ShowAsync(new StackPanel { Children = { scope, outside } });
        last.Focus(FocusState.Keyboard);
        await Session.WaitForIdleAsync();

        var args = TestInput.RaiseKey(last, VirtualKey.Tab);
        await Session.WaitForIdleAsync();

        Assert.True(args.Handled);
        Assert.Same(first, FocusManager.GetFocusedElement(first.XamlRoot!));
    }

    // ---------------------------------------------------------------- Gestures and InputElement triggers

    [UnoHeadlessFact]
    public async Task Gesture_And_Pointer_Triggers_Execute_On_Injected_Input()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Orange) };
        var tapped = new TappedGestureTrigger();
        var pressed = new PointerPressedTrigger();
        var released = new PointerReleasedTrigger();
        var tappedAction = new RecordingAction();
        var pressedAction = new RecordingAction();
        var releasedAction = new RecordingAction();
        tapped.Actions!.Add(tappedAction);
        pressed.Actions!.Add(pressedAction);
        released.Actions!.Add(releasedAction);
        Interaction.GetBehaviors(border).Add(tapped);
        Interaction.GetBehaviors(border).Add(pressed);
        Interaction.GetBehaviors(border).Add(released);
        await Session.ShowAsync(border);

        await TestInput.TapAsync(injector!, border, 40, 40);

        Assert.IsType<TappedRoutedEventArgs>(Assert.Single(tappedAction.Parameters));
        Assert.IsType<PointerRoutedEventArgs>(Assert.Single(pressedAction.Parameters));
        Assert.IsType<PointerRoutedEventArgs>(Assert.Single(releasedAction.Parameters));
    }

    [UnoHeadlessFact]
    public async Task DoubleTapped_Triggers_Execute_On_A_Double_Tap()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Orange) };
        var gesture = new DoubleTappedGestureTrigger();
        var trigger = new DoubleTappedTrigger();
        var gestureAction = new RecordingAction();
        var triggerAction = new RecordingAction();
        gesture.Actions!.Add(gestureAction);
        trigger.Actions!.Add(triggerAction);
        Interaction.GetBehaviors(border).Add(gesture);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);

        await TestInput.TapAsync(injector!, border, 30, 30);
        await TestInput.TapAsync(injector!, border, 30, 30);

        Assert.IsType<DoubleTappedRoutedEventArgs>(Assert.Single(gestureAction.Parameters));
        Assert.Single(triggerAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task KeyTriggers_Filter_By_Key_Gesture_And_Event()
    {
        // A plain element: controls such as Button handle keys like Enter themselves.
        var button = new Border();
        await Session.ShowAsync(button);
        var keyDown = new KeyDownTrigger { Key = VirtualKey.Enter };
        var keyUp = new KeyUpTrigger { Gesture = KeyGesture.Parse("Ctrl+K") };
        var keyTrigger = new KeyTrigger { Key = VirtualKey.Escape, Event = KeyTrigger.FiredOn.KeyUp };
        var gestureTrigger = new KeyGestureTrigger { Gesture = KeyGesture.Parse("Shift+F1") };
        var keyDownAction = new RecordingAction();
        var keyUpAction = new RecordingAction();
        var keyTriggerAction = new RecordingAction();
        var gestureAction = new RecordingAction();
        keyDown.Actions!.Add(keyDownAction);
        keyUp.Actions!.Add(keyUpAction);
        keyTrigger.Actions!.Add(keyTriggerAction);
        gestureTrigger.Actions!.Add(gestureAction);
        Interaction.GetBehaviors(button).Add(keyDown);
        Interaction.GetBehaviors(button).Add(keyUp);
        Interaction.GetBehaviors(button).Add(keyTrigger);
        Interaction.GetBehaviors(button).Add(gestureTrigger);
        await Session.WaitForIdleAsync();

        await TestInput.PressKeyAsync(button, VirtualKey.Enter);
        await TestInput.PressKeyAsync(button, VirtualKey.K, VirtualKey.Control);
        await TestInput.PressKeyAsync(button, VirtualKey.Escape);
        await TestInput.PressKeyAsync(button, VirtualKey.F1, VirtualKey.Shift);

        Assert.Single(keyDownAction.Parameters);
        Assert.Single(keyUpAction.Parameters);
        Assert.Single(keyTriggerAction.Parameters);
        Assert.Single(gestureAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task TextInputTrigger_Filters_By_Text()
    {
        var (_, other) = await ShowFocusablesAsync();
        var trigger = new TextInputTrigger { Text = "a" };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(other).Add(trigger);
        await Session.WaitForIdleAsync();

        TestInput.RaiseCharacter(other, 'b');
        TestInput.RaiseCharacter(other, 'a');

        Assert.IsType<CharacterReceivedRoutedEventArgs>(Assert.Single(action.Parameters));
    }

    [UnoHeadlessFact]
    public async Task ClickEventTrigger_Executes_On_Pointer_Click_And_Keyboard()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Purple) };
        var trigger = new ClickEventTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        var defaultButton = new Button { Content = "default" };
        var defaultTrigger = new ClickEventTrigger { IsDefault = true };
        var defaultAction = new RecordingAction();
        defaultTrigger.Actions!.Add(defaultAction);
        Interaction.GetBehaviors(defaultButton).Add(defaultTrigger);
        var textBox = new TextBox();
        await Session.ShowAsync(new StackPanel { Children = { border, defaultButton, textBox } });

        await TestInput.TapAsync(injector!, border, 50, 50);
        Assert.Single(action.Parameters);

        await TestInput.PressKeyAsync(textBox, VirtualKey.Enter);
        Assert.Single(defaultAction.Parameters);
    }

    // Regression test for https://github.com/wieslawsoltes/Xaml.Behaviors/issues/377: the button releases its pointer
    // capture in its own release handler (before the routed handlers), which must not cancel the pending click of a
    // trigger that captured the pointer on the button.
    [UnoHeadlessFact]
    public async Task ClickEventTrigger_With_Button_SourceControl_Fires_Alongside_The_Button_Trigger()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var button = new Button { Content = "source", Width = 120, Height = 40 };
        var buttonTrigger = new ClickEventTrigger { HandleEvent = false, HandledEventsToo = true };
        var buttonAction = new RecordingAction();
        buttonTrigger.Actions!.Add(buttonAction);
        Interaction.GetBehaviors(button).Add(buttonTrigger);

        var host = new Border { Width = 120, Height = 40, Background = new SolidColorBrush(Microsoft.UI.Colors.SkyBlue) };
        var hostTrigger = new ClickEventTrigger { SourceControl = button, HandledEventsToo = true };
        var hostAction = new RecordingAction();
        hostTrigger.Actions!.Add(hostAction);
        Interaction.GetBehaviors(host).Add(hostTrigger);

        await Session.ShowAsync(new StackPanel { Children = { button, host } });

        await TestInput.TapAsync(injector!, button, 60, 20);

        Assert.Single(buttonAction.Parameters);
        Assert.Single(hostAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task CapturePointerAction_And_ReleasePointerCaptureAction_Manage_The_Capture()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Teal) };
        var pressed = new PointerPressedTrigger();
        pressed.Actions!.Add(new CapturePointerAction());
        var released = new PointerReleasedTrigger();
        var captures = -1;
        released.Actions!.Add(new CaptureProbeAction(() => captures = border.PointerCaptures?.Count ?? 0));
        released.Actions!.Add(new ReleasePointerCaptureAction());
        Interaction.GetBehaviors(border).Add(pressed);
        Interaction.GetBehaviors(border).Add(released);
        await Session.ShowAsync(border);

        await TestInput.TapAsync(injector!, border, 50, 50);

        Assert.Equal(1, captures);
        Assert.True(border.PointerCaptures is null || border.PointerCaptures.Count == 0);
    }

    // ---------------------------------------------------------------- Input

    [UnoHeadlessFact]
    public async Task SelectAllOnFocusBehavior_Selects_The_Text()
    {
        var textBox = new TextBox { Text = "hello" };
        Interaction.GetBehaviors(textBox).Add(new SelectAllOnFocusBehavior());
        await Session.ShowAsync(new StackPanel { Children = { new Button { Content = "b" }, textBox } });

        textBox.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Equal("hello", textBox.SelectedText);
    }

    [UnoHeadlessFact]
    public async Task GlobalHotkeyBehavior_Executes_For_Keys_Pressed_Anywhere()
    {
        var (button, other) = await ShowFocusablesAsync();
        var trigger = new GlobalHotkeyBehavior { Key = VirtualKey.S, KeyModifiers = VirtualKeyModifiers.Control };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.WaitForIdleAsync();

        await TestInput.PressKeyAsync(other, VirtualKey.S);
        Assert.Empty(action.Parameters);

        await TestInput.PressKeyAsync(other, VirtualKey.S, VirtualKey.Control);
        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task InactivityTrigger_Executes_After_The_Timeout()
    {
        var border = new Border();
        var trigger = new InactivityTrigger { Timeout = TimeSpan.FromMilliseconds(50) };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(new StackPanel { Children = { border } });

        await TestInput.WaitUntilAsync(() => action.Parameters.Count > 0);

        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task LoseFocusOnEnterBehavior_Moves_Focus_Away()
    {
        var textBox = new TextBox();
        Interaction.GetBehaviors(textBox).Add(new LoseFocusOnEnterBehavior());
        var root = new ContentControl { Content = textBox, IsTabStop = true };
        await Session.ShowAsync(root);
        textBox.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        await TestInput.PressKeyAsync(textBox, VirtualKey.Enter);

        Assert.Equal(FocusState.Unfocused, textBox.FocusState);
    }

    private sealed partial class CaptureProbeAction(System.Action probe) : Xaml.Interactivity.Action
    {
        public override object? Execute(object? sender, object? parameter)
        {
            probe();
            return null;
        }
    }
}
