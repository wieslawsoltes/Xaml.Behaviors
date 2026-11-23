using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactions.Custom;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class ScreenshotActionTests
{
    [AvaloniaFact]
    public async Task SavePngAsync_Encodes_Control_At_Its_Render_Size()
    {
        var target = new Border { Width = 24, Height = 18, Background = Brushes.Blue };
        var window = new Window { Content = target };
        window.Show();

        try
        {
            using var stream = new MemoryStream();
            await ScreenshotAction.SavePngAsync(target, () => Task.FromResult<Stream?>(stream));

            byte[] bytes = stream.ToArray();
            ReadOnlySpan<byte> pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
            Assert.True(bytes.AsSpan().StartsWith(pngSignature));

            using var readStream = new MemoryStream(bytes);
            using var bitmap = new Bitmap(readStream);
            Assert.Equal(new PixelSize(24, 18), bitmap.PixelSize);
        }
        finally
        {
            window.Close();
        }
    }
}
