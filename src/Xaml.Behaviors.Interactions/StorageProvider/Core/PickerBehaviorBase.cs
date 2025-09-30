// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Base class for picker behaviors.
/// </summary>
public abstract class PickerBehaviorBase : InvokeCommandBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="StorageProvider"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageProvider?> StorageProviderProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, IStorageProvider?>(nameof(StorageProvider));

    /// <summary>
    /// Identifies the <seealso cref="Title"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, string?>(nameof(Title));

    /// <summary>
    /// Identifies the <seealso cref="SuggestedStartLocation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageFolder?> SuggestedStartLocationProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, IStorageFolder?>(nameof(SuggestedStartLocation));

    /// <summary>
    /// Identifies the <seealso cref="SuggestedStartLocationPath"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SuggestedStartLocationPathProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, string?>(nameof(SuggestedStartLocationPath));

    /// <summary>
    /// Identifies the <seealso cref="SuggestedFileName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SuggestedFileNameProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, string?>(nameof(SuggestedFileName));

    /// <summary>
    /// Identifies the <seealso cref="CreateSuggestedStartLocationDirectory"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> CreateSuggestedStartLocationDirectoryProperty =
        AvaloniaProperty.Register<PickerBehaviorBase, bool>(nameof(CreateSuggestedStartLocationDirectory), false);

    /// <summary>
    /// Gets or sets the storage provider that the picker uses to access the file system. This is an avalonia property.
    /// </summary>
    public IStorageProvider? StorageProvider
    {
        get => GetValue(StorageProviderProperty);
        set => SetValue(StorageProviderProperty, value);
    }

    /// <summary>
    /// Gets or sets the text that appears in the title bar of a picker. This is an avalonia property.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the initial location where the file open picker looks for files to present to the user. This is an avalonia property.
    /// </summary>
    public IStorageFolder? SuggestedStartLocation
    {
        get => GetValue(SuggestedStartLocationProperty);
        set => SetValue(SuggestedStartLocationProperty, value);
    }

    /// <summary>
    /// Gets or sets a fallback path that is used to resolve <see cref="SuggestedStartLocation"/> when no folder is provided.
    /// </summary>
    public string? SuggestedStartLocationPath
    {
        get => GetValue(SuggestedStartLocationPathProperty);
        set => SetValue(SuggestedStartLocationPathProperty, value);
    }

    /// <summary>
    /// Gets or sets the file name that the file picker suggests to the user. This is an avalonia property.
    /// </summary>
    public string? SuggestedFileName
    {
        get => GetValue(SuggestedFileNameProperty);
        set => SetValue(SuggestedFileNameProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to create the suggested start location directory if it doesn't exist. This is an avalonia property.
    /// </summary>
    public bool CreateSuggestedStartLocationDirectory
    {
        get => GetValue(CreateSuggestedStartLocationDirectoryProperty);
        set => SetValue(CreateSuggestedStartLocationDirectoryProperty, value);
    }

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
}
