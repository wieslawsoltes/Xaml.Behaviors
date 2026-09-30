using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that initiates an in-process managed drag with an optional preview window.
/// This avoids OS drag-drop and integrates with <see cref="ManagedDragDropService"/>.
/// </summary>
public partial class ManagedContextDragBehavior : StyledElementBehavior<Control>
{
    private static bool s_isDragging;

    private Point _dragStartPoint;
    private PointerEventArgs? _triggerEvent;
    private bool _lock;
    private bool _captured;

    private TopLevel? _topLevel;
    private bool _internalDragging;
    private TaskCompletionSource<bool>? _internalDragTcs;

    // Stores the calculated preview offset (TopLeftOfControl - PointerPosition) in TopLevel client coordinates when enabled
    private Point? _calculatedPreviewOffset;

    /// <summary>
    /// Gets or sets the context value used as a drag payload when the drag starts.
    /// </summary>
    [StyledProperty]
    public partial object? Context { get; set; }

    /// <summary>
    /// Gets or sets the template used to render the drag preview.
    /// </summary>
    [StyledProperty]
    public partial IDataTemplate? PreviewTemplate { get; set; }

    /// <summary>
    /// Gets or sets the minimal horizontal distance required to start dragging.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double HorizontalDragThreshold { get; set; }

    /// <summary>
    /// Gets or sets the minimal vertical distance required to start dragging.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double VerticalDragThreshold { get; set; }

    /// <summary>
    /// Gets or sets a fixed logical offset added to the preview position.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "new Point(0, 0)")]
    public partial Point PreviewOffset { get; set; }

    /// <summary>
    /// Gets or sets the data format name used to identify the managed payload.
    /// </summary>
    [StyledProperty(DefaultValue = "Context")]
    public partial string DataFormat { get; set; }

    /// <summary>
    /// Gets or sets whether to compute a pointer-relative preview offset automatically.
    /// </summary>
    [StyledProperty(DefaultValue = true)]
    public partial bool UsePointerRelativePreviewOffset { get; set; }

    /// <summary>
    /// Gets or sets the preview window opacity.
    /// </summary>
    [StyledProperty(DefaultValue = 0.65)]
    public partial double PreviewOpacity { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        var ao = AssociatedObject;
        _topLevel = ao != null ? TopLevel.GetTopLevel(ao) : null;
        ao?.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        ao?.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        ao?.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        ao?.AddHandler(InputElement.PointerCaptureLostEvent, OnCaptureLost, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var ao = AssociatedObject;
        ao?.RemoveRoutedEventHandler(InputElement.PointerPressedEvent, OnPointerPressed);
        ao?.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
        ao?.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, OnPointerMoved);
        ao?.RemoveRoutedEventHandler(InputElement.PointerCaptureLostEvent, OnCaptureLost);
        DetachTopLevelHandlers();
        _topLevel = null;
    }

