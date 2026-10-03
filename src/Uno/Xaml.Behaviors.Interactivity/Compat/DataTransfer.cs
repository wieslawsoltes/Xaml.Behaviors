// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    {
        if (view is null)
        {
            return false;
        }

        if (view.Properties.ContainsKey(identifier))
        {
            return true;
        }

        if (!view.Contains(identifier))
        {
            return false;
        }

#if WINUI
        // Native WinUI reads the data asynchronously: the read is started when a target asks for the format (drag
        // enter or over), so that its result is available when the data is dropped.
        _ = GetOperation(view, identifier);
#endif
        return true;
    }

    /// <summary>
    /// Gets a string value.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The value or <c>null</c>.</returns>
    public string? TryGetValue(DataFormat<string> format) => TryGetValue(format.Identifier);

    /// <summary>
    /// Gets a string value of an application format.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <returns>The value or <c>null</c>.</returns>
    public string? TryGetValue(string identifier) => TryGetRaw(identifier) as string;

    /// <summary>
    /// Gets a value.
    /// </summary>
    /// <param name="identifier">The format identifier.</param>
    /// <returns>The value or <c>null</c>.</returns>
    public object? TryGetRaw(string identifier)
    {
        if (view is null)
        {
            return null;
        }

        if (view.Properties.TryGetValue(identifier, out var property) && property is not null)
        {
            return property;
        }

#if WINUI
        return view.Contains(identifier) ? GetCompleted((IAsyncOperation<object>)GetOperation(view, identifier)) : null;
#else
        return view.Contains(identifier) ? GetCompleted(view.GetDataAsync(identifier)) : null;
#endif
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text or <c>null</c>.</returns>
    public string? TryGetText()
#if WINUI
        => view is not null && view.Contains(StandardDataFormats.Text) ? GetCompleted((IAsyncOperation<string>)GetOperation(view, StandardDataFormats.Text)) : null;
#else
        => view is not null && view.Contains(StandardDataFormats.Text) ? GetCompleted(view.GetTextAsync()) : null;
#endif

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

#if WINUI
        var items = GetCompleted((IAsyncOperation<IReadOnlyList<IStorageItem>>)GetOperation(view, StandardDataFormats.StorageItems));
#else
        var items = GetCompleted(view.GetStorageItemsAsync());
#endif
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

#if WINUI
    /// <summary>
    /// Gets the read of a format of the dragged data, starting it on the first call for the data of a drag.
    /// </summary>
    private static object GetOperation(DataPackageView view, string identifier)
    {
        var reads = DragDataReads.For(view);
        if (!reads.TryGetValue(identifier, out var operation))
        {
            operation = identifier == StandardDataFormats.Text ? view.GetTextAsync()
                : identifier == StandardDataFormats.StorageItems ? view.GetStorageItemsAsync()
                : view.GetDataAsync(identifier);
            reads.Add(identifier, operation);
        }

        return operation;
    }

#endif
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
    // Event arguments whose DragEffects were assigned by a handler.
    private static readonly ConditionalWeakTable<DragEventArgs, object> s_assigned = new();

#if WINUI
    // The element that received a drag event (the original source of a native WinUI drag event is the drop target).
    private static readonly ConditionalWeakTable<DragEventArgs, UIElement> s_receivers = new();

    /// <summary>
    /// Records the element whose handler receives a drag event.
    /// </summary>
    /// <param name="e">The drag event arguments.</param>
    /// <param name="receiver">The element.</param>
    internal static void SetReceiver(DragEventArgs e, UIElement? receiver)
    {
        if (receiver is not null)
        {
            s_receivers.AddOrUpdate(e, receiver);
        }
    }

#endif
    extension(DragEventArgs e)
    {
#if WINUI
        /// <summary>
        /// Gets the element that raised the event (Avalonia <c>RoutedEventArgs.Source</c>).
        /// </summary>
        /// <remarks>
        /// The original source of a native WinUI drag event is the drop target (the element that allows the drop),
        /// not the element under the pointer: the source is the element under the pointer in the element that
        /// receives the event.
        /// </remarks>
        public object? Source
            => s_receivers.TryGetValue(e, out var receiver)
                ? receiver.GetVisualAt(e.GetPosition(receiver)) ?? e.OriginalSource ?? receiver
                : e.OriginalSource;

#endif
        /// <summary>Gets the dragged data (Avalonia <c>DataTransfer</c>, WinUI <c>DataView</c>).</summary>
        public DataTransferView DataTransfer => new(e.DataView);

        /// <summary>
        /// Gets or sets the drag effects (Avalonia <c>DragEffects</c>, WinUI <c>AcceptedOperation</c>).
        /// </summary>
        /// <remarks>
        /// Like on Avalonia, the effects start as the operations allowed by the drag source (WinUI
        /// <c>AllowedOperations</c>) until a handler assigns them; assigned effects are the accepted operation.
        /// </remarks>
        public DataPackageOperation DragEffects
        {
            get => s_assigned.TryGetValue(e, out _) ? e.AcceptedOperation : e.AllowedOperations;
            set
            {
                e.AcceptedOperation = value;
                s_assigned.AddOrUpdate(e, s_marker);
            }
        }
    }

    private static readonly object s_marker = new();

    /// <summary>
    /// Applies the Avalonia default of drag enter/over events: the target accepts the operations allowed by the source
    /// unless a handler changes the effects (WinUI targets reject drops unless <c>AcceptedOperation</c> is set).
    /// </summary>
    /// <param name="e">The event arguments.</param>
    internal static void ApplyDefaultDragEffects(DragEventArgs e)
    {
        if (!s_assigned.TryGetValue(e, out _) && e.AcceptedOperation == DataPackageOperation.None)
        {
            e.AcceptedOperation = e.AllowedOperations;
        }
    }
}

#if WINUI
/// <summary>
/// The reads started on the data of the current drag operation (native WinUI, see <see cref="DataTransferView"/>).
/// </summary>
/// <remarks>
/// There is one drag operation at a time: the reads of the previous one are dropped when another data view is used.
/// </remarks>
internal static class DragDataReads
{
    private static DataPackageView? s_view;
    private static Dictionary<string, object> s_reads = [];

    /// <summary>
    /// Gets the reads started on a data view, by format identifier.
    /// </summary>
    /// <param name="view">The data view of a drag event.</param>
    /// <returns>The reads.</returns>
    public static Dictionary<string, object> For(DataPackageView view)
    {
        if (s_view is null || !s_view.Equals(view))
        {
            s_view = view;
            s_reads = [];
        }

        return s_reads;
    }
}
#endif

