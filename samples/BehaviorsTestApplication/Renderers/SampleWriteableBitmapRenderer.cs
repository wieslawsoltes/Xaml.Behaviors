using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Media;
using Avalonia.Media.Imaging;


namespace BehaviorsTestApplication.Renderers;

public class SampleWriteableBitmapRenderer : Avalonia.Xaml.Interactions.Custom.WriteableBitmap.IWriteableBitmapRenderer
{
    private readonly Stopwatch _st = Stopwatch.StartNew();

    public void Render(WriteableBitmap bitmap)
    {
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
    }

    private static int ColorToInt(Color color)
    {
        uint v = (uint)(color.B | (color.G << 8) | (color.R << 16) | (color.A << 24));
        return unchecked((int)v);
    }
}