    private void AttachTopLevelHandlers()
    {
        if (_topLevel is null) return;
        _topLevel.AddHandler(InputElement.PointerMovedEvent, OnTopLevelPointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        _topLevel.AddHandler(InputElement.PointerReleasedEvent, OnTopLevelPointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        _topLevel.AddHandler(InputElement.PointerCaptureLostEvent, OnTopLevelPointerCaptureLost, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
    }

    private void DetachTopLevelHandlers()
    {
        if (_topLevel is null) return;
        _topLevel.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, OnTopLevelPointerMoved);
        _topLevel.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, OnTopLevelPointerReleased);
        _topLevel.RemoveRoutedEventHandler(InputElement.PointerCaptureLostEvent, OnTopLevelPointerCaptureLost);
    }

    private static DragDropEffects GetDesiredEffects(PointerEventArgs triggerEvent)
    {
        var effect = DragDropEffects.Move;
        if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Alt)) effect = DragDropEffects.Link;
        else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Control)) effect = DragDropEffects.Copy;
        return effect;
    }

    private static string EffectsToStatus(DragDropEffects effects)
    {
        return effects switch
        {
            DragDropEffects.Move => "Move",
            DragDropEffects.Copy => "Copy",
            DragDropEffects.Link => "Link",
            DragDropEffects.None => string.Empty,
            _ => effects.ToString()
        };
    }

    private async Task StartInternalDragAsync(PointerEventArgs triggerEvent, object value)
    {
        var effects = GetDesiredEffects(triggerEvent);
        var tl = _topLevel ?? TopLevel.GetTopLevel(AssociatedObject);
        if (tl is null) return;

        var client = triggerEvent.GetPosition(tl);
        var previewOffset = UsePointerRelativePreviewOffset && _calculatedPreviewOffset.HasValue
            ? _calculatedPreviewOffset.Value
            : PreviewOffset;

        DragPreviewService.Show(value, PreviewTemplate, tl, client, previewOffset, PreviewOpacity);

        try
        {
            s_isDragging = true;
            _internalDragging = true;
            _internalDragTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            try { if (AssociatedObject != null) triggerEvent.Pointer?.Capture(AssociatedObject); } catch { }
            AttachTopLevelHandlers();
            ManagedDragDropService.Instance.Begin(tl, value, DataFormat, effects, client);
            if (AssociatedObject != null)
                AssociatedObject.DetachedFromVisualTree += AssociatedObject_DetachedFromVisualTree;
            await _internalDragTcs.Task.ConfigureAwait(true);
        }
        finally
        {
            ManagedDragDropService.Instance.End();
            if (AssociatedObject != null)
                AssociatedObject.DetachedFromVisualTree -= AssociatedObject_DetachedFromVisualTree;
            DetachTopLevelHandlers();
            try { triggerEvent.Pointer?.Capture(null); } catch { }
            DragPreviewService.Hide();
            _internalDragging = false;
            _internalDragTcs = null;
            s_isDragging = false;
            _calculatedPreviewOffset = null;
        }
    }

    private void AssociatedObject_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _internalDragging = false;
        _internalDragTcs?.TrySetResult(true);
    }

    private void OnTopLevelPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_internalDragging || _topLevel is null) return;
        var client = e.GetPosition(_topLevel);
        var previewOffset = UsePointerRelativePreviewOffset && _calculatedPreviewOffset.HasValue
            ? _calculatedPreviewOffset.Value
            : PreviewOffset;
        DragPreviewService.Move(_topLevel, client, previewOffset);
        ManagedDragDropService.Instance.Move(client);
    }

    private void OnTopLevelPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_internalDragging) return;
        _internalDragging = false;
        _internalDragTcs?.TrySetResult(true);
    }

    private void OnTopLevelPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (!_internalDragging) return;
        _internalDragging = false;
        _internalDragTcs?.TrySetResult(true);
    }

    private void Release()
    {
        _triggerEvent = null;
        _lock = false;
        _calculatedPreviewOffset = null;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var ao = AssociatedObject;
        if (ao is null) return;
        var properties = e.GetCurrentPoint(ao).Properties;
        if (properties.IsLeftButtonPressed && IsEnabled)
        {
            if (e.Source is Control control && ao.DataContext == control.DataContext)
            {
                if ((control as ISelectable ?? control.Parent as ISelectable ?? control.FindLogicalAncestorOfType<ISelectable>())?.IsSelected ?? false)
                    e.Handled = true;

                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _lock = true;
                _captured = true;

                // Compute the cursor-relative preview offset if enabled in TopLevel coordinates using AssociatedObject top-left
                if (UsePointerRelativePreviewOffset)
                {
                    _calculatedPreviewOffset = -e.GetPosition(ao);
                }
            }
        }
        e.Handled = false;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left && _triggerEvent is not null)
            {
                Release();
            }
            _captured = false;
        }
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var ao = AssociatedObject;
        if (ao is null) return;
        if (!_captured || _triggerEvent is null || !IsEnabled)
            return;

        if (s_isDragging)
            return;

        var properties = e.GetCurrentPoint(ao).Properties;
        if (!properties.IsLeftButtonPressed)
            return;

        var point = e.GetPosition(null);
        var diff = _dragStartPoint - point;
        if (Math.Abs(diff.X) > HorizontalDragThreshold || Math.Abs(diff.Y) > VerticalDragThreshold)
        {
            if (_lock) _lock = false; else return;

            var context = Context ?? AssociatedObject?.DataContext;
            if (context is null)
                return;

            await StartInternalDragAsync(_triggerEvent, context);
            _triggerEvent = null;
        }
    }

    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Release();
        _captured = false;
        if (_internalDragging)
        {
            _internalDragging = false;
            _internalDragTcs?.TrySetResult(true);
        }
    }
}
