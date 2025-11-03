// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Allows reordering of items inside an <see cref="ItemsControl"/> while displaying
/// a placeholder at the insertion point.
/// </summary>
public class ListReorderDragBehavior : ItemDragBehavior
{
    /// <summary>
    /// Identifies the <see cref="PlaceholderTemplate"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ITemplate?> PlaceholderTemplateProperty =
        AvaloniaProperty.Register<ListReorderDragBehavior, ITemplate?>(nameof(PlaceholderTemplate));

    private bool _enableDrag;
    private bool _dragStarted;
    private Point _start;
    private int _draggedIndex;
    private int _targetIndex;
    private ItemsControl? _itemsControl;
    private Control? _draggedContainer;
    private bool _captured;
    private Control? _placeholder;
    private Control? _placeholderContainer;

    /// <summary>
    /// Gets or sets template used to build placeholder shown while reordering.
    /// </summary>
    public ITemplate? PlaceholderTemplate
    {
        get => GetValue(PlaceholderTemplateProperty);
        set => SetValue(PlaceholderTemplateProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed
            && AssociatedObject?.Parent is ItemsControl itemsControl && IsEnabled)
        {
            _enableDrag = true;
            _dragStarted = false;
            _start = e.GetPosition(itemsControl);
            _draggedIndex = -1;
            _targetIndex = -1;
            _itemsControl = itemsControl;
            _draggedContainer = AssociatedObject;
            _captured = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                RemovePlaceholder();
            }
            _captured = false;
        }
    }

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        RemovePlaceholder();
        _captured = false;
    }

    private void RemovePlaceholder()
    {
        if (_placeholderContainer is not null && _placeholder is not null)
        {
            var layer = AdornerLayer.GetAdornerLayer(_placeholderContainer);
            if (layer is not null)
            {
                layer.Children.Remove(_placeholder);
            }
            ((ISetLogicalParent)_placeholder).SetParent(null);
        }

        _placeholder = null;
        _placeholderContainer = null;
        _enableDrag = false;
        _dragStarted = false;
        _itemsControl = null;
        _draggedContainer = null;
    }

    private void AddPlaceholder(Control container)
    {
        if (PlaceholderTemplate is null)
        {
            return;
        }

        if (ReferenceEquals(container, _placeholderContainer))
        {
            return;
        }

        RemovePlaceholder();

        var layer = AdornerLayer.GetAdornerLayer(container);
        if (layer is null)
        {
            return;
        }

        if (PlaceholderTemplate.Build() is Control adorner)
        {
            adorner[AdornerLayer.AdornedElementProperty] = container;
            ((ISetLogicalParent)adorner).SetParent(container);
            layer.Children.Add(adorner);
            _placeholder = adorner;
            _placeholderContainer = container;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured && properties.IsLeftButtonPressed)
        {
            if (_itemsControl?.Items is null || !_enableDrag)
            {
                return;
            }

            var orientation = Orientation;
            var position = e.GetPosition(_itemsControl);
            var delta = orientation == Orientation.Horizontal ? position.X - _start.X : position.Y - _start.Y;

            if (!_dragStarted)
            {
                var diff = _start - position;
                if (orientation == Orientation.Horizontal)
                {
                    if (Math.Abs(diff.X) > HorizontalDragThreshold)
                    {
                        _dragStarted = true;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (Math.Abs(diff.Y) > VerticalDragThreshold)
                    {
                        _dragStarted = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (_draggedContainer is null)
            {
                return;
            }

            _draggedIndex = _itemsControl.IndexFromContainer(_draggedContainer);
            _targetIndex = -1;

            var draggedBounds = _draggedContainer.Bounds;

            var draggedStart = orientation == Orientation.Horizontal ? draggedBounds.X : draggedBounds.Y;

            var draggedDeltaStart = orientation == Orientation.Horizontal
                ? draggedBounds.X + delta
                : draggedBounds.Y + delta;

            var draggedDeltaEnd = orientation == Orientation.Horizontal
                ? draggedBounds.X + delta + draggedBounds.Width
                : draggedBounds.Y + delta + draggedBounds.Height;

            var i = 0;

            foreach (var _ in _itemsControl.Items)
            {
                var targetContainer = _itemsControl.ContainerFromIndex(i);
                if (targetContainer is null || ReferenceEquals(targetContainer, _draggedContainer))
                {
                    i++;
                    continue;
                }

                var targetBounds = targetContainer.Bounds;

                var targetStart = orientation == Orientation.Horizontal ? targetBounds.X : targetBounds.Y;

                var targetMid = orientation == Orientation.Horizontal
                    ? targetBounds.X + targetBounds.Width / 2
                    : targetBounds.Y + targetBounds.Height / 2;

                var targetIndex = _itemsControl.IndexFromContainer(targetContainer);

                if (targetStart > draggedStart && draggedDeltaEnd >= targetMid)
                {
                    _targetIndex = _targetIndex == -1 ? targetIndex :
                        targetIndex > _targetIndex ? targetIndex : _targetIndex;
                }
                else if (targetStart < draggedStart && draggedDeltaStart <= targetMid)
                {
                    _targetIndex = _targetIndex == -1 ? targetIndex :
                        targetIndex < _targetIndex ? targetIndex : _targetIndex;
                }

                i++;
            }

            if (_targetIndex >= 0)
            {
                var container = _itemsControl.ContainerFromIndex(_targetIndex);
                if (container is not null)
                {
                    AddPlaceholder(container);
                }
            }
            else
            {
                RemovePlaceholder();
            }
        }
    }
}

