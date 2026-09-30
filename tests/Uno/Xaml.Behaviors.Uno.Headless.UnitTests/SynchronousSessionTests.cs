// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Behaviors.Uno.Headless.UnitTests;

public class SynchronousSessionTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public void Show_Loads_And_Arranges_Element_Synchronously()
    {
        var border = new Border { Child = new TextBlock { Text = "text" } };

        Session.Show(border);

        Assert.True(border.IsLoaded);
        Assert.True(((TextBlock)border.Child).IsLoaded);
        Assert.Equal(Session.Options.Width / Session.Options.Scale, border.ActualWidth, 0.5);
        Assert.True(((TextBlock)border.Child).ActualHeight > 0);
    }

    [UnoHeadlessFact]
    public void RunJobs_Runs_Queued_Work_Including_Work_It_Queues()
    {
        var order = string.Empty;
        Session.DispatcherQueue.TryEnqueue(() =>
        {
            order += "a";
            Session.DispatcherQueue.TryEnqueue(() => order += "b");
        });

        Assert.True(Session.RunJobs() >= 2);
        Assert.Equal("ab", order);
    }

    [UnoHeadlessFact]
    public void Show_Replaces_Content_And_Unloads_Previous_Element()
    {
        var first = new Border();
        var second = new Border();
        var unloaded = false;
        first.Unloaded += (_, _) => unloaded = true;

        Session.Show(first);
        Session.Show(second);

        Assert.True(unloaded);
        Assert.False(first.IsLoaded);
        Assert.Same(second, Session.Window.Content);
    }

    [UnoHeadlessFact]
    public async Task RunJobs_Throws_Off_The_Ui_Thread()
    {
        await Task.Run(() => Assert.Throws<InvalidOperationException>(() => Session.RunJobs()));
    }
}
