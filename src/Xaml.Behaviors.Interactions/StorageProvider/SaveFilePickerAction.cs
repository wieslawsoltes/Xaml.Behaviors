// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
public class SaveFilePickerAction : PickerActionBase
{
    /// <summary>
    /// Identifies the <seealso cref="DefaultExtension"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> DefaultExtensionProperty =
        AvaloniaProperty.Register<SaveFilePickerAction, string?>(nameof(DefaultExtension));

    /// <summary>
    /// Identifies the <seealso cref="FileTypeChoices"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FileTypeChoicesProperty =
        AvaloniaProperty.Register<OpenFilePickerAction, string?>(nameof(FileTypeChoices));

    /// <summary>
    /// Identifies the <seealso cref="ShowOverwritePrompt"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool?> ShowOverwritePromptProperty =
        AvaloniaProperty.Register<SaveFilePickerAction, bool?>(nameof(ShowOverwritePrompt));

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
    /// Initializes a new instance of the <see cref="SaveFilePickerAction"/> class.
    /// </summary>
    public SaveFilePickerAction()
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
        
        Dispatcher.UIThread.InvokeAsync(async () => await SaveFilePickerAsync(visual));

        return true; 
    }

    private async Task SaveFilePickerAsync(Visual visual)
    {
        if (IsEnabled != true || Command is null)
        {
            return;
        }

        var storageProvider = StorageProvider ?? (visual.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Title,
            SuggestedStartLocation = SuggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            DefaultExtension = DefaultExtension,
            FileTypeChoices = FileTypeChoices is not null 
                ? FileFilterParser.ConvertToFilePickerFileType(FileTypeChoices) 
                : null,
            ShowOverwritePrompt = ShowOverwritePrompt,
        });

        if (file is null)
        {
            return;
        }

        var resolvedParameter = ResolveParameter(file);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
