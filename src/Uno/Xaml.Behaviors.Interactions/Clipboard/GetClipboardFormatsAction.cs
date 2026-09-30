// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Interactivity;

namespace Xaml.Interactions.Core;

/// <summary>
/// An action that will get the formats available on the clipboard and pass them to the command.
/// </summary>
public partial class GetClipboardFormatsAction : InvokeCommandActionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetClipboardFormatsAction"/> class.
    /// </summary>
    public GetClipboardFormatsAction()
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

        _ = Dispatcher.UIThread.InvokeAsync(GetClipboardFormatsAsync);
        return true;
    }

    private async Task GetClipboardFormatsAsync()
    {
        if (!IsEnabled || Command is null)
        {
            return;
        }

        string[]? formats = null;
        try
        {
            formats = [.. await (Clipboard ?? SystemClipboard.Instance).GetFormatsAsync()];
        }
        catch (Exception)
        {
            // ignored
        }

        var resolvedParameter = ResolveParameter(formats);
        if (Command.CanExecute(resolvedParameter))
        {
            Command.Execute(resolvedParameter);
        }
    }
}
