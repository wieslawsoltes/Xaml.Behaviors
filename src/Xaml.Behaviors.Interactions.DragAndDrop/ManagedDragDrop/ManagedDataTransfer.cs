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
        _payloadFormat = ManagedDataFormatHelper.CreatePayloadFormat(format, payload);
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
    public static IEnumerable<DataFormat> CreateLookupFormats(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        if (IsValidApplicationIdentifier(identifier))
        {
            yield return DataFormat.CreateBytesApplicationFormat(identifier);
            yield return DataFormat.CreateStringApplicationFormat(identifier);
            yield break;
        }

        yield return DataFormat.CreateBytesPlatformFormat(identifier);
        yield return DataFormat.CreateStringPlatformFormat(identifier);
    }

    public static DataFormat CreatePayloadFormat(string identifier, object payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(payload);

        var isApplicationIdentifier = IsValidApplicationIdentifier(identifier);

        return payload is byte[]
            ? isApplicationIdentifier
                ? DataFormat.CreateBytesApplicationFormat(identifier)
                : DataFormat.CreateBytesPlatformFormat(identifier)
            : isApplicationIdentifier
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
