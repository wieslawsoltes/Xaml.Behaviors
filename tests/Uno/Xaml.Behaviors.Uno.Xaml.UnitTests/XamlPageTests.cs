// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Behaviors.Uno.XamlPages;
using Xaml.Behaviors.Uno.XamlPages.Pages;
using Xunit;

namespace Xaml.Behaviors.Uno.Xaml.UnitTests;

/// <summary>
/// Behaviors declared in XAML pages compiled by the Uno XAML generator.
/// </summary>
public class XamlPageTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(BehaviorsPage Page, PagesViewModel ViewModel)> ShowPageAsync()
    {
        var viewModel = new PagesViewModel();
        var page = new BehaviorsPage { DataContext = viewModel };
        await Session.ShowAsync(page);
        await Session.WaitForIdleAsync();
        return (page, viewModel);
    }

    private static T Find<T>(FrameworkElement root, string name) where T : class
        => Assert.IsType<T>(root.FindName(name), exactMatch: false);

    private static void Click(Button button) => new ButtonAutomationPeer(button).Invoke();

    [UnoHeadlessFact]
    public async Task EventTrigger_Invokes_Bound_Command_With_Parameter()
    {
        var (page, viewModel) = await ShowPageAsync();

        Click(Find<Button>(page, "IncrementButton"));

        Assert.Equal(1, viewModel.Count);
        Assert.Equal(["from-xaml"], viewModel.Parameters);
    }

    [UnoHeadlessFact]
    public async Task CallMethodAction_Calls_Method_On_Bound_Target()
    {
        var (page, viewModel) = await ShowPageAsync();
        viewModel.Count = 5;

        Click(Find<Button>(page, "ResetButton"));

        Assert.Equal(0, viewModel.Count);
    }

    [UnoHeadlessFact]
    public async Task ChangePropertyAction_Targets_Named_Element()
    {
        var (page, _) = await ShowPageAsync();

        Click(Find<Button>(page, "ChangeButton"));

        Assert.Equal(42d, Find<Border>(page, "Target").Width);
    }

    [UnoHeadlessFact]
    public async Task DataTrigger_Reacts_To_Bound_Value()
    {
        var (page, viewModel) = await ShowPageAsync();
        var text = Find<TextBlock>(page, "StatusText");
        Assert.Equal("idle", text.Text);

        viewModel.Count = 2;
        await Session.WaitForIdleAsync();

        Assert.Equal("many", text.Text);
    }

    [UnoHeadlessFact]
    public async Task Style_Behaviors_Template_Creates_Behaviors_Per_Element()
    {
        var viewModel = new PagesViewModel();
        var page = new TemplatePage { DataContext = viewModel };
        await Session.ShowAsync(page);
        await Session.WaitForIdleAsync();
        var first = Find<Button>(page, "First");
        var second = Find<Button>(page, "Second");

        Click(first);
        Click(second);

        Assert.Equal(2, viewModel.Count);
        Assert.Equal(["styled", "styled"], viewModel.Parameters);
        Assert.NotSame(global::Xaml.Interactivity.Interaction.GetBehaviors(first), global::Xaml.Interactivity.Interaction.GetBehaviors(second));
    }

    [UnoHeadlessFact]
    public async Task Events_Trigger_Declared_In_Xaml_Fires()
    {
        var (page, viewModel) = await ShowPageAsync();

        Find<TextBox>(page, "FocusBox").Focus(FocusState.Programmatic);
        await Session.WaitForIdleAsync();

        Assert.Contains("focus", viewModel.Parameters);
    }
}
