// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Xaml.Interactions.Core;

/// <summary>
/// <see cref="IStorageProvider"/> implementation over the Windows.Storage.Pickers file and folder pickers.
/// </summary>
/// <remarks>
/// WinUI pickers cannot open an arbitrary start folder; <see cref="PickerOptions.SuggestedStartLocation"/> is ignored and
/// the documents library is suggested instead. Folder pickers return a single folder.
/// </remarks>
public sealed class SystemStorageProvider : IStorageProvider
{
    private SystemStorageProvider()
    {
    }

    /// <summary>Gets the shared instance.</summary>
    public static SystemStorageProvider Instance { get; } = new();

    /// <inheritdoc />
    public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        foreach (var extension in GetExtensions(options.FileTypeFilter))
        {
            picker.FileTypeFilter.Add(extension);
        }

        if (options.AllowMultiple)
        {
            var files = await picker.PickMultipleFilesAsync();
            return files?.Cast<IStorageFile>().ToArray() ?? [];
        }

        var file = await picker.PickSingleFileAsync();
        return file is null ? [] : [file];
    }

    /// <inheritdoc />
    public async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
    {
        var picker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        if (!string.IsNullOrEmpty(options.SuggestedFileName))
        {
            picker.SuggestedFileName = options.SuggestedFileName;
        }

        if (!string.IsNullOrEmpty(options.DefaultExtension))
        {
            picker.DefaultFileExtension = NormalizeExtension(options.DefaultExtension!);
        }

        foreach (var fileType in options.FileTypeChoices ?? [])
        {
            var extensions = GetExtensions([fileType]).Where(static e => e != "*").ToList();
            if (extensions.Count > 0)
            {
                picker.FileTypeChoices.Add(fileType.Name, extensions);
            }
        }

        if (picker.FileTypeChoices.Count == 0)
        {
            var extension = string.IsNullOrEmpty(options.DefaultExtension) ? ".txt" : NormalizeExtension(options.DefaultExtension!);
            picker.FileTypeChoices.Add(extension.TrimStart('.'), [extension]);
        }

        return await picker.PickSaveFileAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
    {
        var picker = new FolderPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add("*");
        var folder = await picker.PickSingleFolderAsync();
        return folder is null ? [] : [folder];
    }

    /// <inheritdoc />
    public async Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
    {
        try
        {
            return await StorageFolder.GetFolderFromPathAsync(folderPath.IsFile ? folderPath.LocalPath : folderPath.ToString());
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static IEnumerable<string> GetExtensions(IReadOnlyList<FilePickerFileType>? fileTypes)
    {
        var any = true;
        foreach (var fileType in fileTypes ?? [])
        {
            foreach (var pattern in fileType.Patterns ?? [])
            {
                any = false;
                yield return pattern is "*" or "*.*" ? "*" : NormalizeExtension(pattern.TrimStart('*'));
            }
        }

        if (any)
        {
            yield return "*";
        }
    }

    private static string NormalizeExtension(string extension) => extension.StartsWith('.') ? extension : "." + extension;
}
