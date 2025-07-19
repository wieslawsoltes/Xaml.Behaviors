// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior base class that starts a drag operation using the associated context data.
/// </summary>
public abstract class ContextDragBehaviorBase : StyledElementBehavior<Control>
{
    private Point _dragStartPoint;
    private PointerEventArgs? _triggerEvent;
    private bool _lock;
    private bool _captured;
    private Control? _previewAdorner;

    /// <summary>
    /// Identifies the <see cref="Context"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ContextDragBehaviorBase, object?>(nameof(Context));

    /// <summary>
    /// Identifies the <see cref="HorizontalDragThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> HorizontalDragThresholdProperty =
        AvaloniaProperty.Register<ContextDragBehaviorBase, double>(nameof(HorizontalDragThreshold), 3);

    /// <summary>
    /// Identifies the <see cref="VerticalDragThreshold"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> VerticalDragThresholdProperty =
        AvaloniaProperty.Register<ContextDragBehaviorBase, double>(nameof(VerticalDragThreshold), 3);

    /// <summary>
    /// Identifies the <see cref="DragPreviewTemplate"/> attached avalonia property.
    /// </summary>
    public static readonly AttachedProperty<IDataTemplate?> DragPreviewTemplateProperty =
        AvaloniaProperty.RegisterAttached<ContextDragBehaviorBase, Control, IDataTemplate?>(
            "DragPreviewTemplate");

    /// <summary>
    /// Gets the <see cref="IDataTemplate"/> used to create the drag preview.
    /// </summary>
    /// <param name="control">The control to read the template from.</param>
    /// <returns>The <see cref="IDataTemplate"/> value.</returns>
    public static IDataTemplate? GetDragPreviewTemplate(Control control) =>
        control.GetValue(DragPreviewTemplateProperty);

    /// <summary>
    /// Sets the <see cref="IDataTemplate"/> used to create the drag preview.
    /// </summary>
    /// <param name="control">The control to set the template on.</param>
    /// <param name="value">The template.</param>
    public static void SetDragPreviewTemplate(Control control, IDataTemplate? value) =>
        control.SetValue(DragPreviewTemplateProperty, value);

    /// <summary>
    /// Gets or sets context data passed to the drag handler.
    /// </summary>
    public object? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal distance in pixels required to start a drag.
    /// </summary>
    public double HorizontalDragThreshold
    {
        get => GetValue(HorizontalDragThresholdProperty);
        set => SetValue(HorizontalDragThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical distance in pixels required to start a drag.
    /// </summary>
    public double VerticalDragThreshold
    {
        get => GetValue(VerticalDragThresholdProperty);
        set => SetValue(VerticalDragThresholdProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
        AssociatedObject?.RemoveHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
        AssociatedObject?.RemoveHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        AssociatedObject?.RemoveHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost);
    }

    /// <summary>
    /// Called before the drag operation begins.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    protected abstract void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context);

    /// <summary>
    /// Called after the drag operation completes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    protected abstract void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context);

    private async Task DoDragDrop(PointerEventArgs triggerEvent, object? value)
    {
        var data = new DataObject();
        data.Set(ContextDropBehaviorBase.DataFormat, value!);

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

    private void AddPreviewAdorner()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var template = GetDragPreviewTemplate(AssociatedObject);
        if (template is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        if (layer is null)
        {
            return;
        }

        _previewAdorner = new Avalonia.Xaml.Interactions.Draggable.DragPreviewAdorner
        {
            Content = template.Build(AssociatedObject.DataContext),
            [AdornerLayer.AdornedElementProperty] = AssociatedObject
        };

        ((ISetLogicalParent)_previewAdorner).SetParent(AssociatedObject);
        layer.Children.Add(_previewAdorner);
    }

    private void RemovePreviewAdorner()
    {
        if (AssociatedObject is null || _previewAdorner is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        if (layer is null)
        {
            return;
        }

        layer.Children.Remove(_previewAdorner);
        ((ISetLogicalParent)_previewAdorner).SetParent(null);
        _previewAdorner = null;
    }

    private void Released()
    {
        _triggerEvent = null;
        _lock = false;
        RemovePreviewAdorner();
    }

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control
                && AssociatedObject?.DataContext == control.DataContext)
            {
                if ((control as ISelectable
                    ?? control.Parent as ISelectable
                    ?? control.FindLogicalAncestorOfType<ISelectable>())
                        ?.IsSelected
                    ?? false)
                {
                    e.Handled = true; //avoid deselection on drag
                }

                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _lock = true;
                _captured = true;

                return;
            }
        }
        e.Handled = false;
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
        if (_captured
            && properties.IsLeftButtonPressed &&
            _triggerEvent is not null)
        {
            var point = e.GetPosition(null);
            var diff = _dragStartPoint - point;
            var horizontalDragThreshold = HorizontalDragThreshold;
            var verticalDragThreshold = VerticalDragThreshold;

            if (Math.Abs(diff.X) > horizontalDragThreshold || Math.Abs(diff.Y) > verticalDragThreshold)
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

                AddPreviewAdorner();
                OnBeforeDragDrop(sender, _triggerEvent, context);
                
                await DoDragDrop(_triggerEvent, context);

                OnAfterDragDrop(sender, _triggerEvent, context);
                RemovePreviewAdorner();

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
