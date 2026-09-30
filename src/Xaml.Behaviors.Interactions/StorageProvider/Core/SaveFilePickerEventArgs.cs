// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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
/// Event args raised after a save file pick operation completes.
/// </summary>
public class SaveFilePickerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveFilePickerEventArgs"/> class.
    /// </summary>
    /// <param name="file">The file returned by the picker.</param>
    public SaveFilePickerEventArgs(IStorageFile file)
    {
        File = file;
    }

    /// <summary>
    /// Gets the file returned by the picker.
    /// </summary>
    public IStorageFile File { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the pick operation has been handled.
    /// </summary>
    public bool Handled { get; set; }
}
