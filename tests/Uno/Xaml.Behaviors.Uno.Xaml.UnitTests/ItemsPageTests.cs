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
/// Item factories declared in XAML for <c>AddItemToItemsControlAction</c> and <c>InsertItemToItemsControlAction</c>
/// (issue #383).
/// </summary>
public class ItemsPageTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static T Find<T>(FrameworkElement root, string name) where T : class
        => Assert.IsType<T>(root.FindName(name), exactMatch: false);

    private static void Click(Button button) => new ButtonAutomationPeer(button).Invoke();

    [UnoHeadlessFact]
    public async Task Item_Actions_Create_A_New_Item_Per_Click()
    {
        var viewModel = new PagesViewModel();
        var page = new ItemsPage { DataContext = viewModel };
        await Session.ShowAsync(page);
        await Session.WaitForIdleAsync();

        Click(Find<Button>(page, "AddButton"));
        Click(Find<Button>(page, "AddButton"));
        Click(Find<Button>(page, "InsertButton"));
        await Session.WaitForIdleAsync();

        Assert.Equal(3, viewModel.Items.Count);
        Assert.Equal("inserted", Assert.IsType<PagesItem>(viewModel.Items[0]).Text);
        Assert.Equal("added", Assert.IsType<PagesItem>(viewModel.Items[1]).Text);
        Assert.Equal("added", Assert.IsType<PagesItem>(viewModel.Items[2]).Text);
        Assert.NotSame(viewModel.Items[1], viewModel.Items[2]);
        Assert.Equal(3, Find<ItemsControl>(page, "Items").Items.Count);
    }
}
