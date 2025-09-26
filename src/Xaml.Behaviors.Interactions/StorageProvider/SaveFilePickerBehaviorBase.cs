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
/// Save file picker behavior base.
/// </summary>
public abstract class SaveFilePickerBehaviorBase : PickerBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="DefaultExtension"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> DefaultExtensionProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, string?>(nameof(DefaultExtension));

    /// <summary>
    /// Identifies the <seealso cref="FileTypeChoices"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FileTypeChoicesProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, string?>(nameof(FileTypeChoices));

    /// <summary>
    /// Identifies the <seealso cref="ShowOverwritePrompt"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool?> ShowOverwritePromptProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, bool?>(nameof(ShowOverwritePrompt));

    /// <summary>
    /// Identifies the <seealso cref="StoreSavedFile"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> StoreSavedFileProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, bool>(nameof(StoreSavedFile));

    /// <summary>
    /// Identifies the <seealso cref="SavedFile"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageFile?> SavedFileProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, IStorageFile?>(nameof(SavedFile));

    /// <summary>
    /// Identifies the <seealso cref="SavedFilePath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SavedFilePathProperty =
        AvaloniaProperty.Register<SaveFilePickerBehaviorBase, string?>(nameof(SavedFilePath));

    /// <summary>
    /// Gets or sets the default extension to be used to save the file. This is an avalonia property.
    /// </summary>
    public string? DefaultExtension
    {
        get => GetValue(DefaultExtensionProperty);
        set => SetValue(DefaultExtensionProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of valid file types that the user can choose to assign to a file. This is an avalonia property.
    /// </summary>
    public string? FileTypeChoices
    {
        get => GetValue(FileTypeChoicesProperty);
        set => SetValue(FileTypeChoicesProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether file open picker displays a warning if the user specifies the name of a file that already exists. This is an avalonia property.
    /// </summary>
    public bool? ShowOverwritePrompt
    {
        get => GetValue(ShowOverwritePromptProperty);
        set => SetValue(ShowOverwritePromptProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the saved file should be stored on the behavior for later binding.
    /// </summary>
    public bool StoreSavedFile
    {
        get => GetValue(StoreSavedFileProperty);
        set => SetValue(StoreSavedFileProperty, value);
    }

    /// <summary>
    /// Gets the file produced by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public IStorageFile? SavedFile
    {
        get => GetValue(SavedFileProperty);
        protected set => SetValue(SavedFileProperty, value);
    }

    /// <summary>
    /// Gets the file path produced by the most recent picker operation. This is an avalonia property.
    /// </summary>
    public string? SavedFilePath
    {
        get => GetValue(SavedFilePathProperty);
        protected set => SetValue(SavedFilePathProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveFilePickerBehaviorBase"/> class.
    /// </summary>
    protected SaveFilePickerBehaviorBase()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Launches the save file picker dialog.
    /// </summary>
    /// <param name="sender">The element that started the request.</param>
    /// <param name="parameter">Optional parameter passed to the command.</param>
    protected async Task Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return;
        }
        
        await SaveFilePickerAsync(visual);
    }

    private async Task SaveFilePickerAsync(Visual visual)
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

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Title,
            SuggestedStartLocation = suggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            DefaultExtension = DefaultExtension,
            FileTypeChoices = FileTypeChoices is not null 
                ? FileFilterParser.ConvertToFilePickerFileType(FileTypeChoices) 
                : null,
            ShowOverwritePrompt = ShowOverwritePrompt,
        });

        if (file is null)
        {
            if (StoreSavedFile)
            {
                SavedFile = null;
                SavedFilePath = null;
            }
            return;
        }

        if (StoreSavedFile)
        {
            SavedFile = file;
            SavedFilePath = file.Path.IsAbsoluteUri ? file.Path.LocalPath : file.Path.ToString();
        }

        var resolvedParameter = ResolveParameter(file);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (StoreSavedFile)
        {
            SavedFile = null;
            SavedFilePath = null;
        }
    }
}
