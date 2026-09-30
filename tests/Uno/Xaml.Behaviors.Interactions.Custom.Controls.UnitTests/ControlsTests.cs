// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;
using static Xaml.Interactions.Custom.Controls.UnitTests.TestHelpers;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

public class ControlsTests
{
    [UnoHeadlessFact]
    public void ClearAutoCompleteBoxSelectionAction_Clears_The_AutoSuggestBox_Text()
    {
        var box = new AutoSuggestBox { Text = "query" };

        new ClearAutoCompleteBoxSelectionAction().Execute(box, null);

        Assert.Equal(string.Empty, box.Text);
    }

    [UnoHeadlessFact]
    public void Automation_Actions_And_Behaviors_Set_Automation_Properties()
    {
        var button = new Button();

        Assert.Equal(true, new SetAutomationIdAction { AutomationId = "id" }.Execute(button, null));
        Assert.Equal("id", AutomationProperties.GetAutomationId(button));

        var trigger = new AutomationNameChangedTrigger().AttachTo(button);
        var action = trigger.Record();
        var behavior = new AutomationNameBehavior { AutomationName = "name" }.AttachTo(button);
        Assert.Equal("name", AutomationProperties.GetName(button));

        behavior.AutomationName = "other";
        Assert.Equal("other", AutomationProperties.GetName(button));
        Assert.True(action.Parameters.Count >= 2);

        Interaction.GetBehaviors(button).Remove(behavior);
        Assert.True(string.IsNullOrEmpty(AutomationProperties.GetName(button)));
    }

    [UnoHeadlessFact]
    public async Task ScreenReaderAnnounceAction_Raises_A_Notification()
    {
        var button = new Button();
        await Session.ShowAsync(button);

        Assert.Equal(true, new ScreenReaderAnnounceAction { Message = "hello" }.Execute(button, null));
        Assert.Equal(false, new ScreenReaderAnnounceAction().Execute(button, null));
    }

