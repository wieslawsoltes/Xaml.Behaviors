// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xunit;

namespace Xaml.Interactions.Scripting.UnitTests;

public class ExecuteScriptActionTests
{
    // The first script compilation loads Roslyn, which can take a while on a cold machine.
    private const int ScriptTimeoutMilliseconds = 60_000;

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task Execute_RunsTheScriptOnTheUIThreadWithWinUIImports()
    {
        Border sender = new();
        await Session.ShowAsync(sender);
        ExecuteScriptAction action = new()
        {
            // Border and TextBlock resolve through the WinUI imports of the Uno build.
            Script = "var border = (Border)Sender; border.Tag = Parameter; border.Child = new TextBlock { Text = \"ran\" };",
        };

        Assert.True(action.UseDispatcher);
        Assert.Equal(true, action.Execute(sender, "parameter"));

        await WaitUntilAsync(() => sender.Child is TextBlock { Text: "ran" });
        Assert.Equal("parameter", sender.Tag);
    }

    [UnoHeadlessFact]
    public async Task Execute_RunsTheScriptOnTheThreadPoolWithoutDispatcher()
    {
        StrongBox<object?> sender = new();
        ExecuteScriptAction action = new()
        {
            UseDispatcher = false,
            Script = "((System.Runtime.CompilerServices.StrongBox<object>)Sender).Value = Parameter;",
        };

        Assert.Equal(true, action.Execute(sender, 42));

        await WaitUntilAsync(() => Equals(sender.Value, 42));
    }

    [UnoHeadlessFact]
    public void Execute_ReturnsFalseWhenDisabledOrWithoutScript()
    {
        Assert.Equal(false, new ExecuteScriptAction().Execute(null, null));
        Assert.Equal(false, new ExecuteScriptAction { Script = "   " }.Execute(null, null));
        Assert.Equal(false, new ExecuteScriptAction { Script = "1", IsEnabled = false }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task Execute_SwallowsScriptErrors()
    {
        StrongBox<object?> sender = new();
        ExecuteScriptAction broken = new() { UseDispatcher = false, Script = "this is not C#" };
        ExecuteScriptAction working = new()
        {
            UseDispatcher = false,
            Script = "((System.Runtime.CompilerServices.StrongBox<object>)Sender).Value = \"after\";",
        };

        Assert.Equal(true, broken.Execute(sender, null));
        Assert.Equal(true, working.Execute(sender, null));

        await WaitUntilAsync(() => Equals(sender.Value, "after"));
    }

    [UnoHeadlessFact]
    public void Globals_ExposeTheSenderAndParameter()
    {
        object sender = new();
        ExecuteScriptActionGlobals globals = new(sender, "value");

        Assert.Same(sender, globals.Sender);
        Assert.Equal("value", globals.Parameter);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            Assert.True(stopwatch.ElapsedMilliseconds < ScriptTimeoutMilliseconds, "Timed out waiting for the script.");
            await Task.Delay(20);
        }
    }
}
