// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
    /// Gets or sets an option indicating whether open picker allows users to select multiple folders. This is an avalonia property.
    /// </summary>
    public bool AllowMultiple
    {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
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

        var storageProvider = StorageProvider ?? (visual.GetSelfAndLogicalAncestors().LastOrDefault() as TopLevel)?.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }

        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Title,
            SuggestedStartLocation = SuggestedStartLocation,
            SuggestedFileName = SuggestedFileName,
            AllowMultiple = AllowMultiple
        });

        if (folders.Count <= 0)
        {
            return;
        }

        var resolvedParameter = ResolveParameter(folders);

        if (!Command.CanExecute(resolvedParameter))
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
