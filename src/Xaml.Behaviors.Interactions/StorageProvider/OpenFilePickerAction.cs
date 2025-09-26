// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that will open a file picker dialog.
/// </summary>
public class OpenFilePickerAction : PickerActionBase
{
    /// <summary>
    /// Identifies the <seealso cref="AllowMultiple"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowMultipleProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, bool>(nameof(AllowMultiple));

    /// <summary>
    /// Identifies the <seealso cref="FileTypeFilter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FileTypeFilterProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, string?>(nameof(FileTypeFilter));

    /// <summary>
    /// Identifies the <seealso cref="StoreSelectedFiles"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> StoreSelectedFilesProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, bool>(nameof(StoreSelectedFiles));

    /// <summary>
    /// Identifies the <seealso cref="SelectedFiles"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<IStorageFile>?> SelectedFilesProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, IReadOnlyList<IStorageFile>?>(nameof(SelectedFiles));

    /// <summary>
    /// Identifies the <seealso cref="SelectedFilePaths"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<string>?> SelectedFilePathsProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, IReadOnlyList<string>?>(nameof(SelectedFilePaths));

    /// <summary>
    /// Gets or sets an option indicating whether open picker allows users to select multiple files. This is an avalonia property.
    /// </summary>
    public bool AllowMultiple
    {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of file types that the file open picker displays. This is an avalonia property.
    /// </summary>
    public string? FileTypeFilter
    {
        get => GetValue(FileTypeFilterProperty);
        set => SetValue(FileTypeFilterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the selected files should be stored on the action for later binding.
    /// </summary>
    public bool StoreSelectedFiles
    {
        get => GetValue(StoreSelectedFilesProperty);
        set => SetValue(StoreSelectedFilesProperty, value);
    }

    /// <summary>
    /// Gets the files returned by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public IReadOnlyList<IStorageFile>? SelectedFiles
    {
        get => GetValue(SelectedFilesProperty);
        private set => SetValue(SelectedFilesProperty, value);
    }

    /// <summary>
    /// Gets the file-system paths returned by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public IReadOnlyList<string>? SelectedFilePaths
    {
        get => GetValue(SelectedFilePathsProperty);
        private set => SetValue(SelectedFilePathsProperty, value);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFilePickerAction"/> class.
    /// </summary>
    public OpenFilePickerAction()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the command is successfully executed; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }
        
        Dispatcher.UIThread.InvokeAsync(async () => await OpenFilePickerAsync(visual));

        return true; 
    }

    private async Task OpenFilePickerAsync(Visual visual)
    {
        if (IsEnabled != true || Command is null)
        {
            return;
        }

        var storageProvider = ResolveStorageProvider(visual);
        if (storageProvider is null)
        {
            return;
        }

        var suggestedStartLocation = ResolveSuggestedStartLocation(storageProvider);

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Title,
            SuggestedStartLocation = suggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            AllowMultiple = AllowMultiple,
            FileTypeFilter = FileTypeFilter is not null 
                ? FileFilterParser.ConvertToFilePickerFileType(FileTypeFilter) 
                : null
        });

        if (files.Count <= 0)
        {
            if (StoreSelectedFiles)
            {
                SelectedFiles = null;
                SelectedFilePaths = null;
            }
            return;
        }

        if (StoreSelectedFiles)
        {
            var filesList = files.ToList();
            SelectedFiles = filesList;
            SelectedFilePaths = ConvertToPaths(filesList);
        }

        var resolvedParameter = ResolveParameter(files);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }

    private static IReadOnlyList<string> ConvertToPaths(IEnumerable<IStorageFile> files)
    {
        return files.Select(static f => f.Path.IsAbsoluteUri ? f.Path.LocalPath : f.Path.ToString()).ToList();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (StoreSelectedFiles)
        {
            SelectedFiles = null;
            SelectedFilePaths = null;
        }
    }
}
