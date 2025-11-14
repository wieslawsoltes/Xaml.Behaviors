// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that will get the data from the clipboard.
/// </summary>
public class GetClipboardDataAction : InvokeCommandActionBase
{
    /// <summary>
    /// Identifies the <seealso cref="Clipboard"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IClipboard?> ClipboardProperty =
        AvaloniaProperty.Register<GetClipboardDataAction, IClipboard?>(nameof(Clipboard));

    /// <summary>
    /// Identifies the <seealso cref="Format"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FormatProperty =
        AvaloniaProperty.Register<GetClipboardDataAction, string?>(nameof(Format));

    /// <summary>
    /// Gets or sets the clipboard to use. This is an avalonia property.
    /// </summary>
    public IClipboard? Clipboard
    {
        get => GetValue(ClipboardProperty);
        set => SetValue(ClipboardProperty, value);
    }

    /// <summary>
    /// Gets or sets the format to get from the clipboard. This is an avalonia property.
    /// </summary>
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetClipboardDataAction"/> class.
    /// </summary>
    public GetClipboardDataAction()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the command is successfully executed; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }

        Dispatcher.UIThread.InvokeAsync(async () => await GetClipboardDataAsync(visual));

        return true;
    }

    private async Task GetClipboardDataAsync(Visual visual)
    {
        if (IsEnabled != true || Command is null || Format is null)
        {
            return;
        }

        object? data = null;

        try
        {
            var clipboard = Clipboard ?? (visual.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Clipboard;
            if (clipboard is null)
            {
                return;
            }

            var dataFormat = string.Equals(Format, "Text", StringComparison.OrdinalIgnoreCase)
                ? DataFormat.Text
                : DataFormat.CreateStringApplicationFormat(Format);
            data = await ClipboardExtensions.TryGetValueAsync<string>(clipboard, dataFormat);
        }
        catch (Exception)
        {
            // ignored
        }

        var resolvedParameter = ResolveParameter(data);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
