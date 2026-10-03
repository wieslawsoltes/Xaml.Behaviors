// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Windows.Storage;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Core;
#else
namespace Avalonia.Xaml.Interactions.Core;
#endif

/// <summary>
/// Base class for picker behaviors.
/// </summary>
public abstract partial class PickerBehaviorBase : InvokeCommandBehaviorBase
{

    /// <summary>
    /// Gets or sets the storage provider that the picker uses to access the file system. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IStorageProvider? StorageProvider { get; set; }

    /// <summary>
    /// Gets or sets the text that appears in the title bar of a picker. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? Title { get; set; }

    /// <summary>
    /// Gets or sets the initial location where the file open picker looks for files to present to the user. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IStorageFolder? SuggestedStartLocation { get; set; }

    /// <summary>
    /// Gets or sets a fallback path that is used to resolve <see cref="SuggestedStartLocation"/> when no folder is provided.
    /// </summary>
    [StyledProperty]
    public partial string? SuggestedStartLocationPath { get; set; }

    /// <summary>
    /// Gets or sets the file name that the file picker suggests to the user. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? SuggestedFileName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to create the suggested start location directory if it doesn't exist. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = false)]
    public partial bool CreateSuggestedStartLocationDirectory { get; set; }

    /// <summary>
    /// Resolves the storage provider using the configured value or the provided fallback visual.
    /// </summary>
    /// <param name="fallbackVisual">Visual used as a fallback source when the property is unset.</param>
    /// <returns>The resolved <see cref="IStorageProvider"/> instance or null.</returns>
    protected IStorageProvider? ResolveStorageProvider(Visual? fallbackVisual)
    {
        if (StorageProvider is { } provider)
        {
            return provider;
        }

        if (fallbackVisual is not null)
        {
            provider = ResolveFromObject(fallbackVisual);
            if (provider is not null)
            {
                return provider;
            }
        }

        return ResolveFromObject(AssociatedObject as AvaloniaObject) ?? ResolveFromObject(this);
    }

    /// <summary>
    /// Resolves the suggested start folder using the configured value or the provided path.
    /// </summary>
    /// <param name="provider">Storage provider used to translate the configured path.</param>
    /// <returns>The resolved <see cref="IStorageFolder"/> instance or null.</returns>
    protected IStorageFolder? ResolveSuggestedStartLocation(IStorageProvider? provider)
    {
        return PickerSuggestedStartLocationHelper.Resolve(
            SuggestedStartLocation,
            SuggestedStartLocationPath,
            CreateSuggestedStartLocationDirectory,
            provider);
    }

#if UNO
    // WinUI has no top level storage provider: the pickers are application wide.
    private static IStorageProvider? ResolveFromObject(object? target) => SystemStorageProvider.Instance;
#else
    private static IStorageProvider? ResolveFromObject(object? target)
    {
        return target switch
        {
            TopLevel topLevel => topLevel.StorageProvider,
            Visual visual => visual.GetSelfAndLogicalAncestors().OfType<TopLevel>().FirstOrDefault()?.StorageProvider ?? TopLevel.GetTopLevel(visual)?.StorageProvider,
            ILogical logical => logical.GetSelfAndLogicalAncestors().OfType<TopLevel>().FirstOrDefault()?.StorageProvider,
            _ => null
        };
    }
#endif
}
