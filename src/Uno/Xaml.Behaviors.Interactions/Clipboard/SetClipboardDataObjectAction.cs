// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Interactivity;

namespace Xaml.Interactions.Core;

/// <summary>
/// An action that will set a data package to the clipboard.
/// </summary>
public partial class SetClipboardDataObjectAction : StyledElementAction
{
    /// <summary>
    /// Gets or sets the clipboard to use. When not set, <see cref="SystemClipboard"/> is used.
    /// </summary>
    [StyledProperty]
    public partial IClipboard? Clipboard { get; set; }

    /// <summary>
    /// Gets or sets the data to set to the clipboard (Avalonia: <c>IAsyncDataTransfer</c>).
    /// </summary>
    [StyledProperty]
    public partial DataPackage? DataTransfer { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not UIElement)
        {
            return false;
        }

        _ = Dispatcher.UIThread.InvokeAsync(SetClipboardDataObjectAsync);
        return true;
    }

    private async Task SetClipboardDataObjectAsync()
    {
        if (!IsEnabled || DataTransfer is not { } data)
        {
            return;
        }

        try
        {
            await (Clipboard ?? SystemClipboard.Instance).SetDataAsync(data);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
