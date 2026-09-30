// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Interactivity;

namespace Xaml.Interactions.Core;

/// <summary>
/// An action that will get the text from the clipboard and pass it to the command.
/// </summary>
public partial class GetClipboardTextAction : InvokeCommandActionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetClipboardTextAction"/> class.
    /// </summary>
    public GetClipboardTextAction()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Gets or sets the clipboard to use. When not set, <see cref="SystemClipboard"/> is used.
    /// </summary>
    [StyledProperty]
    public partial IClipboard? Clipboard { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not UIElement)
        {
            return false;
        }

        _ = Dispatcher.UIThread.InvokeAsync(GetClipboardTextAsync);
        return true;
    }

    private async Task GetClipboardTextAsync()
    {
        if (!IsEnabled || Command is null)
        {
            return;
        }

        string? text = null;
        try
        {
            text = await (Clipboard ?? SystemClipboard.Instance).GetTextAsync();
        }
        catch (Exception)
        {
            // ignored
        }

        var resolvedParameter = ResolveParameter(text);
        if (Command.CanExecute(resolvedParameter))
        {
            Command.Execute(resolvedParameter);
        }
    }
}
