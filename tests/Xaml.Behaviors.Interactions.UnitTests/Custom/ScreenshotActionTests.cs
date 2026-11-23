using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class ScreenshotActionTests
{
    [Fact]
    public async Task Execute_Saves_The_Control_As_A_Png()
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(ScreenshotTestAppBuilder));
        await session.Dispatch(() =>
        {
            using var file = new InMemoryStorageFile();
            var target = new Border { Width = 24, Height = 18, Background = Brushes.Blue };
            var window = new Window { Content = target, Tag = new ScreenshotStorageProvider(file) };

            window.Show();
            try
            {
                var action = new ScreenshotAction();
                Assert.True(Assert.IsType<bool>(action.Execute(target, parameter: null)));

                byte[] bytes = file.GetBytes();
                ReadOnlySpan<byte> pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
                Assert.True(bytes.AsSpan().StartsWith(pngSignature));

                using var stream = new MemoryStream(bytes);
                using var bitmap = new Bitmap(stream);
                Assert.Equal(new PixelSize(24, 18), bitmap.PixelSize);
            }
            finally
            {
                window.Close();
            }
        }, TestContext.Current.CancellationToken);
    }

    private static class ScreenshotTestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp() =>
            TestAppBuilder.BuildAvaloniaApp().With<IStorageProviderFactory>(new ScreenshotStorageProviderFactory());
    }

    private sealed class ScreenshotStorageProviderFactory : IStorageProviderFactory
    {
        public IStorageProvider CreateProvider(TopLevel topLevel) => (IStorageProvider)topLevel.Tag!;
    }

    private sealed class ScreenshotStorageProvider(IStorageFile file) : IStorageProvider
    {
        public bool CanOpen => false;
        public bool CanSave => true;
        public bool CanPickFolder => false;

        public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options) => Task.FromResult<IStorageFile?>(file);
        public Task<SaveFilePickerResult> SaveFilePickerWithResultAsync(FilePickerSaveOptions options) => throw new NotSupportedException();
        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options) => throw new NotSupportedException();
        public Task<OpenFilePickerResult> OpenFilePickerWithResultAsync(FilePickerOpenOptions options) => throw new NotSupportedException();
        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options) => throw new NotSupportedException();
        public Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark) => throw new NotSupportedException();
        public Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark) => throw new NotSupportedException();
        public Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath) => throw new NotSupportedException();
        public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath) => throw new NotSupportedException();
        public Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder) => throw new NotSupportedException();
    }

    private sealed class InMemoryStorageFile : IStorageFile
    {
        private readonly MemoryStream _stream = new();

        public string Name => "screenshot.png";
        public Uri Path => throw new NotSupportedException();
        public bool CanBookmark => false;

        public byte[] GetBytes() => _stream.ToArray();
        public Task<Stream> OpenWriteAsync() => Task.FromResult<Stream>(_stream);
        public Task<Stream> OpenReadAsync() => throw new NotSupportedException();
        public Task<StorageItemProperties> GetBasicPropertiesAsync() => throw new NotSupportedException();
        public Task<string?> SaveBookmarkAsync() => throw new NotSupportedException();
        public Task<IStorageFolder?> GetParentAsync() => throw new NotSupportedException();
        public Task DeleteAsync() => throw new NotSupportedException();
        public Task<IStorageItem?> MoveAsync(IStorageFolder destination) => throw new NotSupportedException();
        public void Dispose() => _stream.Dispose();
    }
}
