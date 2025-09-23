using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Arguments used by managed drop handlers to describe the drop context.
/// </summary>
public sealed class ManagedContextDropArgs
{
    /// <summary>
    /// Gets or sets the payload transported by the managed drag operation.
    /// </summary>
    public object? Payload { get; set; }

    /// <summary>
    /// Gets or sets the data format of the <see cref="Payload"/>.
    /// </summary>
    public string? DataFormat { get; set; }

    /// <summary>
    /// Gets or sets the drag-drop effects requested by the initiator.
    /// </summary>
    public DragDropEffects Effects { get; set; }

    /// <summary>
    /// Gets or sets the top-level that originated the drag.
    /// </summary>
    public TopLevel? OriginTopLevel { get; set; }

    /// <summary>
    /// Gets or sets the pointer position local to the drop target control.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the pointer position in screen pixel coordinates.
    /// </summary>
    public PixelPoint ScreenPosition { get; set; }
}

/// <summary>
/// Drop target behavior that integrates with <see cref="ManagedDragDropService"/>.
/// It mirrors the semantics of <see cref="ContextDropBehavior"/> but works entirely in-process.
/// </summary>
[PseudoClasses("wants-drop", "dragover")]
public class ManagedContextDropBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="AcceptDataFormat"/> property.
    /// </summary>
    public static readonly StyledProperty<string> AcceptDataFormatProperty =
        AvaloniaProperty.Register<ManagedContextDropBehavior, string>(nameof(AcceptDataFormat), "Context");

    /// <summary>
    /// Identifies the <see cref="AllowDrop"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowDropProperty =
        AvaloniaProperty.Register<ManagedContextDropBehavior, bool>(nameof(AllowDrop), true);

    /// <summary>
    /// Identifies the <see cref="OverClass"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> OverClassProperty =
        AvaloniaProperty.Register<ManagedContextDropBehavior, string?>(nameof(OverClass));

    /// <summary>
    /// Identifies the <see cref="Context"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ManagedContextDropBehavior, object?>(nameof(Context));

    /// <summary>
    /// Identifies the <see cref="Handler"/> property.
    /// </summary>
    public static readonly StyledProperty<IDropHandler?> HandlerProperty =
        AvaloniaProperty.Register<ManagedContextDropBehavior, IDropHandler?>(nameof(Handler));

    /// <summary>
    /// Gets or sets the accepted managed data format.
    /// </summary>
    public string AcceptDataFormat
    {
        get => GetValue(AcceptDataFormatProperty);
        set => SetValue(AcceptDataFormatProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether drop is allowed.
    /// </summary>
    public bool AllowDrop
    {
        get => GetValue(AllowDropProperty);
        set => SetValue(AllowDropProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional CSS-like class applied while the pointer is over the target during drag.
    /// </summary>
    public string? OverClass
    {
        get => GetValue(OverClassProperty);
        set => SetValue(OverClassProperty, value);
    }

    /// <summary>
    /// Gets or sets the context value supplied to the drop handler.
    /// </summary>
    public object? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <summary>
    /// Gets or sets the handler that receives managed drag-drop notifications.
    /// </summary>
    public IDropHandler? Handler
    {
        get => GetValue(HandlerProperty);
        set => SetValue(HandlerProperty, value);
    }

    private bool _isOver;
    private bool _wantsDrop; // tracks whether pseudo class is applied
    private const string DropTargetPseudoClass = "droptarget";
    private const string DragOverPseudoClass = "dragover";

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        ManagedDragDropService.Instance.DragStarted += OnDragStarted;
        ManagedDragDropService.Instance.DragMoved += OnDragMoved;
        ManagedDragDropService.Instance.DragEnded += OnDragEnded;
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        ManagedDragDropService.Instance.DragStarted -= OnDragStarted;
        ManagedDragDropService.Instance.DragMoved -= OnDragMoved;
        ManagedDragDropService.Instance.DragEnded -= OnDragEnded;
        UpdateOver(false);
        UpdateWantsDrop(false);
    }

    private void OnDragStarted()
    {
        UpdateOver(false);
        var svc = ManagedDragDropService.Instance;
        var compatible = AllowDrop && Handler is not null && svc.IsDragging && string.Equals(svc.DataFormat, AcceptDataFormat, StringComparison.Ordinal);
        if (compatible)
        {
            var target = AssociatedObject;
            try
            {
                if (target is not null)
                {
                    Point local = default;
                    if (target.GetVisualRoot() is TopLevel tl)
                    {
                        var pTop = tl.PointToClient(svc.ScreenPosition);
                        local = tl.TranslatePoint(pTop, target) ?? default;
                    }
                    var e = CreateDragEventArgs(DragDrop.DragEnterEvent, local, svc);
                    if (e is null)
                    {
                        compatible = false;
                    }
                    else
                    {
                        bool valid;
                        try
                        {
                            valid = Handler!.Validate(target, e, svc.Payload, Context ?? target.DataContext, null);
                        }
                        catch
                        {
                            valid = false;
                        }
                        compatible = valid;
                    }
                }
                else
                {
                    compatible = false;
                }
            }
            catch
            {
                compatible = false;
            }
        }
        UpdateWantsDrop(compatible);
        InvokeHandlerEnter();
    }

    private void OnDragMoved()
    {
        var target = AssociatedObject;
        if (target is null || !AllowDrop)
            return;

        var svc = ManagedDragDropService.Instance;
        if (!svc.IsDragging || !string.Equals(svc.DataFormat, AcceptDataFormat, StringComparison.Ordinal))
        {
            if (_isOver)
            {
                UpdateOver(false);
                InvokeHandlerLeave();
            }
            return;
        }

        if (target.GetVisualRoot() is not TopLevel tl)
        {
            if (_isOver)
            {
                UpdateOver(false);
                InvokeHandlerLeave();
            }
            return;
        }

        var pTop = tl.PointToClient(svc.ScreenPosition);
        var pLocal = tl.TranslatePoint(pTop, target) ?? default;
        var over = pLocal.X >= 0 && pLocal.Y >= 0 && pLocal.X <= target.Bounds.Width && pLocal.Y <= target.Bounds.Height;
        if (over != _isOver)
        {
            UpdateOver(over);
            if (!over)
                InvokeHandlerLeave();
        }

        if (over)
        {
            // Commands removed; rely solely on handler.
            InvokeHandlerOver(pLocal, svc);
        }
    }

    private void OnDragEnded()
    {
        var target = AssociatedObject;
        if (target is null)
            return;

        try
        {
            var svc = ManagedDragDropService.Instance;
            if (_isOver && AllowDrop)
            {
                // Commands removed; rely solely on handler.
                InvokeHandlerDrop(svc);
            }
        }
        finally
        {
            if (_isOver)
                InvokeHandlerLeave();
            UpdateOver(false);
            UpdateWantsDrop(false);
        }
    }

    private void UpdateOver(bool over)
    {
        _isOver = over;
        var target = AssociatedObject;
        if (target is null) return;
        var cls = OverClass;
        if (!string.IsNullOrWhiteSpace(cls))
        {
            target.Classes.Set(cls!, over);
        }
        // Apply/remove dragover pseudo class
        if (target.Classes is IPseudoClasses pcDragOver)
        {
            if (over)
                pcDragOver.Add(DragOverPseudoClass);
            else
                pcDragOver.Remove(DragOverPseudoClass);
        }
    }

    private void UpdateWantsDrop(bool wants)
    {
        if (_wantsDrop == wants)
            return;
        _wantsDrop = wants;
        var target = AssociatedObject;
        if (target is null)
            return;
        if (target.Classes is IPseudoClasses pc)
        {
            if (wants)
                pc.Add(DropTargetPseudoClass);
            else
                pc.Remove(DropTargetPseudoClass);
        }
    }

    private DragEventArgs? CreateDragEventArgs(RoutedEvent<DragEventArgs> routedEvent, Point localPosition, ManagedDragDropService svc)
    {
        try
        {
            var data = new DataObject();
            if (svc.Payload is not null && svc.DataFormat is { })
            {
                data.Set(svc.DataFormat, svc.Payload);
            }
            // Use Interactive (base for Control) as required by DragEventArgs
            var target = AssociatedObject as Interactive;
            if (target is null) return null;
            var e = new DragEventArgs(routedEvent, data, target, localPosition, KeyModifiers.None);
            // Propagate current requested effects so handlers can decide behavior
            e.DragEffects = svc.Effects;
            return e;
        }
        catch
        {
            return null;
        }
    }

    private void InvokeHandlerEnter()
    {
        var handler = Handler;
        var target = AssociatedObject;
        var svc = ManagedDragDropService.Instance;
        if (handler is null || target is null || !AllowDrop || !svc.IsDragging || !string.Equals(svc.DataFormat, AcceptDataFormat, StringComparison.Ordinal))
            return;
        var tl = target.GetVisualRoot() as TopLevel;
        if (tl is null) return;
        var pTop = tl.PointToClient(svc.ScreenPosition);
        var pLocal = tl.TranslatePoint(pTop, target) ?? default;
        var e = CreateDragEventArgs(DragDrop.DragEnterEvent, pLocal, svc);
        if (e is not null)
            handler.Enter(target, e, svc.Payload, Context ?? target.DataContext);
    }

    private void InvokeHandlerOver(Point localPosition, ManagedDragDropService svc)
    {
        var handler = Handler;
        var target = AssociatedObject;
        if (handler is null || target is null)
            return;
        var e = CreateDragEventArgs(DragDrop.DragOverEvent, localPosition, svc);
        if (e is not null)
            handler.Over(target, e, svc.Payload, Context ?? target.DataContext);
    }

    private void InvokeHandlerDrop(ManagedDragDropService svc)
    {
        var handler = Handler;
        var target = AssociatedObject;
        if (handler is null || target is null || !_isOver)
            return;
        var tl = target.GetVisualRoot() as TopLevel;
        if (tl is null) return;
        var pTop = tl.PointToClient(svc.ScreenPosition);
        var pLocal = tl.TranslatePoint(pTop, target) ?? default;
        var e = CreateDragEventArgs(DragDrop.DropEvent, pLocal, svc);
        if (e is not null)
            handler.Drop(target, e, svc.Payload, Context ?? target.DataContext);
    }

    private void InvokeHandlerLeave()
    {
        var handler = Handler;
        var target = AssociatedObject;
        if (handler is null || target is null) return;
        handler.Leave(target, new RoutedEventArgs());
    }
}
