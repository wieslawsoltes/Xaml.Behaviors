// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that captures a screenshot of a control and saves it to a file.
/// </summary>
public class ScreenshotAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<ScreenshotAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <seealso cref="FileName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> FileNameProperty =
        AvaloniaProperty.Register<ScreenshotAction, string?>(nameof(FileName));

    /// <summary>
    /// Gets or sets the target control to capture. If null, the associated object is used.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the suggested file name for the screenshot.
    /// </summary>
    public string? FileName
    {
        get => GetValue(FileNameProperty);
        set => SetValue(FileNameProperty, value);
    }

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
            await SavePngAsync(target, async () =>
            {
                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Screenshot",
                    DefaultExtension = "png",
                    SuggestedFileName = FileName ?? "screenshot.png",
                    FileTypeChoices = new[] { FilePickerFileTypes.ImagePng }
                });

                return file is null ? null : await file.OpenWriteAsync();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Screenshot failed: {ex}");
        }
    }

    internal static async Task SavePngAsync(Control target, Func<Task<Stream?>> openWriteAsync)
    {
        var pixelSize = new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height);
        using var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
        bitmap.Render(target);

        using var stream = await openWriteAsync();
        if (stream is not null)
        {
            bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        }
    }
}
