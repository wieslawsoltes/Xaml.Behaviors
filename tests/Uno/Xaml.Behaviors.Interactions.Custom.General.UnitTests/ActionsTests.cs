// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

public class ActionsTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void Show_And_Hide_Control_Actions_Toggle_Visibility()
    {
        var border = new Border();

        Assert.True((bool)new HideControlAction().Execute(border, null));
        Assert.Equal(Visibility.Collapsed, border.Visibility);

        Assert.True((bool)new ShowControlAction { TargetControl = border }.Execute(null, null));
        Assert.Equal(Visibility.Visible, border.Visibility);

        Assert.False((bool)new ShowControlAction { IsEnabled = false }.Execute(border, null));
    }

    [UnoHeadlessFact]
    public async Task DelayedShowControlAction_Shows_After_The_Delay()
    {
        var border = new Border { Visibility = Visibility.Collapsed };
        await Session.ShowAsync(new StackPanel { Children = { border } });

        new DelayedShowControlAction { Delay = TimeSpan.FromMilliseconds(30) }.Execute(border, null);
        Assert.Equal(Visibility.Collapsed, border.Visibility);

        await TestInput.WaitUntilAsync(() => border.Visibility == Visibility.Visible);
        Assert.Equal(Visibility.Visible, border.Visibility);
    }

    [UnoHeadlessFact]
    public void SetEnabledAction_Sets_IsEnabled_On_Controls_Only()
    {
        var button = new Button();

        Assert.True((bool)new SetEnabledAction { IsEnabledValue = false }.Execute(button, null));
        Assert.False(button.IsEnabled);

        Assert.False((bool)new SetEnabledAction().Execute(new Border(), null));
    }

    [UnoHeadlessFact]
    public void RemoveElementAction_Removes_From_Panels_Borders_And_ContentControls()
    {
        var inPanel = new Border();
        var panel = new StackPanel { Children = { inPanel } };
        var inBorder = new TextBlock();
        var border = new Border { Child = inBorder };
        var inContent = new TextBlock();
        var contentControl = new ContentControl { Content = inContent };
#if WINUI
        // Native WinUI sets FrameworkElement.Parent of the elements of a live tree only.
        Session.Show(new StackPanel { Children = { panel, border, contentControl } });
#endif

        Assert.True((bool)new RemoveElementAction().Execute(inPanel, null));
        Assert.Empty(panel.Children);

        Assert.True((bool)new RemoveElementAction { TargetObject = inBorder }.Execute(null, null));
        Assert.Null(border.Child);

        Assert.True((bool)new RemoveElementAction().Execute(inContent, null));
        Assert.Null(contentControl.Content);

        Assert.False((bool)new RemoveElementAction().Execute(new Border(), null));
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Sets_And_Converts_Values()
    {
        var border = new Border();

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = FrameworkElement.WidthProperty, Value = "42" }.Execute(border, null));
        Assert.Equal(42d, border.Width);

        var textBlock = new TextBlock();
        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetObject = textBlock, TargetProperty = TextBlock.TextProperty, Value = "text" }.Execute(null, null));
        Assert.Equal("text", textBlock.Text);

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = UIElement.VisibilityProperty, Value = "Collapsed" }.Execute(border, null));
        Assert.Equal(Visibility.Collapsed, border.Visibility);
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Converts_Strings_With_An_Explicit_Property_Type()
    {
        // Border.Background has no default value and no current value: the type cannot be inferred.
        var border = new Border();
        var action = new ChangeAvaloniaPropertyAction
        {
            TargetObject = border,
            TargetProperty = Border.BackgroundProperty,
            TargetPropertyType = typeof(Brush),
            Value = "Black",
        };

        Assert.True((bool)action.Execute(null, null));

        var brush = Assert.IsType<SolidColorBrush>(border.Background);
        Assert.Equal(Microsoft.UI.Colors.Black, brush.Color);
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Explicit_Property_Type_Rejects_Invalid_Values()
    {
        var border = new Border();
        var action = new ChangeAvaloniaPropertyAction
        {
            TargetProperty = FrameworkElement.WidthProperty,
            TargetPropertyType = typeof(double),
            Value = "not a number",
        };

        Assert.Throws<ArgumentException>(() => action.Execute(border, null));
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Infers_The_Type_From_The_Current_Value()
    {
        var border = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Gray) };

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = Border.BackgroundProperty, Value = "White" }.Execute(border, null));

        var brush = Assert.IsType<SolidColorBrush>(border.Background);
        Assert.Equal(Microsoft.UI.Colors.White, brush.Color);
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Assigns_Other_Values_Of_The_Property_Type_When_The_Type_Is_Inferred()
    {
        // The inferred type (SolidColorBrush) is more derived than the property type (Brush).
        var border = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Gray) };
        var gradient = new LinearGradientBrush();

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = Border.BackgroundProperty, Value = gradient }.Execute(border, null));

        Assert.Same(gradient, border.Background);
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Assigns_Strings_That_Do_Not_Convert_To_The_Inferred_Type()
    {
        // ContentControl.Content is an object property: its current value does not restrict the new value.
        var contentControl = new ContentControl { Content = new Button() };

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = ContentControl.ContentProperty, Value = "text" }.Execute(contentControl, null));

        Assert.Equal("text", contentControl.Content);
    }

    [UnoHeadlessFact]
    public void ChangeAvaloniaPropertyAction_Assigns_Values_When_The_Type_Is_Unknown()
    {
        var border = new Border();
        var brush = new SolidColorBrush(Microsoft.UI.Colors.Red);

        Assert.True((bool)new ChangeAvaloniaPropertyAction { TargetProperty = Border.BackgroundProperty, Value = brush }.Execute(border, null));

        Assert.Same(brush, border.Background);
    }

    [UnoHeadlessFact]
    public void SetThemeVariantAction_Sets_RequestedTheme()
    {
        var border = new Border();

        Assert.True((bool)new SetThemeVariantAction { ThemeVariant = ElementTheme.Dark }.Execute(border, null));
        Assert.Equal(ElementTheme.Dark, border.RequestedTheme);

        Assert.True((bool)new SetThemeVariantAction { Target = border }.Execute(null, null));
        Assert.Equal(ElementTheme.Default, border.RequestedTheme);
    }

    [UnoHeadlessFact]
    public async Task FocusControlAction_Focuses_The_Target()
    {
        var button = new Button { Content = "b" };
        var textBox = new TextBox();
        await Session.ShowAsync(new StackPanel { Children = { button, textBox } });

        new FocusControlAction { TargetControl = textBox }.Execute(button, null);
        await Session.WaitForIdleAsync();

        Assert.NotEqual(FocusState.Unfocused, textBox.FocusState);
    }

    [UnoHeadlessFact]
    public async Task FocusNextElementAction_Moves_Focus_In_Tab_Order()
    {
        var first = new Button { Content = "1" };
        var second = new Button { Content = "2" };
        await Session.ShowAsync(new StackPanel { Children = { first, second } });
        first.Focus(FocusState.Keyboard);
        await Session.WaitForIdleAsync();

        new FocusNextElementAction().Execute(first, null);
        await Session.WaitForIdleAsync();

        Assert.Same(second, FocusManager.GetFocusedElement(first.XamlRoot!));
    }

    [UnoHeadlessFact]
    public async Task Show_And_Hide_Popup_Actions_Open_And_Close_The_Popup()
    {
        var button = new Button { Content = "b" };
        var popup = new Popup { Child = new TextBlock { Text = "popup" } };
        await Session.ShowAsync(new StackPanel { Children = { button } });
        popup.XamlRoot = button.XamlRoot;

        Assert.True((bool)new ShowPopupAction { Popup = popup }.Execute(button, null));
        Assert.True(popup.IsOpen);
        Assert.Same(button, popup.PlacementTarget);

        Assert.True((bool)new HidePopupAction { Popup = popup }.Execute(button, null));
        Assert.False(popup.IsOpen);
    }

    [UnoHeadlessFact]
    public async Task PopupAction_Opens_A_Light_Dismiss_Popup_With_The_DataContext()
    {
        var vm = new TestViewModel();
        var button = new Button { Content = "b", DataContext = vm };
        var child = new TextBlock();
        await Session.ShowAsync(new StackPanel { Children = { button } });
        var action = new PopupAction { Child = child };

        action.Execute(button, null);
        await Session.WaitForIdleAsync();

        var popup = Assert.Single(VisualTreeHelper.GetOpenPopupsForXamlRoot(button.XamlRoot), p => ReferenceEquals(p.Child, child));
        Assert.True(popup.IsOpen);
        Assert.True(popup.IsLightDismissEnabled);
        Assert.Same(vm, popup.DataContext);
        popup.IsOpen = false;
    }

    [UnoHeadlessFact]
    public async Task ShowFlyoutAction_And_HideFlyoutAction_Use_The_Attached_Flyout()
    {
        var button = new Button { Content = "b" };
        var flyout = new Flyout { Content = new TextBlock { Text = "flyout" } };
        FlyoutBase.SetAttachedFlyout(button, flyout);
        await Session.ShowAsync(new StackPanel { Children = { button } });

        Assert.Equal(true, new ShowFlyoutAction().Execute(button, null));
        await Session.WaitForIdleAsync();
        Assert.True(flyout.IsOpen);

        Assert.True((bool)new HideFlyoutAction().Execute(button, null));
        await Session.WaitForIdleAsync();
        Assert.False(flyout.IsOpen);
    }

    [UnoHeadlessFact]
    public async Task ShowContextMenuAction_Opens_The_ContextFlyout()
    {
        var button = new Button { Content = "b" };
        var menu = new MenuFlyout { Items = { new MenuFlyoutItem { Text = "item" } } };
        button.ContextFlyout = menu;
        await Session.ShowAsync(new StackPanel { Children = { button } });

        Assert.True((bool)new ShowContextMenuAction().Execute(button, null));
        await Session.WaitForIdleAsync();

        Assert.True(menu.IsOpen);
        menu.Hide();
        Assert.False((bool)new ShowContextMenuAction().Execute(new Border(), null));
    }

    [UnoHeadlessFact]
    public void LaunchUriAction_Rejects_Invalid_Uris()
    {
        Assert.False((bool)new LaunchUriAction().Execute(null, null));
        Assert.False((bool)new LaunchUriAction { Uri = "not an absolute uri" }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task CallMethodAsyncAction_Invokes_The_Target_Method()
    {
        var target = new CoreTests.AsyncTarget();

        Assert.True((bool)new CallMethodAsyncAction { TargetObject = target, MethodName = nameof(CoreTests.AsyncTarget.LoadAsync) }.Execute(null, null));
        await Session.WaitForIdleAsync();

        Assert.Equal(1, target.Calls);
    }

    [UnoHeadlessFact]
    public void LogAction_Formats_The_Message()
    {
        Assert.Equal(true, new LogAction { Message = "value {0}", Argument = 1 }.Execute(null, null));
        Assert.Null(new LogAction { IsEnabled = false }.Execute(null, null));
    }
}
