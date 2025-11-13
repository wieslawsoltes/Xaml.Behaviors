// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Event args raised after a folder pick operation completes.
/// </summary>
public class FolderPickerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderPickerEventArgs"/> class.
    /// </summary>
    /// <param name="folders">The collection of folders returned by the picker.</param>
    public FolderPickerEventArgs(IReadOnlyList<IStorageFolder> folders)
    {
        Folders = folders;
    }

    /// <summary>
    /// Gets the collection of folders returned by the picker.
    /// </summary>
    public IReadOnlyList<IStorageFolder> Folders { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the pick operation has been handled.
    /// </summary>
    public bool Handled { get; set; }
}
