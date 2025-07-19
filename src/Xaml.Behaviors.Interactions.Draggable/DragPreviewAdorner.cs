using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Displays drag preview content in an <see cref="AdornerLayer"/>.
/// </summary>
public class DragPreviewAdorner : ContentControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragPreviewAdorner"/> class.
    /// </summary>
    public DragPreviewAdorner()
    {
        IsHitTestVisible = false;
    }
}
