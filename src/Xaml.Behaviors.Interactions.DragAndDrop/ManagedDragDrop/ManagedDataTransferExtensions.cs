using System;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides helpers to retrieve managed drag-drop payloads from Avalonia 12 data transfer objects.
/// </summary>
public static class ManagedDataTransferExtensions
{
    /// <summary>
    /// Tries to get a managed payload for the specified application format.
    /// </summary>
    /// <param name="dataTransfer">The drag-drop payload source.</param>
    /// <param name="format">The managed application format identifier.</param>
    /// <returns>The stored payload, or <see langword="null"/> when the format is not present.</returns>
    public static object? TryGetManagedValue(this IDataTransfer dataTransfer, string format)
    {
        ArgumentNullException.ThrowIfNull(dataTransfer);

        DataFormat managedFormat = ManagedDataFormatHelper.CreateLookupFormat(format);

        foreach (IDataTransferItem item in dataTransfer.Items)
        {
            object? value = item.TryGetRaw(managedFormat);
            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to get a managed payload for the specified application format and cast it to the requested type.
    /// </summary>
    /// <typeparam name="T">The expected payload type.</typeparam>
    /// <param name="dataTransfer">The drag-drop payload source.</param>
    /// <param name="format">The managed application format identifier.</param>
    /// <returns>The stored payload cast to <typeparamref name="T"/>, or <see langword="null"/> when unavailable.</returns>
    public static T? TryGetManagedValue<T>(this IDataTransfer dataTransfer, string format)
        where T : class
    {
        return dataTransfer.TryGetManagedValue(format) as T;
    }

    /// <summary>
    /// Tries to get a managed payload for the specified application format from a single data transfer item.
    /// </summary>
    /// <param name="dataTransferItem">The drag-drop payload item.</param>
    /// <param name="format">The managed application format identifier.</param>
    /// <returns>The stored payload, or <see langword="null"/> when the format is not present.</returns>
    public static object? TryGetManagedValue(this IDataTransferItem dataTransferItem, string format)
    {
        ArgumentNullException.ThrowIfNull(dataTransferItem);
        return dataTransferItem.TryGetRaw(ManagedDataFormatHelper.CreateLookupFormat(format));
    }

    /// <summary>
    /// Tries to get a managed payload for the specified application format from a single data transfer item and cast it to the requested type.
    /// </summary>
    /// <typeparam name="T">The expected payload type.</typeparam>
    /// <param name="dataTransferItem">The drag-drop payload item.</param>
    /// <param name="format">The managed application format identifier.</param>
    /// <returns>The stored payload cast to <typeparamref name="T"/>, or <see langword="null"/> when unavailable.</returns>
    public static T? TryGetManagedValue<T>(this IDataTransferItem dataTransferItem, string format)
        where T : class
    {
        return dataTransferItem.TryGetManagedValue(format) as T;
    }
}
