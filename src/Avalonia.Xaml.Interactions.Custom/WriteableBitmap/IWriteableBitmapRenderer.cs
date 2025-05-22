using Avalonia.Media.Imaging;

namespace Avalonia.Xaml.Interactions.Custom.WriteableBitmap;

/// <summary>
/// Defines a method used to render into a <see cref="WriteableBitmap"/>.
/// </summary>
public interface IWriteableBitmapRenderer
{
    /// <summary>
    /// Renders into the provided <see cref="WriteableBitmap"/>.
    /// </summary>
    /// <param name="bitmap">The target bitmap.</param>
    void Render(WriteableBitmap bitmap);
}
