// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Input.Preview.Injection;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;
using static Xaml.Interactions.Custom.Controls.UnitTests.TestHelpers;

namespace Xaml.Interactions.Custom.Controls.UnitTests;

/// <summary>
/// Tests of the control behaviors that derive from Core base types.
/// </summary>
/// <remarks>
/// Not compiled until Core/** of Xaml.Behaviors.Interactions.Custom is ported (see the project file and
/// src/Uno/Xaml.Behaviors.Interactions.Custom/SharedSources.props).
/// </remarks>
public class CoreDependentTests
{
    [UnoHeadlessFact]
    public async Task AutoCompleteBox_Focus_Behaviors_Open_The_Suggestions_And_Focus_The_TextBox()
    {
        var box = new AutoSuggestBox { ItemsSource = new[] { "a", "b" } };
        new AutoCompleteBoxOpenDropDownOnFocusBehavior().AttachTo(box);
        new FocusAutoCompleteBoxTextBoxBehavior().AttachTo(box);
        await Session.ShowAsync(new StackPanel { Children = { new Button(), box } });

        box.Focus(FocusState.Programmatic);

        Assert.True(await WaitUntilAsync(() => box.IsSuggestionListOpen));
        Assert.True(await WaitUntilAsync(() => FocusManager.GetFocusedElement(box.XamlRoot) is TextBox));
    }

    [UnoHeadlessFact]
    public async Task Selection_Triggers_Handle_The_WinUI_Selection_Events()
    {
        var flipView = new FlipView { Items = { "a", "b" } };
        var carousel = new CarouselSelectionChangedTrigger().AttachTo(flipView).Record();
        var tabView = new TabView();
        tabView.TabItems.Add(new TabViewItem { Header = "a" });
        tabView.TabItems.Add(new TabViewItem { Header = "b" });
        var tabs = new TabControlSelectionChangedTrigger().AttachTo(tabView).Record();
        await Session.ShowAsync(new StackPanel { Children = { flipView, tabView } });
        carousel.Parameters.Clear();
        tabs.Parameters.Clear();

        flipView.SelectedIndex = 1;
        tabView.SelectedIndex = 1;

        Assert.True(await WaitUntilAsync(() => carousel.Parameters.Count > 0 && tabs.Parameters.Count > 0));
        Assert.IsType<SelectionChangedEventArgs>(carousel.Parameters[0]);
    }

    [UnoHeadlessFact]
    public async Task ContextDialogBehavior_Opens_And_Closes_A_Popup()
    {
        var target = new Button { Content = "target" };
        var behavior = new ContextDialogBehavior { DialogContent = new TextBlock { Text = "dialog" }, Placement = PopupPlacementMode.Bottom };
        var opened = 0;
        var closed = 0;
        behavior.Opened += (_, _) => opened++;
        behavior.Closed += (_, _) => closed++;
        behavior.AttachTo(target);
        await Session.ShowAsync(target);

        Assert.Equal(true, new ShowContextDialogAction { TargetDialog = behavior }.Execute(null, null));
        Assert.True(behavior.IsOpen);
        Assert.Equal(1, opened);
        Assert.Single(VisualTreeHelper.GetOpenPopupsForXamlRoot(target.XamlRoot), p => p.Child is TextBlock);

        Assert.Equal(true, new HideContextDialogAction { TargetDialog = behavior }.Execute(null, null));
        Assert.False(behavior.IsOpen);
        Assert.True(closed >= 1);
    }

    [UnoHeadlessFact]
    public async Task ScrollToItemIndexBehavior_Scrolls_The_ListView()
    {
        var index = new TestSubject<int>();
        var listView = new ListView { Height = 100, ItemsSource = Enumerable.Range(0, 200).ToList() };
        new ScrollToItemIndexBehavior { ItemIndex = index }.AttachTo(listView);
        await Session.ShowAsync(listView);

        index.OnNext(150);

        Assert.True(await WaitUntilAsync(() => listView.ContainerFromIndex(150) is not null));
    }

    [UnoHeadlessFact]
    public async Task ListBox_Selection_Behaviors_Select_And_Unselect_All()
    {
        var listView = new ListView { SelectionMode = ListViewSelectionMode.Multiple, ItemsSource = new[] { "a", "b", "c" } };
        new ListBoxSelectAllBehavior().AttachTo(listView);
        await Session.ShowAsync(listView);

        Assert.True(await WaitUntilAsync(() => listView.SelectedItems.Count == 3));

        var other = new ListView { SelectionMode = ListViewSelectionMode.Multiple, ItemsSource = new[] { "a", "b" } };
        other.SelectedItems.Add("a");
        new ListBoxUnselectAllBehavior().AttachTo(other);
        await Session.ShowAsync(other);

        Assert.True(await WaitUntilAsync(() => other.SelectedItems.Count == 0));
    }

