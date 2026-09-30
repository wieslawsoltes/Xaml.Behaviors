// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Windows.Storage;

namespace Xaml.Interactions.Core;

/// <summary>
/// Describes a file type shown by a file picker.
/// </summary>
/// <param name="name">The display name of the file type.</param>
public sealed class FilePickerFileType(string? name)
{
    /// <summary>Gets the display name of the file type.</summary>
    public string Name { get; } = name ?? string.Empty;

    /// <summary>Gets or sets the glob patterns (for example <c>*.txt</c>).</summary>
    public IReadOnlyList<string>? Patterns { get; set; }

    /// <summary>Gets or sets the MIME types.</summary>
    public IReadOnlyList<string>? MimeTypes { get; set; }

    /// <summary>Gets or sets the Apple uniform type identifiers.</summary>
    public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; set; }
}

/// <summary>
/// Common picker options.
/// </summary>
public class PickerOptions
{
    /// <summary>Gets or sets the picker title.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets the initial folder.</summary>
    public IStorageFolder? SuggestedStartLocation { get; set; }

    /// <summary>Gets or sets the suggested file name.</summary>
    public string? SuggestedFileName { get; set; }
}

/// <summary>
/// Options of a file open picker.
/// </summary>
public class FilePickerOpenOptions : PickerOptions
{
    /// <summary>Gets or sets a value indicating whether several files can be picked.</summary>
    public bool AllowMultiple { get; set; }

    /// <summary>Gets or sets the file types that can be picked.</summary>
    public IReadOnlyList<FilePickerFileType>? FileTypeFilter { get; set; }
}

/// <summary>
/// Options of a file save picker.
/// </summary>
public class FilePickerSaveOptions : PickerOptions
{
    /// <summary>Gets or sets the default extension.</summary>
    public string? DefaultExtension { get; set; }

    /// <summary>Gets or sets the file types the file can be saved as.</summary>
    public IReadOnlyList<FilePickerFileType>? FileTypeChoices { get; set; }

    /// <summary>Gets or sets a value indicating whether an overwrite prompt is shown.</summary>
    public bool? ShowOverwritePrompt { get; set; }
}

/// <summary>
/// Options of a folder picker.
/// </summary>
public class FolderPickerOpenOptions : PickerOptions
{
    /// <summary>Gets or sets a value indicating whether several folders can be picked.</summary>
    public bool AllowMultiple { get; set; }
}
