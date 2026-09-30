// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Xaml.Interactions.Core;

/// <summary>
/// Opens file and folder pickers for the storage provider actions and behaviors.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia <c>IStorageProvider</c>. The default implementation is
/// <see cref="SystemStorageProvider"/> (Windows.Storage.Pickers); assign another implementation (for example in tests)
/// through the <c>StorageProvider</c> property.
/// </remarks>
public interface IStorageProvider
{
    /// <summary>Opens a file open picker.</summary>
    /// <param name="options">The picker options.</param>
    /// <returns>The picked files (empty when the picker is cancelled).</returns>
    Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);

    /// <summary>Opens a file save picker.</summary>
    /// <param name="options">The picker options.</param>
    /// <returns>The picked file, or <c>null</c> when the picker is cancelled.</returns>
    Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options);

    /// <summary>Opens a folder picker.</summary>
    /// <param name="options">The picker options.</param>
    /// <returns>The picked folders (empty when the picker is cancelled).</returns>
    Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);

    /// <summary>Gets a folder from a file system path.</summary>
    /// <param name="folderPath">The folder URI.</param>
    /// <returns>The folder, or <c>null</c> when it does not exist or is not accessible.</returns>
    Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath);
}
