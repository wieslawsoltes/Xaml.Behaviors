// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Base class for picker actions.
/// </summary>
public abstract class PickerActionBase : InvokeCommandActionBase
{
    /// <summary>
    /// Identifies the <seealso cref="StorageProvider"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageProvider?> StorageProviderProperty =
        AvaloniaProperty.Register<PickerActionBase, IStorageProvider?>(nameof(StorageProvider));

    /// <summary>
    /// Identifies the <seealso cref="Title"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<PickerActionBase, string?>(nameof(Title));
    
    /// <summary>
    /// Identifies the <seealso cref="SuggestedStartLocation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IStorageFolder?> SuggestedStartLocationProperty =
        AvaloniaProperty.Register<PickerActionBase, IStorageFolder?>(nameof(SuggestedStartLocation));
    
    /// <summary>
    /// Identifies the <seealso cref="SuggestedFileName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SuggestedFileNameProperty =
        AvaloniaProperty.Register<PickerActionBase, string?>(nameof(SuggestedFileName));

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
    /// Gets or sets the file name that the file picker suggests to the user. This is an avalonia property.
    /// </summary>
    public string? SuggestedFileName
    {
        get => GetValue(SuggestedFileNameProperty);
        set => SetValue(SuggestedFileNameProperty, value);
    } 
}
