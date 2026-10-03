// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom.Converters;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

public class ControlsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    // ---------------------------------------------------------------- Behaviors

    [UnoHeadlessFact]
    public async Task AutoScrollToBottomBehavior_Scrolls_When_Items_Are_Added()
    {
        var items = new ObservableCollection<string>();
        var panel = new StackPanel();
        var scrollViewer = new ScrollViewer { Height = 100, Content = panel };
        await Session.ShowAsync(scrollViewer);

        // The headless host has no render loop: skip when scroll requests are not applied.
        panel.Children.Add(new Border { Height = 300 });
        await Session.WaitForIdleAsync();
        scrollViewer.ChangeView(null, 10, null, disableAnimation: true);
        await TestInput.WaitUntilAsync(() => scrollViewer.VerticalOffset > 0, 500);
        Assert.SkipWhen(scrollViewer.VerticalOffset == 0, "ScrollViewer.ChangeView is not applied by the headless host.");
        scrollViewer.ChangeView(null, 0, null, disableAnimation: true);
        await TestInput.WaitUntilAsync(() => scrollViewer.VerticalOffset == 0);

        var behavior = new AutoScrollToBottomBehavior { ItemsSource = items };
        Interaction.GetBehaviors(scrollViewer).Add(behavior);
        for (var i = 0; i < 20; i++)
        {
            panel.Children.Add(new Border { Height = 30 });
            items.Add(i.ToString());
        }

        await TestInput.WaitUntilAsync(() => scrollViewer.ScrollableHeight > 0 && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 1);

        Assert.Equal(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset, 1.0);
    }

    // ---------------------------------------------------------------- Button

    [UnoHeadlessFact]
    public async Task ButtonClickEventTriggerBehavior_Executes_On_Click()
    {
        var button = new Button { Content = "b" };
        var trigger = new ButtonClickEventTriggerBehavior();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        TestInput.Click(button);

        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ButtonClickEventTriggerBehavior_Requires_The_Configured_Modifiers()
    {
        var button = new Button { Content = "b" };
        var trigger = new ButtonClickEventTriggerBehavior { KeyModifiers = VirtualKeyModifiers.Control };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        TestInput.Click(button);
        Assert.Empty(action.Parameters);

        TestInput.RaiseKey(button, VirtualKey.Control);
        TestInput.Click(button);
        TestInput.RaiseKey(button, VirtualKey.Control, keyUp: true);
        Assert.Single(action.Parameters);
    }

    [UnoHeadlessFact]
    public async Task InvokeButtonClickAction_Raises_Click()
    {
        var button = new Button { Content = "b" };
        var clicks = 0;
        button.Click += (_, _) => clicks++;
        await Session.ShowAsync(button);

        Assert.True((bool)new InvokeButtonClickAction().Execute(button, null));
        Assert.Equal(1, clicks);

        button.IsEnabled = false;
        Assert.False((bool)new InvokeButtonClickAction { TargetButton = button }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task ButtonHideFlyoutBehavior_Hides_The_Flyout()
    {
        var flyout = new Flyout { Content = new TextBlock { Text = "f" } };
        var button = new Button { Content = "b", Flyout = flyout };
        var behavior = new ButtonHideFlyoutBehavior { IsFlyoutOpen = true };
        Interaction.GetBehaviors(button).Add(behavior);
        await Session.ShowAsync(button);
        flyout.ShowAt(button);
        await Session.WaitForIdleAsync();
        Assert.True(flyout.IsOpen);

        behavior.IsFlyoutOpen = false;
        await Session.WaitForIdleAsync();

        Assert.False(flyout.IsOpen);
    }

    [UnoHeadlessFact]
    public async Task ButtonHidePopupOnClickBehavior_Closes_The_Hosting_Popup()
    {
        var command = new RecordingCommand();
        var button = new Button { Content = "b", Command = command };
        Interaction.GetBehaviors(button).Add(new ButtonHidePopupOnClickBehavior());
        var anchor = new Border();
        await Session.ShowAsync(anchor);
        var popup = new Popup { Child = button, XamlRoot = anchor.XamlRoot };
        popup.IsOpen = true;
        await Session.WaitForIdleAsync();
        await TestInput.WaitUntilAsync(() => button.IsLoaded);

        TestInput.Click(button);
        await Session.WaitForIdleAsync();

        Assert.False(popup.IsOpen);
        Assert.NotEmpty(command.Parameters);
    }

    [UnoHeadlessFact]
    public async Task ButtonExecuteCommandOnKeyDownBehavior_Executes_The_Button_Command_On_The_Root_Key()
    {
        var command = new RecordingCommand();
        var button = new Button { Content = "b", Command = command, CommandParameter = "p" };
        var other = new TextBox();
        Interaction.GetBehaviors(button).Add(new ButtonExecuteCommandOnKeyDownBehavior { Key = VirtualKey.F5 });
        var root = new StackPanel { Children = { button, other } };
        await Session.ShowAsync(root);

        await TestInput.PressKeyAsync(other, VirtualKey.F5);

        Assert.Equal(["p"], command.Parameters);
    }

    // ---------------------------------------------------------------- Collections

    [UnoHeadlessFact]
    public void Collection_Actions_Add_Remove_And_Clear_Items()
    {
        var list = new ObservableCollection<int> { 1 };

        Assert.True((bool)new AddRangeAction { Target = list, Items = new[] { 2, 3 } }.Execute(null, null));
        Assert.Equal([1, 2, 3], list);

        Assert.True((bool)new RemoveRangeAction { Target = list, Items = new[] { 1, 3, 4 } }.Execute(null, null));
        Assert.Equal([2], list);

        Assert.True((bool)new ClearCollectionAction { Target = list }.Execute(null, null));
        Assert.Empty(list);

        Assert.False((bool)new ClearCollectionAction().Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task CollectionChangedTrigger_And_Behavior_Execute_On_Changes()
    {
        var list = new ObservableCollection<int>();
        var border = new Border();
        var trigger = new CollectionChangedTrigger();
        var triggerAction = new RecordingAction();
        trigger.Actions!.Add(triggerAction);
        var behavior = new CollectionChangedBehavior();
        var added = new RecordingAction();
        var removed = new RecordingAction();
        var reset = new RecordingAction();
        behavior.AddedActions.Add(added);
        behavior.RemovedActions.Add(removed);
        behavior.ResetActions.Add(reset);
        Interaction.GetBehaviors(border).Add(trigger);
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(border);
        trigger.Collection = list;
        behavior.Collection = list;

        list.Add(1);
        list.Remove(1);
        list.Clear();

        Assert.Equal(3, triggerAction.Parameters.Count);
        Assert.Single(added.Parameters);
        Assert.Single(removed.Parameters);
        Assert.IsType<NotifyCollectionChangedEventArgs>(Assert.Single(reset.Parameters));
    }

    // ---------------------------------------------------------------- Control

    [UnoHeadlessFact]
    public async Task DelayedLoadBehavior_Hides_Then_Shows_The_Control()
    {
        var border = new Border();
        Interaction.GetBehaviors(border).Add(new DelayedLoadBehavior { Delay = TimeSpan.FromMilliseconds(50) });

        await Session.ShowAsync(new StackPanel { Children = { border } });
        Assert.Equal(Visibility.Collapsed, border.Visibility);

        await TestInput.WaitUntilAsync(() => border.Visibility == Visibility.Visible);
        Assert.Equal(Visibility.Visible, border.Visibility);
    }

    [UnoHeadlessFact]
    public async Task HideOnKeyPressedBehavior_Hides_The_Target_On_The_Key()
    {
        var target = new Border();
        var textBox = new TextBox();
        Interaction.GetBehaviors(textBox).Add(new HideOnKeyPressedBehavior { TargetControl = target });
        await Session.ShowAsync(new StackPanel { Children = { textBox, target } });

        await TestInput.PressKeyAsync(textBox, VirtualKey.A);
        Assert.Equal(Visibility.Visible, target.Visibility);

        await TestInput.PressKeyAsync(textBox, VirtualKey.Escape);
        Assert.Equal(Visibility.Collapsed, target.Visibility);
    }

    [UnoHeadlessFact]
    public async Task HideOnLostFocusBehavior_Hides_The_Target()
    {
        var target = new Border();
        var textBox = new TextBox();
        var other = new Button { Content = "o" };
        Interaction.GetBehaviors(textBox).Add(new HideOnLostFocusBehavior { TargetControl = target });
        await Session.ShowAsync(new StackPanel { Children = { textBox, other, target } });

        textBox.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();
        other.Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Equal(Visibility.Collapsed, target.Visibility);
    }

    [UnoHeadlessFact]
    public async Task InlineEditBehavior_Switches_Between_Display_And_Edit()
    {
#if WINUI
        // Native WinUI key input goes to the focused element, which has to be a control.
        var display = new ContentControl { IsTabStop = true, Content = new TextBlock { Text = "value" } };
#else
        var display = new TextBlock { Text = "value" };
#endif
        var edit = new TextBox { Text = "value" };
        var host = new StackPanel { Children = { display, edit } };
        Interaction.GetBehaviors(host).Add(new InlineEditBehavior { DisplayControl = display, EditControl = edit });
        await Session.ShowAsync(host);
        Assert.Equal(Visibility.Collapsed, edit.Visibility);

        await TestInput.PressKeyAsync(display, VirtualKey.F2);
        Assert.Equal(Visibility.Visible, edit.Visibility);
        Assert.Equal(Visibility.Collapsed, display.Visibility);

        await TestInput.PressKeyAsync(edit, VirtualKey.Enter);
        Assert.Equal(Visibility.Collapsed, edit.Visibility);
        Assert.Equal(Visibility.Visible, display.Visibility);
    }

    [UnoHeadlessFact]
    public async Task BoundsObserverBehavior_Publishes_Width_And_Height()
    {
        var border = new Border();
        var behavior = new BoundsObserverBehavior();
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(border);

        behavior.Bounds = new Rect(1, 2, 30, 40);

        Assert.Equal(30, behavior.Width);
        Assert.Equal(40, behavior.Height);
    }

    [UnoHeadlessFact]
    public async Task BindTagToVisualRootDataContextBehavior_Binds_The_Root_DataContext()
    {
        var vm = new TestViewModel();
        var border = new Border();
        var root = new StackPanel { DataContext = vm, Children = { border } };
        await Session.ShowAsync(root);

        Interaction.GetBehaviors(border).Add(new BindTagToVisualRootDataContextBehavior());

        Assert.Same(vm, border.Tag);
    }

    [UnoHeadlessFact]
    public async Task HideAttachedFlyoutBehavior_Hides_The_Attached_Flyout()
    {
        var border = new Border { Width = 10, Height = 10 };
        var flyout = new Flyout { Content = new TextBlock { Text = "f" } };
        FlyoutBase.SetAttachedFlyout(border, flyout);
        var behavior = new HideAttachedFlyoutBehavior { IsFlyoutOpen = true };
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(border);
        FlyoutBase.ShowAttachedFlyout(border);
        await Session.WaitForIdleAsync();
        Assert.True(flyout.IsOpen);

        behavior.IsFlyoutOpen = false;
        await Session.WaitForIdleAsync();

        Assert.False(flyout.IsOpen);
    }

    [UnoHeadlessFact]
    public async Task Pointer_Behaviors_Track_Injected_Input()
    {
        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var target = new Border { Width = 50, Height = 50, Background = new SolidColorBrush(Microsoft.UI.Colors.Blue) };
        var text = new TextBlock();
        var canvas = new Canvas { Width = 300, Height = 300, Background = new SolidColorBrush(Microsoft.UI.Colors.White), Children = { target } };
        var pointerOver = new BindPointerOverBehavior();
        Interaction.GetBehaviors(target).Add(pointerOver);
        Interaction.GetBehaviors(target).Add(new ShowPointerPositionBehavior { TargetTextBlock = text });
        Interaction.GetBehaviors(target).Add(new DragControlBehavior());
        await Session.ShowAsync(new StackPanel { Children = { canvas, text } });

        var entered = 0;
        target.PointerEntered += (_, _) => entered++;
        await TestInput.MoveAsync(injector!, canvas, 280, 280);
        await TestInput.MoveAsync(injector!, target, 10, 10);
        await TestInput.MoveAsync(injector!, target, 12, 12);
        Assert.True(entered > 0, "PointerEntered was not raised by the injected input.");
        Assert.True(pointerOver.IsPointerOver, "IsPointerOver");
        Assert.False(string.IsNullOrEmpty(text.Text), "ShowPointerPosition");

        await TestInput.LeftDownAsync(injector!);
        await TestInput.MoveAsync(injector!, canvas, 100, 100);
        await TestInput.LeftUpAsync(injector!);

        var transform = Assert.IsType<TranslateTransform>(target.RenderTransform);
        Assert.True(transform.X > 0);
        Assert.True(transform.Y > 0);

        await TestInput.MoveAsync(injector!, canvas, 280, 280);
        Assert.False(pointerOver.IsPointerOver);
    }

    // ---------------------------------------------------------------- Converters

    [UnoHeadlessFact]
    public async Task PointerEventArgsConverter_Converts_Pointer_Positions()
    {
        var converter = PointerEventArgsConverter.Instance;
        Assert.Same(DependencyProperty.UnsetValue, converter.Convert(null, typeof(object), null, string.Empty));
        Assert.Same(DependencyProperty.UnsetValue, converter.ConvertBack(null, typeof(object), null, string.Empty));

        var injector = TestInput.CreateInjector();
        Assert.SkipWhen(injector is null, "Input injection is not available.");

        var border = new Border { Width = 100, Height = 100, Background = new SolidColorBrush(Microsoft.UI.Colors.Red) };
        object? converted = null;
        border.PointerPressed += (_, e) => converted = converter.Convert(e, typeof(object), null, string.Empty);
        await Session.ShowAsync(border);

        await TestInput.TapAsync(injector!, border, 20, 30);

        var (x, y) = Assert.IsType<(double, double)>(converted);
        Assert.Equal(20, x, 1.0);
        Assert.Equal(30, y, 1.0);
    }
}
