using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Provides visuals that represent a dragged control.
/// </summary>
public interface IDragVisualProvider
{
    /// <summary>
    /// Shows the drag visual for the specified control.
    /// </summary>
    /// <param name="control">The control being dragged.</param>
    void Show(Control control);

    /// <summary>
    /// Hides the drag visual for the specified control.
    /// </summary>
    /// <param name="control">The control being dragged.</param>
    void Hide(Control control);
}
