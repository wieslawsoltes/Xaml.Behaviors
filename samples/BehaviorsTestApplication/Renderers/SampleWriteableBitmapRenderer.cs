using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if UNO
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using Xaml.Interactions.Custom;
#else
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactions.Custom;
#endif


namespace BehaviorsTestApplication.Renderers;

public class SampleWriteableBitmapRenderer : IWriteableBitmapRenderer
{
    private readonly Stopwatch _st = Stopwatch.StartNew();

    public void Render(WriteableBitmap bitmap)
    {
#if UNO
        // WinUI: the pixel buffer holds premultiplied BGRA bytes; Invalidate redraws the bitmap.
        var pixelCount = bitmap.PixelWidth * bitmap.PixelHeight;
        var pixels = new int[pixelCount];
        byte alpha = (byte)((_st.ElapsedMilliseconds / 10) % 256);
        Array.Fill(pixels, ColorToInt(Color.FromArgb(alpha, 0, alpha, 0)));
        var bytes = new byte[pixelCount * sizeof(int)];
        Buffer.BlockCopy(pixels, 0, bytes, 0, bytes.Length);
        using (var stream = bitmap.PixelBuffer.AsStream())
        {
            stream.Write(bytes, 0, bytes.Length);
        }
        bitmap.Invalidate();
#else
        using var fb = bitmap.Lock();
        var pixels = new int[fb.Size.Width * fb.Size.Height];
        byte alpha = (byte)((_st.ElapsedMilliseconds / 10) % 256);
        for (int y = 0; y < fb.Size.Height; y++)
        {
            for (int x = 0; x < fb.Size.Width; x++)
            {
                pixels[y * fb.Size.Width + x] = ColorToInt(Color.FromArgb(alpha, 0, 255, 0));
            }
        }
        Marshal.Copy(pixels, 0, fb.Address, pixels.Length);
#endif
    }

    private static int ColorToInt(Color color)
    {
        uint v = (uint)(color.B | (color.G << 8) | (color.R << 16) | (color.A << 24));
        return unchecked((int)v);
    }
}
