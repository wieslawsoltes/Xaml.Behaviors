using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

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
public partial class ManagedContextDropBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Gets or sets the accepted managed data format.
    /// </summary>
    [StyledProperty(DefaultValue = "Context")]
    public partial string AcceptDataFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether drop is allowed.
    /// </summary>
    [StyledProperty(DefaultValue = true)]
    public partial bool AllowDrop { get; set; }

    /// <summary>
    /// Gets or sets an optional CSS-like class applied while the pointer is over the target during drag.
    /// </summary>
    [StyledProperty]
    public partial string? OverClass { get; set; }

    /// <summary>
    /// Gets or sets the context value supplied to the drop handler.
    /// </summary>
    [StyledProperty]
    public partial object? Context { get; set; }

    /// <summary>
    /// Gets or sets the handler that receives managed drag-drop notifications.
    /// </summary>
    [StyledProperty]
    public partial IDropHandler? Handler { get; set; }

    private bool _isOver;
    private bool _wantsDrop; // tracks whether pseudo class is applied
    private DragDropServiceSubscription? _dragDropSubscription;
    private const string DropTargetPseudoClass = "droptarget";
    private const string DragOverPseudoClass = "dragover";

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        _dragDropSubscription ??= new DragDropServiceSubscription(this);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DetachManagedDragEvents();

        base.OnDetachedFromVisualTree();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        DetachManagedDragEvents();

        base.OnDetaching();
    }

    private void DetachManagedDragEvents()
    {
        _dragDropSubscription?.Dispose();
        _dragDropSubscription = null;
        UpdateOver(false);
        UpdateWantsDrop(false);
    }

    private void OnDragStarted()
    {
        UpdateOver(false);
        var target = AssociatedObject;
        if (target is null || !target.IsEffectivelyVisible)
        {
            UpdateWantsDrop(false);
            return;
        }
        var svc = ManagedDragDropService.Instance;
        var compatible = AllowDrop && Handler is not null && svc.IsDragging && string.Equals(svc.DataFormat, AcceptDataFormat, StringComparison.Ordinal);
        if (compatible)
        {
            try
            {
                Point local = default;
                if (TopLevel.GetTopLevel(target) is TopLevel tl)
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
        if (!target.IsEffectivelyVisible)
        {
            if (_isOver)
            {
                UpdateOver(false);
                InvokeHandlerLeave();
            }
            UpdateWantsDrop(false);
            return;
        }

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

        if (TopLevel.GetTopLevel(target) is not TopLevel tl)
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
        if (!target.IsEffectivelyVisible)
        {
            if (_isOver)
                InvokeHandlerLeave();
            UpdateOver(false);
            UpdateWantsDrop(false);
            return;
        }

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
            IDataTransfer data = CreateDataTransfer(svc.Payload, svc.DataFormat);
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

    internal static IDataTransfer CreateDataTransfer(object? payload, string? format)
    {
        if (payload is not null && format is not null)
        {
            // Avalonia 12 only exposes public application-format factories for string and byte[].
            // Preserve the managed in-process payload in a custom IDataTransfer wrapper so callers
            // can still recover the original object instead of a stringified surrogate.
            return ManagedPayloadDataTransfer.Create(format, payload);
        }

        return new DataTransfer();
    }

    private void InvokeHandlerEnter()
    {
        var handler = Handler;
        var target = AssociatedObject;
        var svc = ManagedDragDropService.Instance;
        if (handler is null || target is null || !target.IsEffectivelyVisible || !AllowDrop || !svc.IsDragging || !string.Equals(svc.DataFormat, AcceptDataFormat, StringComparison.Ordinal))
            return;
        var tl = TopLevel.GetTopLevel(target);
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
        if (handler is null || target is null || !target.IsEffectivelyVisible)
            return;
        var e = CreateDragEventArgs(DragDrop.DragOverEvent, localPosition, svc);
        if (e is not null)
            handler.Over(target, e, svc.Payload, Context ?? target.DataContext);
    }

    private void InvokeHandlerDrop(ManagedDragDropService svc)
    {
        var handler = Handler;
        var target = AssociatedObject;
        if (handler is null || target is null || !target.IsEffectivelyVisible || !_isOver)
            return;
        var tl = TopLevel.GetTopLevel(target);
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

    private sealed class DragDropServiceSubscription : IDisposable
    {
        private readonly WeakReference<ManagedContextDropBehavior> _owner;
        private bool _disposed;

        public DragDropServiceSubscription(ManagedContextDropBehavior owner)
        {
            _owner = new WeakReference<ManagedContextDropBehavior>(owner);

            var service = ManagedDragDropService.Instance;
            service.DragStarted += OnDragStarted;
            service.DragMoved += OnDragMoved;
            service.DragEnded += OnDragEnded;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            var service = ManagedDragDropService.Instance;
            service.DragStarted -= OnDragStarted;
            service.DragMoved -= OnDragMoved;
            service.DragEnded -= OnDragEnded;
        }

        private void OnDragStarted()
        {
            if (_owner.TryGetTarget(out var owner))
            {
                owner.OnDragStarted();
            }
            else
            {
                Dispose();
            }
        }

        private void OnDragMoved()
        {
            if (_owner.TryGetTarget(out var owner))
            {
                owner.OnDragMoved();
            }
            else
            {
                Dispose();
            }
        }

        private void OnDragEnded()
        {
            if (_owner.TryGetTarget(out var owner))
            {
                owner.OnDragEnded();
            }
            else
            {
                Dispose();
            }
        }
    }
}
