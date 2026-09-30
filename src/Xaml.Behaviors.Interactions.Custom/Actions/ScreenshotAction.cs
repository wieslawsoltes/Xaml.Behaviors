// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media;
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

        var topLevel = TopLevel.GetTopLevel(target);
        if (topLevel is null)
        {
            return false;
        }

        CaptureAsync(target, topLevel);
        return true;
    }

    private async void CaptureAsync(Control target, TopLevel topLevel)
    {
        try
        {
            // Render the control to a bitmap
            var pixelSize = new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height);
            // Use default DPI or target's DPI? 
            // For simplicity, we use 96 DPI (Vector(1,1) * 96) or just Vector(96, 96).
            // RenderTargetBitmap expects DPI.
            var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
            bitmap.Render(target);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Screenshot",
                DefaultExtension = "png",
                SuggestedFileName = FileName ?? "screenshot.png",
                FileTypeChoices = new[] { FilePickerFileTypes.ImagePng }
            });

            if (file is not null)
            {
                using var stream = await file.OpenWriteAsync();
                bitmap.Save(stream);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Screenshot failed: {ex}");
        }
    }
}
