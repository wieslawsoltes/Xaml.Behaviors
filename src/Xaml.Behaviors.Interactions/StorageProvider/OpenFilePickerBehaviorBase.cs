// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Open file picker behavior base.
/// </summary>
public abstract class OpenFilePickerBehaviorBase : PickerBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="AllowMultiple"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowMultipleProperty =
        AvaloniaProperty.Register<OpenFilePickerBehaviorBase, bool>(nameof(AllowMultiple));

    /// <summary>
    /// Identifies the <seealso cref="FileTypeFilter"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FileTypeFilterProperty =
        AvaloniaProperty.Register<OpenFilePickerBehaviorBase, string?>(nameof(FileTypeFilter));

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
            return;
        }

        var resolvedParameter = ResolveParameter(files);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
