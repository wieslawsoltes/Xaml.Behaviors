using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

internal sealed class ManagedPayloadDataTransfer : IDataTransfer, IAsyncDataTransfer
{
    private readonly ManagedPayloadDataTransferItem _item;
    private readonly IReadOnlyList<IDataTransferItem> _items;
    private readonly IReadOnlyList<IAsyncDataTransferItem> _asyncItems;

    private ManagedPayloadDataTransfer(ManagedPayloadDataTransferItem item)
    {
        _item = item;
        _items = [item];
        _asyncItems = [item];
    }

    public IReadOnlyList<DataFormat> Formats => _item.Formats;

    public IReadOnlyList<IDataTransferItem> Items => _items;

    IReadOnlyList<IAsyncDataTransferItem> IAsyncDataTransfer.Items => _asyncItems;

    public static IDataTransfer Create(string format, object payload)
    {
        return new ManagedPayloadDataTransfer(new ManagedPayloadDataTransferItem(format, payload));
    }

    public void Dispose()
    {
    }
}

internal sealed class ManagedPayloadDataTransferItem : IDataTransferItem, IAsyncDataTransferItem
{
    private readonly IReadOnlyList<DataFormat> _formats;
    private readonly DataFormat _payloadFormat;
    private readonly object _payload;

    public ManagedPayloadDataTransferItem(string format, object payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentNullException.ThrowIfNull(payload);

        _payload = payload;
        _payloadFormat = ManagedDataFormatHelper.CreateLookupFormat(format);
        _formats = payload is string
            ? [_payloadFormat, DataFormat.Text]
            : [_payloadFormat];
    }

    public IReadOnlyList<DataFormat> Formats => _formats;

    public object? TryGetRaw(DataFormat format)
    {
        if (format == _payloadFormat)
        {
            return _payload;
        }

        if (_payload is string text && format == DataFormat.Text)
        {
            return text;
        }

        return null;
    }

    public Task<object?> TryGetRawAsync(DataFormat format)
    {
        return Task.FromResult(TryGetRaw(format));
    }
}

internal static class ManagedDataFormatHelper
{
    public static DataFormat CreateLookupFormat(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        return IsValidApplicationIdentifier(identifier)
            ? DataFormat.CreateStringApplicationFormat(identifier)
            : DataFormat.CreateStringPlatformFormat(identifier);
    }

    private static bool IsValidApplicationIdentifier(string identifier)
    {
        foreach (char character in identifier)
        {
            if (!char.IsAsciiLetterOrDigit(character) && character != '.' && character != '-')
            {
                return false;
            }
        }

        return true;
    }
}
