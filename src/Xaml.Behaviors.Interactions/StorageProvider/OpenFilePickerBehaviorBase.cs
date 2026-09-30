// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// Open file picker behavior base.
/// </summary>
public abstract partial class OpenFilePickerBehaviorBase : PickerBehaviorBase
{
    /// <summary>
    /// Occurs after the file picker successfully returns one or more files.
    /// </summary>
    public event EventHandler<FilePickerEventArgs>? Pick;

    /// <summary>
    /// Gets or sets an option indicating whether open picker allows users to select multiple files. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool AllowMultiple { get; set; }

    /// <summary>
    /// Gets or sets the collection of file types that the file open picker displays. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? FileTypeFilter { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFilePickerBehaviorBase"/> class.
    /// </summary>
    protected OpenFilePickerBehaviorBase()
    {
        PassEventArgsToCommand = true;
    }

    /// <summary>
    /// Executes the open file picker using the provided sender and command parameter.
    /// </summary>
    /// <param name="sender">The control that triggered the behavior.</param>
    /// <param name="parameter">An optional parameter used when executing the command.</param>
    protected async Task Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return;
        }

        await OpenFilePickerAsync(visual);
    }

    private async Task OpenFilePickerAsync(Visual visual)
    {
        var command = Command;
        if (IsEnabled != true || (command is null && Pick is null))
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
            return;
        }

        var eventArgs = new FilePickerEventArgs(files);
        OnPick(eventArgs);
        if (eventArgs.Handled || command is null)
        {
            return;
        }

        var resolvedParameter = ResolveParameter(files);

        if (!command.CanExecute(resolvedParameter))
        {
            return;
        }

        command.Execute(resolvedParameter);
    }

    /// <summary>
    /// Raises the <see cref="Pick"/> event.
    /// </summary>
    /// <param name="args">Event arguments describing the picked files.</param>
    protected virtual void OnPick(FilePickerEventArgs args)
    {
        Pick?.Invoke(this, args);
    }
}