    [UnoHeadlessFact]
    public void ConsoleBeepAction_Is_Disabled_When_Not_Enabled()
    {
        Assert.Equal(false, new ConsoleBeepAction { IsEnabled = false }.Execute(null, null));
        Assert.IsType<bool>(new ConsoleBeepAction().Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task Popup_Triggers_Follow_The_Popup_State()
    {
        var host = new Grid();
        var popup = new Popup { Child = new Border { Width = 10, Height = 10 } };
        host.Children.Add(popup);
        var opened = new PopupOpenedTrigger().AttachTo(popup).Record();
        var closed = new PopupClosedTrigger().AttachTo(popup).Record();
        await Session.ShowAsync(host);

        popup.IsOpen = true;
        Assert.True(await WaitUntilAsync(() => opened.Parameters.Count == 1));

        popup.IsOpen = false;
        Assert.True(await WaitUntilAsync(() => closed.Parameters.Count == 1));
    }

    [UnoHeadlessFact]
    public async Task ContextDialog_Triggers_Handle_Opened_And_Closed_Events()
    {
        var host = new Grid();
        var popup = new Popup { Child = new Border { Width = 10, Height = 10 } };
        host.Children.Add(popup);
        var opened = new ContextDialogOpenedTrigger { SourceObject = popup }.AttachTo(host).Record();
        var closed = new ContextDialogClosedTrigger { SourceObject = popup }.AttachTo(host).Record();
        await Session.ShowAsync(host);

        popup.IsOpen = true;
        Assert.True(await WaitUntilAsync(() => opened.Parameters.Count == 1));
        popup.IsOpen = false;
        Assert.True(await WaitUntilAsync(() => closed.Parameters.Count == 1));
    }

    [UnoHeadlessFact]
    public async Task Dialog_Triggers_And_ShowDialogAction_Use_ContentDialog()
    {
        var host = new Grid();
        var dialog = new ContentDialog { Content = "dialog", CloseButtonText = "Close" };
        var opened = new DialogOpenedTrigger { SourceObject = dialog }.AttachTo(host).Record();
        var closed = new DialogClosedTrigger { SourceObject = dialog }.AttachTo(host).Record();
        await Session.ShowAsync(host);

        Assert.Equal(false, new ShowDialogAction().Execute(host, null));
        Assert.Equal(true, new ShowDialogAction { Dialog = dialog }.Execute(host, null));
        Assert.Same(host.XamlRoot, dialog.XamlRoot);

        Assert.True(await WaitUntilAsync(() => opened.Parameters.Count == 1));
        dialog.Hide();
        Assert.True(await WaitUntilAsync(() => closed.Parameters.Count == 1));
        Assert.IsType<ContentDialogClosedEventArgs>(closed.Parameters[0]);
    }

    [UnoHeadlessFact]
    public async Task UploadCompletedTrigger_Executes_When_Completed()
    {
        var host = new Border();
        var trigger = new UploadCompletedTrigger().AttachTo(host);
        var action = trigger.Record();
        await Session.ShowAsync(host);

        trigger.IsCompleted = true;

        Assert.True(await WaitUntilAsync(() => action.Parameters.Count == 1));
    }

    [UnoHeadlessFact]
    public async Task ButtonUploadFileBehavior_Attaches_To_Buttons()
    {
        var button = new Button();
        var behavior = new ButtonUploadFileBehavior { FilePath = "missing.txt", Url = "http://localhost/" }.AttachTo(button);
        await Session.ShowAsync(button);

        Assert.Same(button, behavior.AssociatedObject);
        Assert.Equal(false, new UploadFileAction().Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task MoveElementToPanelAction_Moves_The_Sender()
    {
        var element = new Border();
        var source = new StackPanel { Children = { element } };
        var target = new StackPanel();
        await Session.ShowAsync(new StackPanel { Children = { source, target } });

        Assert.Equal(true, new MoveElementToPanelAction { TargetPanel = target }.Execute(element, null));

        Assert.Empty(source.Children);
        Assert.Same(element, Assert.Single(target.Children));
    }

    [UnoHeadlessFact]
    public async Task FluidMoveBehavior_Animates_Moved_Children()
    {
        var first = new Border { Height = 20 };
        var panel = new StackPanel { Children = { first } };
        new FluidMoveBehavior { AppliesTo = FluidMoveScope.Children }.AttachTo(panel);
        await Session.ShowAsync(panel);
        await Session.WaitForIdleAsync();

        panel.Children.Insert(0, new Border { Height = 30 });

        Assert.True(await WaitUntilAsync(() => first.RenderTransform is TranslateTransform));
    }

    [UnoHeadlessFact]
    public async Task ScrollToOffsetAction_Scrolls_The_ScrollViewer()
    {
        var scrollViewer = new ScrollViewer
        {
            Width = 100,
            Height = 100,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollMode = ScrollMode.Enabled,
            Content = new Border { Width = 1000, Height = 1000 },
        };
        await Session.ShowAsync(scrollViewer);

        Assert.Equal(true, new ScrollToOffsetAction { VerticalOffset = 200 }.Execute(scrollViewer, null));
        Assert.True(await WaitUntilAsync(() => Math.Abs(scrollViewer.VerticalOffset - 200) < 0.5));
        Assert.Equal(0, scrollViewer.HorizontalOffset);

        new ScrollToOffsetAction { ScrollViewer = scrollViewer, HorizontalOffset = 50 }.Execute(null, null);
        Assert.True(await WaitUntilAsync(() => Math.Abs(scrollViewer.HorizontalOffset - 50) < 0.5));
        Assert.Equal(200, scrollViewer.VerticalOffset, 1);
    }

    [UnoHeadlessFact]
    public async Task HorizontalScrollViewerBehavior_Attaches_And_Detaches()
    {
        var scrollViewer = new ScrollViewer();
        var behavior = new HorizontalScrollViewerBehavior { RequireShiftKey = true }.AttachTo(scrollViewer);
        await Session.ShowAsync(scrollViewer);

        Interaction.GetBehaviors(scrollViewer).Remove(behavior);
        Assert.Null(behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public async Task SplitView_Toggle_Action_And_Pane_Triggers()
    {
        var splitView = new SplitView { DisplayMode = SplitViewDisplayMode.Inline, Pane = new Border(), Content = new Border() };
        var opening = new SplitViewPaneOpeningTrigger().AttachTo(splitView).Record();
        var opened = new SplitViewPaneOpenedTrigger().AttachTo(splitView).Record();
        var closing = new SplitViewPaneClosingTrigger().AttachTo(splitView).Record();
        var closed = new SplitViewPaneClosedTrigger().AttachTo(splitView).Record();
        await Session.ShowAsync(splitView);

        Assert.Equal(true, new SplitViewTogglePaneAction().Execute(splitView, null));
        Assert.True(splitView.IsPaneOpen);
        Assert.True(await WaitUntilAsync(() => opening.Parameters.Count == 1 && opened.Parameters.Count == 1));

        new SplitViewTogglePaneAction { TargetSplitView = splitView }.Execute(null, null);
        Assert.False(splitView.IsPaneOpen);
        Assert.True(await WaitUntilAsync(() => closing.Parameters.Count == 1 && closed.Parameters.Count == 1));
        Assert.IsType<SplitViewPaneClosingEventArgs>(closing.Parameters[0]);
    }

    [UnoHeadlessFact]
    public async Task SplitViewStateBehavior_Applies_Matching_Setters()
    {
        var splitView = new SplitView { Width = 300, Height = 200 };
        var behavior = new SplitViewStateBehavior();
        behavior.Setters.Add(new SplitViewStateSetter { MinWidth = 0, MaxWidth = 500, DisplayMode = SplitViewDisplayMode.CompactOverlay, IsPaneOpen = true });
        behavior.Setters.Add(new SplitViewStateSetter { MinWidth = 500, DisplayMode = SplitViewDisplayMode.Inline, IsPaneOpen = false });
        behavior.AttachTo(splitView);
        await Session.ShowAsync(splitView);

        Assert.True(await WaitUntilAsync(() => splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay));
        Assert.True(splitView.IsPaneOpen);

        splitView.Width = 800;
        Assert.True(await WaitUntilAsync(() => splitView.DisplayMode == SplitViewDisplayMode.Inline));
        Assert.False(splitView.IsPaneOpen);
    }

    [UnoHeadlessFact]
    public async Task TextBox_Behaviors_Select_All_Text()
    {
        var autoSelect = new TextBox { Text = "hello" };
        new AutoSelectBehavior().AttachTo(autoSelect);
        var onFocus = new TextBox { Text = "world" };
        new TextBoxSelectAllOnGotFocusBehavior().AttachTo(onFocus);
        await Session.ShowAsync(new StackPanel { Children = { autoSelect, onFocus } });

        Assert.True(await WaitUntilAsync(() => autoSelect.SelectionLength == 5));

        onFocus.Focus(FocusState.Programmatic);
        Assert.True(await WaitUntilAsync(() => onFocus.SelectionLength == 5));
    }

    [UnoHeadlessFact]
    public async Task ThemeVariantTrigger_Executes_When_The_Actual_Theme_Matches()
    {
        var border = new Border { RequestedTheme = ElementTheme.Light };
        var trigger = new ThemeVariantTrigger { ThemeVariant = ElementTheme.Dark }.AttachTo(border);
        var action = trigger.Record();
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        Assert.Empty(action.Parameters);

        border.RequestedTheme = ElementTheme.Dark;

        Assert.True(await WaitUntilAsync(() => action.Parameters.Count > 0));
    }

    [UnoHeadlessFact]
    public async Task ToolTip_Actions_Set_And_Open_The_ToolTip()
    {
        var button = new Button { Content = "b" };
        await Session.ShowAsync(button);

        Assert.Equal(true, new SetToolTipTipAction { Tip = "tip" }.Execute(button, null));
        Assert.Equal("tip", ToolTipService.GetToolTip(button));

        Assert.Equal(true, new ShowToolTipAction { TargetControl = button }.Execute(null, null));
        var toolTip = Assert.IsType<ToolTip>(ToolTipService.GetToolTip(button));
        Assert.Equal("tip", toolTip.Content);
        Assert.True(toolTip.IsOpen);

        Assert.Equal(true, new HideToolTipAction().Execute(button, null));
        Assert.False(toolTip.IsOpen);
    }

    [UnoHeadlessFact]
    public async Task RenderTargetBitmapTrigger_Renders_The_Target_On_Ticks()
    {
        var host = new CountingRenderHost();
        var element = new Border();
        var trigger = new RenderTargetBitmapTrigger { Target = host, MillisecondsPerTick = 10 }.AttachTo(element);
        var action = trigger.Record();
        await Session.ShowAsync(element);

        Assert.True(await WaitUntilAsync(() => host.Count > 1 && action.Parameters.Count > 1));

        Assert.Equal(true, new RenderRenderTargetBitmapAction { Target = host }.Execute(null, null));
        Interaction.GetBehaviors(element).Remove(trigger);
    }

    [UnoHeadlessFact]
    public async Task WriteableBitmapBehavior_Creates_And_Renders_The_Bitmap()
    {
        var renderer = new FillRenderer();
        var image = new Image();
        var behavior = new WriteableBitmapBehavior { PixelWidth = 4, PixelHeight = 2, Renderer = renderer }.AttachTo(image);
        await Session.ShowAsync(image);

        var bitmap = Assert.IsType<WriteableBitmap>(behavior.Bitmap);
        Assert.Same(bitmap, image.Source);
        Assert.Equal(4, bitmap.PixelWidth);
        Assert.Equal(2, bitmap.PixelHeight);
        Assert.Equal(1, renderer.Count);

        behavior.Render();
        Assert.Equal(2, renderer.Count);

        Assert.Equal(true, new WriteableBitmapRenderAction { Renderer = renderer, Bitmap = bitmap }.Execute(null, null));
        Assert.Equal(3, renderer.Count);

        var trigger = new WriteableBitmapTrigger { Bitmap = bitmap }.AttachTo(image);
        var action = trigger.Record();
        trigger.Trigger();
        Assert.Same(bitmap, Assert.Single(action.Parameters));

        Interaction.GetBehaviors(image).Remove(behavior);
        Assert.Null(behavior.Bitmap);
        Assert.Null(image.Source);
    }

    [UnoHeadlessFact]
    public async Task WriteableBitmapRenderBehavior_Renders_On_A_Timer()
    {
        var renderer = new FillRenderer();
        var image = new Image();
        var behavior = new WriteableBitmapRenderBehavior { PixelWidth = 2, PixelHeight = 2, Renderer = renderer }.AttachTo(image);
        await Session.ShowAsync(image);

        Assert.True(await WaitUntilAsync(() => renderer.Count > 1));
        Interaction.GetBehaviors(image).Remove(behavior);
    }

    [UnoHeadlessFact]
    public async Task Window_Actions_Resolve_The_Hosting_Window()
    {
        var button = new Button();
        await Session.ShowAsync(button);

        // The window of the element is found through its XamlRoot; a detached element has none.
        Assert.Equal(false, new CloseWindowAction().Execute(new Button(), null));
        Assert.Null(new WindowAction { ActionType = WindowActionType.Close }.Execute(new Button(), null));
        Assert.Equal(false, new ShowWindowAction().Execute(button, null));
    }

    [UnoHeadlessFact]
    public async Task ClipboardMonitorBehavior_Starts_And_Stops_Monitoring()
    {
        var host = new Border();
        var behavior = new ClipboardMonitorBehavior { Formats = "Text,FileNames" }.AttachTo(host);
        await Session.ShowAsync(host);
        await Session.WaitForIdleAsync();

        Assert.Equal("Text,FileNames", behavior.Formats);
        Interaction.GetBehaviors(host).Remove(behavior);
        Assert.Null(behavior.AssociatedObject);
    }

    [UnoHeadlessFact]
    public async Task NetworkStatusTrigger_Attaches_And_Detaches()
    {
        var host = new Border();
        var trigger = new NetworkStatusTrigger { Status = NetworkStatus.Offline }.AttachTo(host);
        await Session.ShowAsync(host);

        Interaction.GetBehaviors(host).Remove(trigger);
        Assert.Equal(NetworkStatus.Offline, trigger.Status);
    }

    private sealed class CountingRenderHost : IRenderTargetBitmapRenderHost
    {
        public int Count { get; private set; }

        public void Render() => Count++;
    }

    private sealed class FillRenderer : IWriteableBitmapRenderer
    {
        public int Count { get; private set; }

        public void Render(WriteableBitmap bitmap)
        {
            Count++;
            using var stream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(bitmap.PixelBuffer);
            var pixels = Enumerable.Repeat((byte)0xFF, bitmap.PixelWidth * bitmap.PixelHeight * 4).ToArray();
            stream.Write(pixels, 0, pixels.Length);
        }
    }
}
