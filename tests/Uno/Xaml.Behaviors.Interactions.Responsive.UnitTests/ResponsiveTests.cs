// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Responsive;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Responsive.UnitTests;

public class ResponsiveTests
{
    private const string StatesXaml =
        """
        <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="Layout">
              <VisualState x:Name="Normal" />
              <VisualState x:Name="wide" />
            </VisualStateGroup>
            <VisualStateGroup x:Name="Orientation">
              <VisualState x:Name="Landscape" />
              <VisualState x:Name="NotLandscape" />
            </VisualStateGroup>
          </VisualStateManager.VisualStateGroups>
        </Grid>
        """;

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static (UserControl Host, Grid Root) CreateHost(double width, double height)
    {
        var root = (Grid)XamlReader.Load(StatesXaml);
        var host = new UserControl
        {
            Content = root,
            Width = width,
            Height = height,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };
        return (host, root);
    }

    private static string? CurrentState(FrameworkElement root, string group)
        => VisualStateManager.GetVisualStateGroups(root).Single(g => g.Name == group).CurrentState?.Name;

    [UnoHeadlessFact]
    public async Task AdaptiveBehavior_Adds_And_Removes_Class_When_Width_Changes()
    {
        var (host, root) = CreateHost(300, 100);
        var behavior = new AdaptiveBehavior();
        behavior.Setters.Add(new AdaptiveClassSetter { MinWidth = 200, ClassName = "wide" });
        Interaction.GetBehaviors(host).Add(behavior);

        await Session.ShowAsync(new Grid { Children = { host } });
        await Session.WaitForIdleAsync();

        Assert.True(host.Classes.Contains("wide"));
        Assert.Equal("wide", CurrentState(root, "Layout"));

        host.Width = 100;
        await Session.WaitForIdleAsync();
        await Session.WaitForIdleAsync();

        Assert.False(host.Classes.Contains("wide"));
        Assert.Equal("Normal", CurrentState(root, "Layout"));
    }

    [UnoHeadlessFact]
    public async Task AdaptiveBehavior_Uses_Height_Conditions_And_TargetControl()
    {
        var (host, _) = CreateHost(100, 300);
        var target = new Border();
        var behavior = new AdaptiveBehavior { TargetControl = target };
        behavior.Setters.Add(new AdaptiveClassSetter { MinHeight = 250, ClassName = "tall" });
        behavior.Setters.Add(new AdaptiveClassSetter { MaxHeight = 250, ClassName = "short" });
        Interaction.GetBehaviors(host).Add(behavior);

        await Session.ShowAsync(new Grid { Children = { host, target } });
        await Session.WaitForIdleAsync();

        Assert.True(target.Classes.Contains("tall"));
        Assert.False(target.Classes.Contains("short"));
        Assert.False(host.Classes.Contains("tall"));
    }

    [UnoHeadlessFact]
    public async Task AdaptiveBehavior_Without_Conditions_Removes_Class()
    {
        var (host, _) = CreateHost(100, 100);
        host.Classes.Add("unconditional");
        var behavior = new AdaptiveBehavior();
        behavior.Setters.Add(new AdaptiveClassSetter { ClassName = "unconditional" });
        Interaction.GetBehaviors(host).Add(behavior);

        await Session.ShowAsync(new Grid { Children = { host } });
        await Session.WaitForIdleAsync();

        Assert.False(host.Classes.Contains("unconditional"));
    }

    [UnoHeadlessFact]
    public async Task AspectRatioBehavior_Maps_Pseudo_Class_To_Visual_States()
    {
        var (host, root) = CreateHost(300, 100);
        var behavior = new AspectRatioBehavior();
        behavior.Setters.Add(new AspectRatioClassSetter { MinRatio = 1.5, ClassName = ":landscape", IsPseudoClass = true });
        Interaction.GetBehaviors(host).Add(behavior);

        await Session.ShowAsync(new Grid { Children = { host } });
        await Session.WaitForIdleAsync();

        Assert.True(host.Classes.Contains(":landscape"));
        Assert.Equal("Landscape", CurrentState(root, "Orientation"));

        host.Width = 100;
        host.Height = 300;
        await Session.WaitForIdleAsync();
        await Session.WaitForIdleAsync();

        Assert.False(host.Classes.Contains(":landscape"));
        Assert.Equal("NotLandscape", CurrentState(root, "Orientation"));
    }

    [UnoHeadlessFact]
    public async Task Detached_Behavior_Stops_Observing()
    {
        var (host, _) = CreateHost(300, 100);
        var behavior = new AdaptiveBehavior();
        behavior.Setters.Add(new AdaptiveClassSetter { MinWidth = 200, ClassName = "wide" });
        Interaction.GetBehaviors(host).Add(behavior);
        await Session.ShowAsync(new Grid { Children = { host } });
        await Session.WaitForIdleAsync();
        Assert.True(host.Classes.Contains("wide"));

        Interaction.GetBehaviors(host).Remove(behavior);
        host.Width = 100;
        await Session.WaitForIdleAsync();

        Assert.True(host.Classes.Contains("wide"));
    }

    [UnoHeadlessFact]
    public void ClassSetter_Defaults_Match_Avalonia()
    {
        var adaptive = new AdaptiveClassSetter();
        var ratio = new AspectRatioClassSetter();

        Assert.Equal(ComparisonConditionType.GreaterThanOrEqual, adaptive.MinWidthOperator);
        Assert.Equal(ComparisonConditionType.LessThan, adaptive.MaxWidthOperator);
        Assert.Equal(double.PositiveInfinity, adaptive.MaxWidth);
        Assert.Equal(double.PositiveInfinity, adaptive.MaxHeight);
        Assert.Equal(ComparisonConditionType.GreaterThanOrEqual, ratio.MinRatioOperator);
        Assert.Equal(double.PositiveInfinity, ratio.MaxRatio);
    }

    [UnoHeadlessFact]
    public async Task StyleClasses_On_Plain_Element_Are_Tracked_Without_Visual_States()
    {
        var border = new Border();
        await Session.ShowAsync(new Grid { Children = { border } });

        border.Classes.Add("a");
        ((IPseudoClasses)border.Classes).Add(":b");

        Assert.True(border.Classes.Contains("a"));
        Assert.True(border.Classes.Contains(":b"));
        Assert.True(border.Classes.Remove("a"));
        Assert.False(border.Classes.Remove("a"));
    }
}
