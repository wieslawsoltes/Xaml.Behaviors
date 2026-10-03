// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Xaml.Interactions.Core;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that captures a screenshot of a control and saves it to a file.
/// </summary>
public partial class ScreenshotAction : StyledElementAction
{
    private const string SaveScreenshotTitle = "Save Screenshot";
    private const string DefaultFileName = "screenshot.png";

    /// <summary>
    /// Gets or sets the target control to capture. If null, the associated object is used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <summary>
    /// Gets or sets the suggested file name for the screenshot.
    /// </summary>
    [StyledProperty]
    public partial string? FileName { get; set; }

    /// <summary>
    /// Gets or sets the storage provider used to pick the screenshot file. If null, the storage provider of the
    /// top level of the target control is used. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial IStorageProvider? StorageProvider { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = TargetControl ?? sender as Control;
        if (target is null)
        {
            return false;
        }

#if UNO
        // WinUI has no top level storage provider (the pickers are application wide); the target must be loaded so
        // that RenderTargetBitmap can render it.
        if (target.XamlRoot is null)
        {
            return false;
        }

        var storageProvider = StorageProvider ?? SystemStorageProvider.Instance;
#else
        var topLevel = TopLevel.GetTopLevel(target);
        if (topLevel is null)
        {
            return false;
        }

        var storageProvider = StorageProvider ?? topLevel.StorageProvider;
#endif

        _ = CaptureAsync(target, storageProvider);
        return true;
    }

    private async Task CaptureAsync(Control target, IStorageProvider storageProvider)
    {
        try
        {
            var screenshot = await RenderAsync(target);
            if (screenshot is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = SaveScreenshotTitle,
                DefaultExtension = "png",
                SuggestedFileName = FileName ?? DefaultFileName,
                FileTypeChoices = [PngFileType]
            });

            if (file is not null)
            {
                await SaveAsync(screenshot, file);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Screenshot failed: {ex}");
        }
    }

#if !UNO
    private static FilePickerFileType PngFileType => FilePickerFileTypes.ImagePng;

    /// <summary>
    /// Renders the control into a bitmap of its size in device independent pixels (96 DPI).
    /// </summary>
    private static Task<RenderTargetBitmap?> RenderAsync(Control target)
    {
        var pixelSize = new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height);
        var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
        bitmap.Render(target);
        return Task.FromResult<RenderTargetBitmap?>(bitmap);
    }

    private static async Task SaveAsync(RenderTargetBitmap bitmap, IStorageFile file)
    {
        using var stream = await file.OpenWriteAsync();
        bitmap.Save(stream);
    }
#endif
}
