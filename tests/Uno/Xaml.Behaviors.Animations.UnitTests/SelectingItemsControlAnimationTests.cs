// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class SelectingItemsControlAnimationTests
{
    private const string IndicatorName = "PART_SelectedPipe";

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void EnableSelectionAnimation_AttachedPropertyRoundTrips()
    {
        ListView target = new();

        Assert.False(SelectingItemsControlBehavior.GetEnableSelectionAnimation(target));
        SelectingItemsControlBehavior.SetEnableSelectionAnimation(target, true);

        Assert.True(SelectingItemsControlBehavior.GetEnableSelectionAnimation(target));
    }

    [UnoHeadlessFact]
    public async Task SelectionIndicatorAnimation_MovesTheIndicatorFromThePreviousContainer()
    {
        Border oldIndicator = CreateIndicator();
        Border newIndicator = CreateIndicator();
        ContentControl oldSelection = new() { Content = oldIndicator, Height = 40d };
        ContentControl newSelection = new() { Content = newIndicator, Height = 40d };
        await Session.ShowAsync(new StackPanel { Children = { oldSelection, newSelection } });
        await Session.WaitForIdleAsync();

        bool started = SelectionIndicatorAnimation.TryStart(newSelection, oldSelection, TimeSpan.FromSeconds(10));

        Assert.True(started);
        Visual indicatorVisual = TestHelpers.GetVisual(newIndicator);
        Assert.Equal(CompositionGetValueStatus.Succeeded, indicatorVisual.Properties.TryGetVector3("Translation", out Vector3 translation));
        // The indicator starts at the previous container (40 pixels above) and moves to its own position.
        TestHelpers.AssertNear(new Vector3(0f, -40f, 0f), translation, 1f);
    }

    [UnoHeadlessFact]
    public async Task SelectionIndicatorAnimation_CompletesAtTheIndicatorPosition()
    {
        Border oldIndicator = CreateIndicator();
        Border newIndicator = CreateIndicator();
        ContentControl oldSelection = new() { Content = oldIndicator, Height = 40d };
        ContentControl newSelection = new() { Content = newIndicator, Height = 40d };
        await Session.ShowAsync(new StackPanel { Children = { oldSelection, newSelection } });
        await Session.WaitForIdleAsync();

        Assert.True(SelectionIndicatorAnimation.TryStart(newSelection, oldSelection, TimeSpan.FromMilliseconds(60)));

        Visual indicatorVisual = TestHelpers.GetVisual(newIndicator);
        await TestHelpers.WaitUntilAsync(
            () => indicatorVisual.Properties.TryGetVector3("Translation", out Vector3 translation) == CompositionGetValueStatus.Succeeded
                && translation == Vector3.Zero
                && indicatorVisual.Scale == Vector3.One,
            "the indicator reached its position");
    }

    [UnoHeadlessFact]
    public void SelectionIndicatorAnimation_RejectsNegativeDurationAndMissingVisuals()
    {
        Assert.Equal(TimeSpan.FromMilliseconds(250), SelectionIndicatorAnimation.DefaultDuration);
        Assert.False(SelectionIndicatorAnimation.TryStart(new ContentControl(), new ContentControl(), TimeSpan.Zero));
        Assert.False(SelectionIndicatorAnimation.TryStart(new ContentControl(), new ContentControl(), TimeSpan.FromSeconds(1)));
        Assert.False(SelectionIndicatorAnimation.TryStart(null, new ContentControl(), TimeSpan.FromSeconds(1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => SelectionIndicatorAnimation.TryStart(
            new ContentControl(),
            new ContentControl(),
            TimeSpan.FromMilliseconds(-1d)));
    }

    [UnoHeadlessFact]
    public async Task SelectionIndicatorAnimation_ReturnsFalseForZeroDurationWithAvailableVisuals()
    {
        Border oldIndicator = new() { Width = 4d, Height = 30d };
        Border newIndicator = new() { Width = 4d, Height = 30d };
        ContentControl oldSelection = new() { Content = oldIndicator, Height = 40d };
        ContentControl newSelection = new() { Content = newIndicator, Height = 40d };
        await Session.ShowAsync(new StackPanel { Children = { oldSelection, newSelection } });

        Assert.False(SelectionIndicatorAnimation.TryStart(newIndicator, oldIndicator, newSelection, oldSelection, TimeSpan.Zero));
    }

    [UnoHeadlessFact]
    public async Task EnableSelectionAnimation_AnimatesTheIndicatorOnSelectionChange()
    {
        Border firstIndicator = CreateIndicator();
        Border secondIndicator = CreateIndicator();
        ListViewItem first = new() { Content = firstIndicator, Height = 40d };
        ListViewItem second = new() { Content = secondIndicator, Height = 40d };
        // The headless test application has no theme resources: give the list a minimal template.
        ListView listView = (ListView)XamlReader.Load(
            """
            <ListView xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
              <ListView.Template>
                <ControlTemplate TargetType="ListView">
                  <ItemsPresenter />
                </ControlTemplate>
              </ListView.Template>
              <ListView.ItemsPanel>
                <ItemsPanelTemplate>
                  <StackPanel />
                </ItemsPanelTemplate>
              </ListView.ItemsPanel>
            </ListView>
            """);
        listView.Items.Add(first);
        listView.Items.Add(second);
        listView.SelectedIndex = 0;
        SelectingItemsControlBehavior.SetEnableSelectionAnimation(listView, true);
        await Session.ShowAsync(listView);
        await Session.WaitForIdleAsync();

        listView.SelectedIndex = 1;

        Visual indicatorVisual = TestHelpers.GetVisual(secondIndicator);
        Assert.Equal(CompositionGetValueStatus.Succeeded, indicatorVisual.Properties.TryGetVector3("Translation", out Vector3 translation));
        // The indicator starts at the previous container (40 pixels above) and moves to its own position.
        TestHelpers.AssertNear(new Vector3(0f, -40f, 0f), translation, 1f);

        SelectingItemsControlBehavior.SetEnableSelectionAnimation(listView, false);
        listView.SelectedIndex = 0;

        // Disabled: the first indicator is not moved from the second container.
        TestHelpers.GetVisual(firstIndicator).Properties.TryGetVector3("Translation", out Vector3 firstTranslation);
        Assert.Equal(Vector3.Zero, firstTranslation);
    }

    private static Border CreateIndicator() => new() { Name = IndicatorName, Width = 4d, Height = 30d };
}