    [UnoHeadlessFact]
    public async Task ScrollViewer_Behaviors_Scroll_And_Observe_The_Viewport()
    {
        var inside = new Border { Height = 50, Width = 50 };
        var outside = new Border { Height = 50, Width = 50, Margin = new Thickness(0, 800, 0, 0) };
        var scrollViewer = new ScrollViewer { Height = 200, Content = new StackPanel { Children = { inside, outside } } };
        var changed = new ScrollChangedTrigger().AttachTo(scrollViewer).Record();
        var offset = new ScrollViewerOffsetBehavior().AttachTo(scrollViewer);
        var insideViewport = new ViewportBehavior { IsAlwaysOn = true }.AttachTo(inside);
        var outsideViewport = new ViewportBehavior { IsAlwaysOn = true }.AttachTo(outside);
        await Session.ShowAsync(scrollViewer);

        Assert.True(await WaitUntilAsync(() => insideViewport.IsFullyInViewport));
        Assert.False(outsideViewport.IsInViewport);

        offset.VerticalOffset = 700;

        Assert.True(await WaitUntilAsync(() => Math.Abs(scrollViewer.VerticalOffset - 700) < 1));
        Assert.True(await WaitUntilAsync(() => changed.Parameters.Count > 0 && outsideViewport.IsInViewport && !insideViewport.IsInViewport));
    }

    [UnoHeadlessFact]
    public async Task ShowOnTappedBehavior_Shows_The_Target()
    {
        var injector = InputInjector.TryCreate();
        Assert.SkipWhen(injector is null, "Input injection is not available.");
        injector!.InitializeTouchInjection(InjectedInputVisualizationMode.None);

        var target = new TextBox { Visibility = Visibility.Collapsed };
        var button = new Border { Width = 100, Height = 30, Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) };
        new ShowOnTappedBehavior { TargetControl = target }.AttachTo(button);
        await Session.ShowAsync(new StackPanel { Children = { button, target } });

        var position = button.TransformToVisual(null).TransformPoint(new Point(10, 10));
        injector.InjectMouseInput([new InjectedInputMouseInfo { DeltaX = (int)position.X, DeltaY = (int)position.Y, MouseOptions = InjectedInputMouseOptions.Absolute | InjectedInputMouseOptions.Move }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftDown }]);
        injector.InjectMouseInput([new InjectedInputMouseInfo { MouseOptions = InjectedInputMouseOptions.LeftUp }]);

        Assert.True(await WaitUntilAsync(() => target.Visibility == Visibility.Visible));
    }

    [UnoHeadlessFact]
    public async Task TextBox_And_Theme_Behaviors_Apply_On_Attach()
    {
        var textBox = new TextBox { Text = "abc" };
        new TextBoxSelectAllTextBehavior().AttachTo(textBox);
        var border = new Border();
        var theme = new ThemeVariantBehavior { ThemeVariant = ElementTheme.Dark }.AttachTo(border);
        await Session.ShowAsync(new StackPanel { Children = { textBox, border } });

        Assert.True(await WaitUntilAsync(() => textBox.SelectionLength == 3));
        Assert.Equal(ElementTheme.Dark, border.RequestedTheme);

        theme.ThemeVariant = ElementTheme.Light;
        Assert.Equal(ElementTheme.Light, border.RequestedTheme);

        Interaction.GetBehaviors(border).Remove(theme);
        Assert.Equal(ElementTheme.Default, border.RequestedTheme);
    }

    [UnoHeadlessFact]
    public async Task ToolTip_Triggers_Handle_Opened_And_Closed()
    {
        var button = new Button { Content = "b" };
        ToolTipService.SetToolTip(button, "tip");
        var opening = new ToolTipOpeningTrigger().AttachTo(button).Record();
        var closing = new ToolTipClosingTrigger().AttachTo(button).Record();
        await Session.ShowAsync(button);

        new ShowToolTipAction().Execute(button, null);
        Assert.True(await WaitUntilAsync(() => opening.Parameters.Count == 1));

        new HideToolTipAction().Execute(button, null);
        Assert.True(await WaitUntilAsync(() => closing.Parameters.Count == 1));
    }

    [UnoHeadlessFact]
    public async Task WindowStateTrigger_Executes_For_The_Current_State()
    {
        var host = new Border();
        var trigger = new WindowStateTrigger { State = WindowState.Normal }.AttachTo(host);
        var action = trigger.Record();
        await Session.ShowAsync(host);

        Assert.True(await WaitUntilAsync(() => action.Parameters.Count == 1));
        Assert.Equal(WindowState.Normal, action.Parameters[0]);
    }

    private sealed class TestSubject<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = [];

        public void OnNext(T value)
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return DisposableAction.Create(() => _observers.Remove(observer));
        }
    }
}
