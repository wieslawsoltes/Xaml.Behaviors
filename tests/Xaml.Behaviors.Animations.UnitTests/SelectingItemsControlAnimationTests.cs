// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Xaml.Behaviors.Animations.UnitTests;

public class SelectingItemsControlAnimationTests
{
    [AvaloniaFact]
    public void EnableSelectionAnimation_AttachedPropertyRoundTrips()
    {
        var target = new ListBox();

        SelectingItemsControlBehavior.SetEnableSelectionAnimation(target, true);

        Assert.True(SelectingItemsControlBehavior.GetEnableSelectionAnimation(target));
    }

    [AvaloniaFact]
    public void SelectionIndicatorAnimation_InstallsImplicitAnimationForTemplatedContainers()
    {
        var oldIndicator = new Border { Name = "PART_SelectedPipe", Width = 4d, Height = 30d };
        var newIndicator = new Border { Name = "PART_SelectedPipe", Width = 4d, Height = 30d };
        var oldSelection = new ContentControl { Content = oldIndicator, Height = 40d };
        var newSelection = new ContentControl { Content = newIndicator, Height = 40d };
        var panel = new StackPanel { Children = { oldSelection, newSelection } };
        var window = new Window { Content = panel };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        bool started = SelectionIndicatorAnimation.TryStart(
            newSelection,
            oldSelection,
            SelectionIndicatorAnimation.DefaultDuration);

        Assert.True(started);
        Assert.NotNull(ElementComposition.GetElementVisual(newIndicator)?.ImplicitAnimations);
        window.Close();
    }

    [AvaloniaFact]
    public void SelectionIndicatorAnimation_RejectsNegativeDurationAndMissingVisuals()
    {
        Assert.Equal(TimeSpan.FromMilliseconds(250), SelectionIndicatorAnimation.DefaultDuration);
        Assert.False(SelectionIndicatorAnimation.TryStart(
            new ContentControl(),
            new ContentControl(),
            TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => SelectionIndicatorAnimation.TryStart(
            new ContentControl(),
            new ContentControl(),
            TimeSpan.FromMilliseconds(-1d)));
    }

    [AvaloniaFact]
    public void SelectionIndicatorAnimation_ReturnsFalseForZeroDurationWithAvailableVisuals()
    {
        var oldIndicator = new Border { Width = 4d, Height = 30d };
        var newIndicator = new Border { Width = 4d, Height = 30d };
        var oldSelection = new ContentControl { Content = oldIndicator, Height = 40d };
        var newSelection = new ContentControl { Content = newIndicator, Height = 40d };
        var panel = new StackPanel { Children = { oldSelection, newSelection } };
        var window = new Window { Content = panel };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        try
        {
            Assert.False(SelectionIndicatorAnimation.TryStart(
                newIndicator,
                oldIndicator,
                newSelection,
                oldSelection,
                TimeSpan.Zero));
        }
        finally
        {
            window.Close();
        }
    }
}
