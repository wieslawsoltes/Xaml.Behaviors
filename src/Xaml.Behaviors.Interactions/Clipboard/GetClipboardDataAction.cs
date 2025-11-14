// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
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

        try
        {
            var clipboard = Clipboard ?? (visual.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.Clipboard;
            if (clipboard is null)
            {
                return;
            }

            var data = await GetClipboardDataAsync(clipboard).ConfigureAwait(false);
            var resolvedParameter = ResolveParameter(data);

            if (!Command.CanExecute(resolvedParameter))
            {
                return;
            }

            Command.Execute(resolvedParameter);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async Task<object?> GetClipboardDataAsync(IClipboard clipboard)
    {
        var format = Format;
        if (format is null)
        {
            return null;
        }

        if (IsFormat(format, TextFormat))
        {
            return await ClipboardExtensions.TryGetTextAsync(clipboard).ConfigureAwait(false);
        }

        if (IsFormat(format, FilesFormat))
        {
            return await ClipboardExtensions.TryGetFilesAsync(clipboard).ConfigureAwait(false);
        }

        if (IsFormat(format, FileNamesFormat))
        {
            var files = await ClipboardExtensions.TryGetFilesAsync(clipboard).ConfigureAwait(false);
            return files?
                .Select(file => file.TryGetLocalPath())
                .Where(path => path is not null)
                .ToArray();
        }

        var dataTransfer = await clipboard.TryGetDataAsync().ConfigureAwait(false);
        if (dataTransfer is null)
        {
            return null;
        }

        using (dataTransfer)
        {
            // Try to treat the format as an application string identifier first.
            var stringValue = await TryGetApplicationStringAsync(dataTransfer, format).ConfigureAwait(false);
            if (stringValue is not null)
            {
                return stringValue;
            }

            var bytes = await dataTransfer
                .TryGetValueAsync(DataFormat.CreateBytesPlatformFormat(format))
                .ConfigureAwait(false);
            return TryDeserializeBinaryPayload(bytes) ?? bytes;
        }
    }

    private static async Task<string?> TryGetApplicationStringAsync(IAsyncDataTransfer dataTransfer, string format)
    {
        try
        {
            var stringFormat = DataFormat.CreateStringApplicationFormat(format);
            return await dataTransfer.TryGetValueAsync(stringFormat).ConfigureAwait(false);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static bool IsFormat(string format, string expected) =>
        string.Equals(format, expected, StringComparison.OrdinalIgnoreCase);

    private static object? TryDeserializeBinaryPayload(byte[]? bytes)
    {
        if (bytes is null || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return null;
        }

        var signature = SerializedObjectGuid;
        if (bytes.Length < signature.Length)
        {
            return null;
        }

        for (var i = 0; i < signature.Length; i++)
        {
            if (bytes[i] != signature[i])
            {
                return null;
            }
        }

        using var stream = new MemoryStream(bytes);
        stream.Position = signature.Length;
#pragma warning disable SYSLIB0011
        return new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    private static ReadOnlySpan<byte> SerializedObjectGuid =>
    [
        0x96, 0xa7, 0x9e, 0xfd,
        0x13, 0x3b,
        0x70, 0x43,
        0xa6, 0x79, 0x56, 0x10, 0x6b, 0xb2, 0x88, 0xfb
    ];

    private const string TextFormat = "Text";
    private const string FilesFormat = "Files";
    private const string FileNamesFormat = "FileNames";
}
