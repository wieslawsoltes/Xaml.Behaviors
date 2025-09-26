// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Open folder picker behavior base.
/// </summary>
public abstract class OpenFolderPickerBehaviorBase : PickerBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="AllowMultiple"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowMultipleProperty =
        AvaloniaProperty.Register<OpenFolderPickerBehaviorBase, bool>(nameof(AllowMultiple));

    /// <summary>
    /// Identifies the <seealso cref="StoreSelectedFolders"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> StoreSelectedFoldersProperty =
        AvaloniaProperty.Register<OpenFolderPickerBehaviorBase, bool>(nameof(StoreSelectedFolders));

    /// <summary>
    /// Identifies the <seealso cref="SelectedFolders"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<IStorageFolder>?> SelectedFoldersProperty =
        AvaloniaProperty.Register<OpenFolderPickerBehaviorBase, IReadOnlyList<IStorageFolder>?>(nameof(SelectedFolders));

    /// <summary>
    /// Identifies the <seealso cref="SelectedFolderPaths"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<string>?> SelectedFolderPathsProperty =
        AvaloniaProperty.Register<OpenFolderPickerBehaviorBase, IReadOnlyList<string>?>(nameof(SelectedFolderPaths));

    /// <summary>
    /// Gets or sets an option indicating whether open picker allows users to select multiple folders. This is an avalonia property.
    /// </summary>
    public bool AllowMultiple
    {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the selected folders should be stored on the behavior for later binding.
    /// </summary>
    public bool StoreSelectedFolders
    {
        get => GetValue(StoreSelectedFoldersProperty);
        set => SetValue(StoreSelectedFoldersProperty, value);
    }

    /// <summary>
    /// Gets the folders returned by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public IReadOnlyList<IStorageFolder>? SelectedFolders
    {
        get => GetValue(SelectedFoldersProperty);
        protected set => SetValue(SelectedFoldersProperty, value);
    }

    /// <summary>
    /// Gets the folder paths returned by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public IReadOnlyList<string>? SelectedFolderPaths
    {
        get => GetValue(SelectedFolderPathsProperty);
        protected set => SetValue(SelectedFolderPathsProperty, value);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFolderPickerBehaviorBase"/> class.
    /// </summary>
    protected OpenFolderPickerBehaviorBase()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Launches the folder picker dialog.
    /// </summary>
    /// <param name="sender">The initiating control.</param>
    /// <param name="parameter">Optional command parameter.</param>
    protected async Task Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return;
        }

        await OpenFolderPickerAsync(visual);
    }

    private async Task OpenFolderPickerAsync(Visual visual)
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

        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Title,
            SuggestedStartLocation = suggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            AllowMultiple = AllowMultiple
        });

        if (folders.Count <= 0)
        {
            if (StoreSelectedFolders)
            {
                SelectedFolders = null;
                SelectedFolderPaths = null;
            }
            return;
        }

        if (StoreSelectedFolders)
        {
            var foldersList = folders.ToList();
            SelectedFolders = foldersList;
            SelectedFolderPaths = ConvertToPaths(foldersList);
        }

        var resolvedParameter = ResolveParameter(folders);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }

    private static IReadOnlyList<string> ConvertToPaths(IEnumerable<IStorageFolder> folders)
    {
        return folders.Select(static f => f.Path.IsAbsoluteUri ? f.Path.LocalPath : f.Path.ToString()).ToList();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (StoreSelectedFolders)
        {
            SelectedFolders = null;
            SelectedFolderPaths = null;
        }
    }
}
