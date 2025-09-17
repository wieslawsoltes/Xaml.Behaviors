using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides a lightweight preview window shown during a managed drag operation.
/// </summary>
public static class DragPreviewService
{
    private static Controls.DragPreviewWindow? _window;

    /// <summary>
    /// Shows a drag preview near the current pointer position.
    /// </summary>
    /// <param name="content">Content to render inside the preview window.</param>
    /// <param name="template">Optional template to use for the <paramref name="content"/>.</param>
    /// <param name="origin">The top-level where the drag originates.</param>
    /// <param name="clientPosition">The current pointer position in <paramref name="origin"/> client coordinates.</param>
    /// <param name="logicalOffset">Logical offset added to the preview position in client coordinates.</param>
    /// <param name="opacity">The preview window opacity, clamped to 0..1.</param>
    public static void Show(
        object? content,
        IDataTemplate? template,
        TopLevel origin,
        Point clientPosition,
        Point logicalOffset,
        double opacity = 0.65)
    {
        Hide();

        _window = new Controls.DragPreviewWindow();

        if (template is not null)
        {
            _window.Content = new ContentPresenter { Content = content, ContentTemplate = template };
        }
        else
        {
            _window.Content = content;
        }

        var screenPos = origin.PointToScreen(clientPosition + logicalOffset);

        // Manual clamp for cross-targeting frameworks
        var o = opacity;
        if (o < 0) o = 0;
        else if (o > 1) o = 1;
        _window.Opacity = o;

        _window.Position = screenPos;
        _window.Show();
    }

    /// <summary>
    /// Moves the drag preview to a new client position.
    /// </summary>
    /// <param name="origin">The top-level where the drag originates.</param>
    /// <param name="clientPosition">The new pointer position in client coordinates.</param>
    /// <param name="logicalOffset">Logical offset added to the preview position in client coordinates.</param>
    public static void Move(TopLevel origin, Point clientPosition, Point logicalOffset)
    {
        if (_window is null)
            return;

        var screenPos = origin.PointToScreen(clientPosition + logicalOffset);
        _window.Position = screenPos;
    }

    /// <summary>
    /// Hides the preview if shown.
    /// </summary>
    public static void Hide()
    {
        try
        {
            _window?.Close();
        }
        catch
        {
            // ignore
        }
        finally
        {
            _window = null;
        }
    }
}
