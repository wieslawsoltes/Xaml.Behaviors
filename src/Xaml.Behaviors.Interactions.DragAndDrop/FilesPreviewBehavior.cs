// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Storage;
#else
using Avalonia.Collections;
using Avalonia.Input;
using Avalonia.Platform.Storage;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that collects file paths while dragging over the associated control.
/// </summary>
public sealed partial class FilesPreviewBehavior : DragAndDropEventsBehavior
{
    /// <summary>
    /// Gets the collection of file paths currently previewed. This is an avalonia property.
    /// </summary>
    [DirectProperty(Lazy = true)]
#if UNO
    public partial ObservableCollection<IStorageItem> PreviewFiles { get; }
#else
    public partial AvaloniaList<IStorageItem> PreviewFiles { get; }
#endif

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
        if (!e.DataTransfer.Contains(DataFormat.File))
        {
            return;
        }

        var files = e.DataTransfer.TryGetFiles();
        if (files is null || files.Length == 0)
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
        PreviewFiles.Clear();
    }
}
