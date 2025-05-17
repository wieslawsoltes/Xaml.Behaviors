using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides callbacks for drag operations.
/// </summary>
public interface IDragHandler
{
    /// <summary>
    /// Called before the drag operation begins.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    void BeforeDragDrop(object? sender, PointerEventArgs e, object? context);

    /// <summary>
    /// Called after the drag operation completes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    void AfterDragDrop(object? sender, PointerEventArgs e, object? context);
}