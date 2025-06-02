// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Collections;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that collects file paths while dragging over the associated control.
/// </summary>
public sealed class FilesPreviewBehavior : DragAndDropEventsBehavior
{
    private AvaloniaList<IStorageItem>? _previewFiles;

    /// <summary>
    /// Identifies the <seealso cref="PreviewFiles"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<FilesPreviewBehavior, AvaloniaList<IStorageItem>> PreviewFilesProperty =
        AvaloniaProperty.RegisterDirect<FilesPreviewBehavior, AvaloniaList<IStorageItem>>(nameof(PreviewFiles), b => b.PreviewFiles);

    /// <summary>
    /// Gets the collection of file paths currently previewed. This is an avalonia property.
    /// </summary>
    public AvaloniaList<IStorageItem> PreviewFiles => _previewFiles ??= [];

    /// <inheritdoc />
    protected override void OnDragEnter(object? sender, DragEventArgs e)
    {
        UpdatePreview(e);
    }

    /// <inheritdoc />
    protected override void OnDragLeave(object? sender, DragEventArgs e)
    {
        ClearPreview();
    }

    /// <inheritdoc />
    protected override void OnDragOver(object? sender, DragEventArgs e)
    {
        UpdatePreview(e);
    }

    /// <inheritdoc />
    protected override void OnDrop(object? sender, DragEventArgs e)
    {
        ClearPreview();
    }

    private void UpdatePreview(DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            return;
        }

        var files = e.Data.GetFiles();
        if (files is null)
        {
            return;
        }

        var list = PreviewFiles;
        list.Clear();
        foreach (var file in files)
        {
            list.Add(file);
        }
    }

    private void ClearPreview()
    {
        _previewFiles?.Clear();
    }
}
