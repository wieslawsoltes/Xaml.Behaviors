// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Interactivity;

namespace Xaml.Interactions.Core;

/// <summary>
/// An action that will set the text to the clipboard.
/// </summary>
public partial class SetClipboardTextAction : StyledElementAction
{
    /// <summary>
    /// Gets or sets the clipboard to use. When not set, <see cref="SystemClipboard"/> is used.
    /// </summary>
    [StyledProperty]
    public partial IClipboard? Clipboard { get; set; }

    /// <summary>
    /// Gets or sets the text to set to the clipboard.
    /// </summary>
    [StyledProperty]
    public partial string? Text { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not UIElement)
        {
            return false;
        }

        _ = Dispatcher.UIThread.InvokeAsync(SetClipboardTextAsync);
        return true;
    }

    private async Task SetClipboardTextAsync()
    {
        if (!IsEnabled || Text is not { } text)
        {
            return;
        }

        try
        {
            await (Clipboard ?? SystemClipboard.Instance).SetTextAsync(text);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
