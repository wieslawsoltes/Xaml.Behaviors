using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that starts drag and drop with information about drag direction.
/// </summary>
public sealed class ContextDragWithDirectionBehavior : StyledElementBehavior<Control>
{
    private Point _dragStartPoint;
    private PointerEventArgs? _triggerEvent;
    private bool _lock;
    private bool _captured;

    /// <summary>
    /// Identifies the <see cref="Context"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ContextDragWithDirectionBehavior, object?>(nameof(Context));

    /// <summary>
    /// Identifies the <see cref="Handler"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IDragHandler?> HandlerProperty =
        AvaloniaProperty.Register<ContextDragWithDirectionBehavior, IDragHandler?>(nameof(Handler));

    /// <summary>
    /// Identifies the <see cref="HorizontalDragThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> HorizontalDragThresholdProperty =
        AvaloniaProperty.Register<ContextDragWithDirectionBehavior, double>(nameof(HorizontalDragThreshold), 3);

    /// <summary>
    /// Identifies the <see cref="VerticalDragThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> VerticalDragThresholdProperty =
        AvaloniaProperty.Register<ContextDragWithDirectionBehavior, double>(nameof(VerticalDragThreshold), 3);

    /// <summary>
    /// Gets or sets the context used for drag operations.
    /// </summary>
    public object? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <summary>
    /// Gets or sets the drag handler to notify.
    /// </summary>
    public IDragHandler? Handler
    {
        get => GetValue(HandlerProperty);
        set => SetValue(HandlerProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal drag threshold.
    /// </summary>
    public double HorizontalDragThreshold
    {
        get => GetValue(HorizontalDragThresholdProperty);
        set => SetValue(HorizontalDragThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical drag threshold.
    /// </summary>
    public double VerticalDragThreshold
    {
        get => GetValue(VerticalDragThresholdProperty);
        set => SetValue(VerticalDragThresholdProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
        AssociatedObject?.RemoveHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
        AssociatedObject?.RemoveHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        AssociatedObject?.RemoveHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost);
    }

    private async Task DoDragDrop(PointerEventArgs triggerEvent, object? value, string direction)
    {
        var data = new DataObject();
        data.Set(ContextDropBehavior.DataFormat, value!);
        data.Set("direction", direction);

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

        await DragDrop.DoDragDrop(triggerEvent, data, effect);
    }

    private void Released()
    {
        _triggerEvent = null;
        _lock = false;
    }

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control && AssociatedObject?.DataContext == control.DataContext)
            {
                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _lock = true;
                _captured = true;
            }
        }
    }

    private void AssociatedObject_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left && _triggerEvent is not null)
            {
                Released();
            }

            _captured = false;
        }
    }

    private async void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured && properties.IsLeftButtonPressed && _triggerEvent is not null)
        {
            var point = e.GetPosition(null);
            var diff = _dragStartPoint - point;
            var horizontal = HorizontalDragThreshold;
            var vertical = VerticalDragThreshold;

            if (Math.Abs(diff.X) > horizontal || Math.Abs(diff.Y) > vertical)
            {
                if (_lock)
                {
                    _lock = false;
                }
                else
                {
                    return;
                }

                var context = Context ?? AssociatedObject?.DataContext;

                Handler?.BeforeDragDrop(sender, _triggerEvent, context);

                await DoDragDrop(_triggerEvent, context, diff.Y > 0 ? "up" : "down");

                Handler?.AfterDragDrop(sender, _triggerEvent, context);

                _triggerEvent = null;
            }
        }
    }

    private void AssociatedObject_CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Released();
        _captured = false;
    }
}
