// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Behaviors.Uno.XamlPages;
using Xaml.Behaviors.Uno.XamlPages.Pages;
using Xunit;

namespace Xaml.Behaviors.Uno.Xaml.UnitTests;

/// <summary>
/// Bindings written in XAML for <c>BindingBehavior.Binding</c>, <c>BindingTriggerBehavior.Binding</c> and
/// <c>Condition.Binding</c>: WinUI applies them, the behaviors use the bound value (issue #382).
/// </summary>
public class BindingPageTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(BindingPage Page, PagesViewModel ViewModel)> ShowPageAsync()
    {
        var viewModel = new PagesViewModel();
        var page = new BindingPage { DataContext = viewModel };
        await Session.ShowAsync(page);
        await Session.WaitForIdleAsync();
        return (page, viewModel);
    }

    private static T Find<T>(FrameworkElement root, string name) where T : class
        => Assert.IsType<T>(root.FindName(name), exactMatch: false);

    [UnoHeadlessFact]
    public async Task BindingBehavior_Pushes_The_XBind_Value_To_The_Target()
    {
        var (page, _) = await ShowPageAsync();
        var target = Find<TextBlock>(page, "TargetText");

        Assert.Equal("hello", target.Text);

        Find<TextBox>(page, "SourceBox").Text = "changed";
        await Session.WaitForIdleAsync();

        Assert.Equal("changed", target.Text);
    }

    [UnoHeadlessFact]
    public async Task BindingBehavior_Pushes_The_Binding_Value_To_The_Target()
    {
        var (page, viewModel) = await ShowPageAsync();
        var target = Find<TextBlock>(page, "StatusText");

        Assert.Equal("idle", target.Text);

        viewModel.Status = "busy";
        await Session.WaitForIdleAsync();

        Assert.Equal("busy", target.Text);
    }

    [UnoHeadlessFact]
    public async Task BindingBehavior_Clears_The_Value_When_Detached()
    {
        var (page, _) = await ShowPageAsync();
        var target = Find<TextBlock>(page, "TargetText");
        Assert.Equal("hello", target.Text);

        global::Xaml.Interactivity.Interaction.GetBehaviors(target).Clear();
        await Session.WaitForIdleAsync();

        Assert.Equal(string.Empty, target.Text);
    }

    [UnoHeadlessFact]
    public async Task BindingTriggerBehavior_Compares_The_XBind_Value()
    {
        var (page, _) = await ShowPageAsync();
        var state = Find<TextBlock>(page, "SliderState");
        Assert.Equal("low", state.Text);

        Find<Slider>(page, "Slider").Value = 75;
        await Session.WaitForIdleAsync();

        Assert.Equal("high", state.Text);
    }

    [UnoHeadlessFact]
    public async Task BindingTriggerBehavior_Compares_The_Binding_Value()
    {
        var (page, viewModel) = await ShowPageAsync();
        var state = Find<TextBlock>(page, "CountState");

        viewModel.Count = 2;
        await Session.WaitForIdleAsync();
        Assert.Equal("few", state.Text);

        viewModel.Count = 3;
        await Session.WaitForIdleAsync();
        Assert.Equal("many", state.Text);
    }

    [UnoHeadlessFact]
    public async Task Conditions_Compare_The_Bound_Values()
    {
        var (page, viewModel) = await ShowPageAsync();
        var state = Find<TextBlock>(page, "ConditionState");

        Find<Slider>(page, "Slider").Value = 75;
        await Session.WaitForIdleAsync();
        Assert.Equal("off", state.Text);

        viewModel.Count = 3;
        await Session.WaitForIdleAsync();
        Assert.Equal("on", state.Text);
    }
}
