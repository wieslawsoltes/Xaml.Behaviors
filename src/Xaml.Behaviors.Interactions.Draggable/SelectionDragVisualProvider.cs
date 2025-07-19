using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Default <see cref="IDragVisualProvider"/> implementation that uses
/// <see cref="SelectionAdorner"/>.
/// </summary>
public class SelectionDragVisualProvider : IDragVisualProvider
{
    private readonly Dictionary<Control, Control> _adorners = new();

    /// <inheritdoc />
    public void Show(Control control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);
        if (layer is null)
        {
            return;
        }

        var adorner = new SelectionAdorner
        {
            [AdornerLayer.AdornedElementProperty] = control
        };

        ((ISetLogicalParent)adorner).SetParent(control);
        layer.Children.Add(adorner);

        _adorners[control] = adorner;
    }

    /// <inheritdoc />
    public void Hide(Control control)
    {
        if (!_adorners.TryGetValue(control, out var adorner))
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(control);
        if (layer is null)
        {
            return;
        }

        layer.Children.Remove(adorner);
        ((ISetLogicalParent)adorner).SetParent(null);
        _adorners.Remove(control);
    }
}
