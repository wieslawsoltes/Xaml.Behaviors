// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Core;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

public class ScreenshotActionTests
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    [UnoHeadlessFact]
    public async Task Execute_Saves_The_Target_As_Png_Through_The_Storage_Provider()
    {
        var path = await CreateFileAsync(new byte[4096]);
        var provider = new RecordingStorageProvider(await StorageFile.GetFileFromPathAsync(path));
        var target = new Border { Width = 40, Height = 30, Background = new SolidColorBrush(Colors.Red) };
        var sender = new Button();
        await Session.ShowAsync(new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Children = { sender, target },
        });

        var action = new ScreenshotAction { TargetControl = target, FileName = "capture.png", StorageProvider = provider };
        Assert.True((bool)action.Execute(sender, null));

        var png = await ReadCompletePngAsync(path);

        Assert.Equal(PngSignature, png.Take(PngSignature.Length));
        Assert.Equal("IHDR", System.Text.Encoding.ASCII.GetString(png, 12, 4));
        Assert.Equal(40, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(16, 4)));
        Assert.Equal(30, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(20, 4)));
        Assert.Equal([0xFF, 0x00, 0x00, 0xFF], ReadFirstPixel(png));

        var options = Assert.Single(provider.SaveOptions);
        Assert.Equal("Save Screenshot", options.Title);
        Assert.Equal("capture.png", options.SuggestedFileName);
        Assert.Equal("png", options.DefaultExtension);
        Assert.Equal(["*.png"], Assert.Single(options.FileTypeChoices!).Patterns!);
    }

    [UnoHeadlessFact]
    public async Task Execute_Captures_The_Sender_When_No_Target_Is_Set()
    {
        var path = await CreateFileAsync([]);
        var provider = new RecordingStorageProvider(await StorageFile.GetFileFromPathAsync(path));
        var sender = new Border { Width = 24, Height = 12, Background = new SolidColorBrush(Colors.Blue) };
        await Session.ShowAsync(new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Children = { sender },
        });

        Assert.True((bool)new ScreenshotAction { StorageProvider = provider }.Execute(sender, null));

        var png = await ReadCompletePngAsync(path);
        Assert.Equal(24, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(16, 4)));
        Assert.Equal(12, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(20, 4)));
        Assert.Equal([0x00, 0x00, 0xFF, 0xFF], ReadFirstPixel(png));
        Assert.Equal("screenshot.png", Assert.Single(provider.SaveOptions).SuggestedFileName);
    }

    [UnoHeadlessFact]
    public async Task Execute_Does_Not_Save_When_The_Picker_Is_Cancelled()
    {
        var provider = new RecordingStorageProvider(null);
        var target = new Border { Width = 10, Height = 10, Background = new SolidColorBrush(Colors.Green) };
        await Session.ShowAsync(target);

        Assert.True((bool)new ScreenshotAction { StorageProvider = provider }.Execute(target, null));

        await TestInput.WaitUntilAsync(() => provider.SaveOptions.Count > 0);
        Assert.Single(provider.SaveOptions);
    }

    [UnoHeadlessFact]
    public void Execute_Returns_False_Without_A_Loaded_Target()
    {
        var provider = new RecordingStorageProvider(null);

        Assert.False((bool)new ScreenshotAction { StorageProvider = provider }.Execute(null, null));
        Assert.False((bool)new ScreenshotAction { StorageProvider = provider }.Execute(new Border(), null));
        Assert.False((bool)new ScreenshotAction { StorageProvider = provider, IsEnabled = false }.Execute(new Border(), null));
        Assert.Empty(provider.SaveOptions);
    }

    private static async Task<string> CreateFileAsync(byte[] content)
    {
        var directory = Directory.CreateTempSubdirectory("xaml-behaviors-uno-screenshot-");
        var path = Path.Combine(directory.FullName, "capture.png");
        await File.WriteAllBytesAsync(path, content);
        return path;
    }

    /// <summary>
    /// Waits until the action has written a complete PNG (the file ends with the IEND chunk).
    /// </summary>
    private static async Task<byte[]> ReadCompletePngAsync(string path)
    {
        byte[] content = [];
        await TestInput.WaitUntilAsync(() =>
        {
            content = ReadShared(path);
            return EndsWithIend(content);
        }, timeoutMilliseconds: 10000);

        Assert.True(EndsWithIend(content), $"The screenshot was not written ({content.Length} bytes).");
        return content;
    }

    /// <summary>
    /// Reads the RGBA value of the top left pixel (8 bit RGBA image). Every PNG row filter leaves the first pixel of the
    /// first row unchanged, so no unfiltering is needed.
    /// </summary>
    private static byte[] ReadFirstPixel(byte[] png)
    {
        Assert.Equal(8, png[24]);
        Assert.Equal(6, png[25]);

        using var compressed = new MemoryStream();
        for (var offset = PngSignature.Length; offset < png.Length;)
        {
            var length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset, 4));
            if (System.Text.Encoding.ASCII.GetString(png, offset + 4, 4) == "IDAT")
            {
                compressed.Write(png, offset + 8, length);
            }

            offset += length + 12;
        }

        compressed.Position = 0;
        using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
        var scanline = new byte[5];
        zlib.ReadExactly(scanline);
        return scanline[1..];
    }

    private static byte[] ReadShared(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static bool EndsWithIend(byte[] content)
        => content.Length >= PngSignature.Length + 12
           && System.Text.Encoding.ASCII.GetString(content, content.Length - 8, 4) == "IEND";

    private sealed class RecordingStorageProvider(IStorageFile? file) : IStorageProvider
    {
        public List<FilePickerSaveOptions> SaveOptions { get; } = [];

        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
            => Task.FromResult<IReadOnlyList<IStorageFile>>([]);

        public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
        {
            SaveOptions.Add(options);
            return Task.FromResult(file);
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
            => Task.FromResult<IReadOnlyList<IStorageFolder>>([]);

        public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
            => Task.FromResult<IStorageFolder?>(null);
    }
}
