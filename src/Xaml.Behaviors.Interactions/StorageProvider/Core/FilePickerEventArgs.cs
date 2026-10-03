// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
#if UNO
using Windows.Storage;
#else
using Avalonia.Platform.Storage;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// Event args raised after a file pick operation completes.
/// </summary>
public class FilePickerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilePickerEventArgs"/> class.
    /// </summary>
    /// <param name="files">The collection of files returned by the picker.</param>
    public FilePickerEventArgs(IReadOnlyList<IStorageFile> files)
    {
        Files = files;
    }

    /// <summary>
    /// Gets the collection of files returned by the picker.
    /// </summary>
    public IReadOnlyList<IStorageFile> Files { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the pick operation has been handled.
    /// </summary>
    public bool Handled { get; set; }
}
