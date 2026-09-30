// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Core;
using Xunit;

namespace Xaml.Interactions.UnitTests;

public sealed class FakeClipboard : IClipboard
{
    public string? Text { get; set; }

    public DataPackage? Data { get; private set; }

    public Task SetTextAsync(string text)
    {
        Text = text;
        return Task.CompletedTask;
    }

    public Task<string?> GetTextAsync() => Task.FromResult(Text);

    public Task ClearAsync()
    {
        Text = null;
        Data = null;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> GetFormatsAsync()
        => Task.FromResult<IReadOnlyList<string>>(Text is null ? [] : [SystemClipboard.TextFormat]);

    public Task SetDataAsync(DataPackage data)
    {
        Data = data;
        return Task.CompletedTask;
    }

    public Task<object?> GetDataAsync(string format) => Task.FromResult<object?>(format == SystemClipboard.TextFormat ? Text : null);
}

public class ClipboardTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task Set_Get_And_Clear_Text()
    {
        var clipboard = new FakeClipboard();
        var border = new Border();
        var command = new RecordingCommand();

        Assert.True((bool)new SetClipboardTextAction { Clipboard = clipboard, Text = "hello" }.Execute(border, null));
        await Session.WaitForIdleAsync();
        Assert.Equal("hello", clipboard.Text);

        new GetClipboardTextAction { Clipboard = clipboard, Command = command }.Execute(border, null);
        await Session.WaitForIdleAsync();
        Assert.Equal(["hello"], command.Parameters);

        new ClearClipboardAction { Clipboard = clipboard }.Execute(border, null);
        await Session.WaitForIdleAsync();
        Assert.Null(clipboard.Text);
    }

    [UnoHeadlessFact]
    public async Task Formats_And_Data_Are_Passed_To_Command()
    {
        var clipboard = new FakeClipboard { Text = "x" };
        var border = new Border();
        var formats = new RecordingCommand();
        var data = new RecordingCommand();

        new GetClipboardFormatsAction { Clipboard = clipboard, Command = formats }.Execute(border, null);
        new GetClipboardDataAction { Clipboard = clipboard, Command = data, Format = "Text" }.Execute(border, null);
        await Session.WaitForIdleAsync();

        Assert.Equal(new[] { "Text" }, Assert.IsType<string[]>(formats.Parameters[0]));
        Assert.Equal(["x"], data.Parameters);
    }

    [UnoHeadlessFact]
    public async Task SetClipboardDataObject_Uses_Data_Package()
    {
        var clipboard = new FakeClipboard();
        var package = new DataPackage();
        package.SetText("data");

        new SetClipboardDataObjectAction { Clipboard = clipboard, DataTransfer = package }.Execute(new Border(), null);
        await Session.WaitForIdleAsync();

        Assert.Same(package, clipboard.Data);
    }

    [UnoHeadlessFact]
    public void Actions_Require_An_Element_Sender()
    {
        Assert.False((bool)new SetClipboardTextAction { Text = "x" }.Execute(null, null));
    }

    [UnoHeadlessFact]
    public async Task System_Clipboard_Round_Trips_Text_When_Available()
    {
        string? text;
        try
        {
            await SystemClipboard.Instance.SetTextAsync("uno");
            text = await SystemClipboard.Instance.GetTextAsync();
        }
        catch (System.Exception exception)
        {
            Assert.Skip($"The system clipboard is not available headless: {exception.GetType().Name}");
            return;
        }

        Assert.Equal("uno", text);
    }
}
