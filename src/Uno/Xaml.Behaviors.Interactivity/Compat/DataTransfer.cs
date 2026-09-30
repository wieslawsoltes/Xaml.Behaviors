// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>DataFormat</c>: a WinUI <see cref="DataPackage"/> format identifier.
/// </summary>
/// <param name="identifier">The format identifier.</param>
internal class DataFormat(string identifier)
{
    /// <summary>Gets the format identifier.</summary>
    public string Identifier { get; } = identifier;

    /// <summary>Gets the text format.</summary>
    public static DataFormat<string> Text { get; } = new(StandardDataFormats.Text);

    /// <summary>Gets the file (storage items) format.</summary>
    public static DataFormat<IStorageItem[]> File { get; } = new(StandardDataFormats.StorageItems);

    /// <summary>
    /// Creates an application specific string format.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <returns>The format.</returns>
    public static DataFormat<string> CreateStringApplicationFormat(string identifier) => new(identifier);
}

/// <summary>
/// Typed <see cref="DataFormat"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <param name="identifier">The format identifier.</param>
internal sealed class DataFormat<T>(string identifier) : DataFormat(identifier);

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>DataTransferItem</c>.
/// </summary>
internal sealed class DataTransferItem
{
    private DataTransferItem(string identifier, object value)
    {
        Identifier = identifier;
        Value = value;
    }

    /// <summary>Gets the format identifier.</summary>
    public string Identifier { get; }

    /// <summary>Gets the value.</summary>
    public object Value { get; }

    /// <summary>
    /// Creates an item.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="format">The format.</param>
    /// <param name="value">The value.</param>
    /// <returns>The item.</returns>
    public static DataTransferItem Create<T>(DataFormat<T> format, T value) where T : notnull => new(format.Identifier, value);

    /// <summary>
    /// Creates an item for a format identifier.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <param name="value">The value.</param>
    /// <returns>The item.</returns>
    public static DataTransferItem Create(string identifier, object value) => new(identifier, value);
}

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>DataTransfer</c> used to start a drag operation.
/// </summary>
/// <remarks>
/// The items are written to the <see cref="DataPackage"/> of the drag operation, both as data (so targets can test
/// the format with <see cref="DataPackageView.Contains(string)"/>) and as package properties, which WinUI exposes
/// synchronously to in-process drop targets through <see cref="DataPackageView.Properties"/>.
/// </remarks>
internal sealed class DataTransfer
{
    private readonly List<DataTransferItem> _items = [];

    /// <summary>Gets the items.</summary>
    public IReadOnlyList<DataTransferItem> Items => _items;

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(DataTransferItem item) => _items.Add(item);

    /// <summary>
    /// Writes the items to a data package.
    /// </summary>
    /// <param name="package">The package.</param>
    public void ApplyTo(DataPackage package)
    {
        foreach (var item in _items)
        {
            if (item.Identifier == StandardDataFormats.Text && item.Value is string text)
            {
                package.SetText(text);
            }
            else
            {
                package.SetData(item.Identifier, item.Value);
            }

            package.Properties[item.Identifier] = item.Value;
        }
    }
}

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>IDataTransfer</c> read by drop targets (a <see cref="DataPackageView"/>).
/// </summary>
/// <remarks>
/// WinUI data retrieval is asynchronous. The synchronous Avalonia accessors return values that are available
/// immediately: package properties and data that was set directly on the package (always the case for in-process
/// drags on Uno Platform). Delay rendered data that is not available yet is reported as missing.
/// </remarks>
/// <param name="view">The package view.</param>
internal readonly struct DataTransferView(DataPackageView? view)
{
    /// <summary>Gets the underlying package view.</summary>
    public DataPackageView? View => view;

    /// <summary>
    /// Checks whether the data contains a format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns><c>true</c> when the format is available.</returns>
    public bool Contains(DataFormat format) => Contains(format.Identifier);

    /// <summary>
    /// Checks whether the data contains a format.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <returns><c>true</c> when the format is available.</returns>
    public bool Contains(string identifier)
        => view is not null && (view.Contains(identifier) || view.Properties.ContainsKey(identifier));

    /// <summary>
    /// Gets a string value.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The value or <c>null</c>.</returns>
    public string? TryGetValue(DataFormat<string> format) => TryGetValue(format.Identifier) as string;

    /// <summary>
    /// Gets a value.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <returns>The value or <c>null</c>.</returns>
    public object? TryGetValue(string identifier)
    {
        if (view is null)
        {
            return null;
        }

        if (view.Properties.TryGetValue(identifier, out var property) && property is not null)
        {
            return property;
        }

        return view.Contains(identifier) ? GetCompleted(view.GetDataAsync(identifier)) : null;
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text or <c>null</c>.</returns>
    public string? TryGetText()
        => view is not null && view.Contains(StandardDataFormats.Text) ? GetCompleted(view.GetTextAsync()) : null;

    /// <summary>
    /// Gets the dropped files and folders.
    /// </summary>
    /// <returns>The storage items or <c>null</c>.</returns>
    public IStorageItem[]? TryGetFiles()
    {
        if (view is null || !view.Contains(StandardDataFormats.StorageItems))
        {
            return null;
        }

        var items = GetCompleted(view.GetStorageItemsAsync());
        if (items is null)
        {
            return null;
        }

        var result = new IStorageItem[items.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = items[i];
        }

        return result;
    }

    private static T? GetCompleted<T>(IAsyncOperation<T> operation)
    {
        try
        {
            return operation.Status == AsyncStatus.Completed ? operation.GetResults() : default;
        }
        catch (Exception)
        {
            return default;
        }
    }
}

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>DragEventArgs</c> members used by the shared sources.
/// </summary>
internal static class DragEventArgsCompatExtensions
{
    extension(DragEventArgs e)
    {
        /// <summary>Gets the dragged data (Avalonia <c>DataTransfer</c>, WinUI <c>DataView</c>).</summary>
        public DataTransferView DataTransfer => new(e.DataView);

        /// <summary>Gets or sets the accepted operation (Avalonia <c>DragEffects</c>, WinUI <c>AcceptedOperation</c>).</summary>
        public DataPackageOperation DragEffects
        {
            get => e.AcceptedOperation;
            set => e.AcceptedOperation = value;
        }
    }
}
