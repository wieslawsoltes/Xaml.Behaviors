// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Xaml.Interactivity;

namespace Xaml.Interactions.Core;

/// <summary>
/// An action that will get data of a format from the clipboard and pass it to the command.
/// </summary>
/// <remarks>
/// Supported formats: <c>Text</c>, <c>Files</c> (storage items), <c>FileNames</c> (paths) and platform format
/// identifiers.
/// </remarks>
public partial class GetClipboardDataAction : InvokeCommandActionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetClipboardDataAction"/> class.
    /// </summary>
    public GetClipboardDataAction()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Gets or sets the clipboard to use. When not set, <see cref="SystemClipboard"/> is used.
    /// </summary>
    [StyledProperty]
    public partial IClipboard? Clipboard { get; set; }

    /// <summary>
    /// Gets or sets the format to get from the clipboard.
    /// </summary>
    [StyledProperty]
    public partial string? Format { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not UIElement)
        {
            return false;
        }

        _ = Dispatcher.UIThread.InvokeAsync(GetClipboardDataAsync);
        return true;
    }

    private async Task GetClipboardDataAsync()
    {
        if (!IsEnabled || Command is null || Format is not { } format)
        {
            return;
        }

        try
        {
            var data = await (Clipboard ?? SystemClipboard.Instance).GetDataAsync(format);
            var resolvedParameter = ResolveParameter(data);
            if (Command.CanExecute(resolvedParameter))
            {
                Command.Execute(resolvedParameter);
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
