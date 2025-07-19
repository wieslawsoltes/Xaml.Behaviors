using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides helpers for starting drag operations.
/// </summary>
public static class DragService
{
    /// <summary>
    /// Creates a <see cref="DataObject"/> containing context data and any additional entries.
    /// </summary>
    /// <param name="context">Context object to store using <see cref="ContextDropBehaviorBase.DataFormat"/>.</param>
    /// <param name="extras">Optional extra data entries.</param>
    /// <returns>The created <see cref="DataObject"/>.</returns>
    public static DataObject CreateDataObject(object? context, params (string format, object? data)[] extras)
    {
        var dataObject = new DataObject();
        dataObject.Set(ContextDropBehaviorBase.DataFormat, context!);

        foreach (var (format, data) in extras)
        {
            dataObject.Set(format, data!);
        }

        return dataObject;
    }

    /// <summary>
    /// Determines allowed drag effects based on key modifiers.
    /// </summary>
    /// <param name="triggerEvent">Event containing modifier state.</param>
    /// <returns>The drag effects.</returns>
    public static DragDropEffects GetDragEffects(PointerEventArgs triggerEvent)
    {
        var effect = DragDropEffects.None;

        if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            effect |= DragDropEffects.Link;
        }
        else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            effect |= DragDropEffects.Move;
        }
        else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            effect |= DragDropEffects.Copy;
        }
        else
        {
            effect |= DragDropEffects.Move;
        }

        return effect;
    }

    /// <summary>
    /// Starts a drag operation using the provided data.
    /// </summary>
    /// <param name="triggerEvent">Pointer event that initiated the drag.</param>
    /// <param name="data">Data object containing drag data.</param>
    public static Task StartDragAsync(PointerEventArgs triggerEvent, DataObject data)
    {
        var effect = GetDragEffects(triggerEvent);
        return DragDrop.DoDragDrop(triggerEvent, data, effect);
    }

    /// <summary>
    /// Creates a data object and begins a drag using standard options.
    /// </summary>
    /// <param name="triggerEvent">Pointer event that initiated the drag.</param>
    /// <param name="context">Context data to include.</param>
    /// <param name="extras">Additional data entries.</param>
    public static Task StartDragAsync(
        PointerEventArgs triggerEvent,
        object? context,
        params (string format, object? data)[] extras)
    {
        var dataObject = CreateDataObject(context, extras);
        return StartDragAsync(triggerEvent, dataObject);
    }
}
