using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides a lightweight, managed drag-drop state shared within the application.
/// This service coordinates drag state, payload, and positions for behaviors that
/// implement an in-process drag experience without using the OS drag-drop.
/// </summary>
public sealed class ManagedDragDropService
{
    private static readonly Lazy<ManagedDragDropService> _lazy = new(() => new ManagedDragDropService());

    /// <summary>
    /// Gets the singleton instance of the <see cref="ManagedDragDropService"/>.
    /// </summary>
    public static ManagedDragDropService Instance => _lazy.Value;

    private ManagedDragDropService() { }

    /// <summary>
    /// Gets a value indicating whether a managed drag operation is in progress.
    /// </summary>
    public bool IsDragging { get; private set; }

    /// <summary>
    /// Gets the payload being dragged.
    /// </summary>
    public object? Payload { get; private set; }

    /// <summary>
    /// Gets the data format associated with the <see cref="Payload"/>.
    /// </summary>
    public string? DataFormat { get; private set; }

    internal string? PayloadKey { get; private set; }

    /// <summary>
    /// Gets the effective drag-drop effects requested by the drag initiator.
    /// </summary>
    public DragDropEffects Effects { get; private set; }

    /// <summary>
    /// Gets the top-level where the drag originated.
    /// </summary>
    public TopLevel? OriginTopLevel { get; private set; }

    /// <summary>
    /// Gets the current pointer position in client coordinates of the <see cref="OriginTopLevel"/>.
    /// </summary>
    public Point CurrentClientPosition { get; private set; }

    /// <summary>
    /// Gets the current pointer position in screen pixel coordinates.
    /// </summary>
    public PixelPoint ScreenPosition =>
        OriginTopLevel is null ? new PixelPoint(0, 0) : OriginTopLevel.PointToScreen(CurrentClientPosition);

    /// <summary>
    /// Occurs when a managed drag operation begins.
    /// </summary>
    public event Action? DragStarted;

    /// <summary>
    /// Occurs when a managed drag operation ends.
    /// </summary>
    public event Action? DragEnded;

    /// <summary>
    /// Occurs when the pointer moves during a managed drag operation.
    /// </summary>
    public event Action? DragMoved;

    /// <summary>
    /// Begins a managed drag operation.
    /// </summary>
    /// <param name="origin">The top-level where the drag starts.</param>
    /// <param name="payload">The payload being dragged.</param>
    /// <param name="dataFormat">The data format describing the payload.</param>
    /// <param name="effects">The allowed drag-drop effects.</param>
    /// <param name="startClient">The starting pointer position in client coordinates.</param>
    public void Begin(TopLevel origin, object? payload, string? dataFormat, DragDropEffects effects, Point startClient)
    {
        IsDragging = true;
        OriginTopLevel = origin;
        Payload = payload;
        DataFormat = dataFormat;
        Effects = effects;
        CurrentClientPosition = startClient;
        PayloadKey = DragDropContextStore.Add(payload);
        DragStarted?.Invoke();
    }

    /// <summary>
    /// Updates the current pointer position during a managed drag operation.
    /// </summary>
    /// <param name="clientPosition">The current pointer position in client coordinates.</param>
    public void Move(Point clientPosition)
    {
        if (!IsDragging) return;
        CurrentClientPosition = clientPosition;
        DragMoved?.Invoke();
    }

    /// <summary>
    /// Ends the current managed drag operation and resets state.
    /// </summary>
    public void End()
    {
        if (!IsDragging) return;
        IsDragging = false;
        DragEnded?.Invoke();
        DragDropContextStore.Remove(PayloadKey);
        PayloadKey = null;
        OriginTopLevel = null;
        Payload = null;
        DataFormat = null;
        Effects = DragDropEffects.None;
        CurrentClientPosition = default;
    }
}
