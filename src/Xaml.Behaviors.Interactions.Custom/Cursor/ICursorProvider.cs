using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Provides a custom cursor instance.
/// </summary>
public interface ICursorProvider
{
    /// <summary>
    /// Creates a cursor instance.
    /// </summary>
    /// <returns>The created <see cref="Cursor"/>.</returns>
    Cursor CreateCursor();
}
