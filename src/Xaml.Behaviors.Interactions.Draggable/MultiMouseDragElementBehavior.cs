// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Xaml.Interactivity;
#else
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Draggable;
#else
namespace Avalonia.Xaml.Interactions.Draggable;
#endif

/// <summary>
/// Enables dragging of multiple controls with the mouse using <see cref="TranslateTransform"/>.
/// </summary>
public partial class MultiMouseDragElementBehavior : StyledElementBehavior<Control>
{
    private bool _captured;
    private Point _start;
    private readonly Dictionary<Control, TranslateTransform> _transforms = new();
    private Control? _parent;

    /// <summary>
    /// Gets the collection of controls that should be dragged together. This is an avalonia property.
    /// </summary>
    [DirectProperty(Lazy = true, Content = true)]
#if UNO
    public partial System.Collections.ObjectModel.ObservableCollection<FrameworkElement> TargetControls { get; }
#else
    public partial AvaloniaList<Control> TargetControls { get; }
#endif

    /// <summary>
    /// Gets or sets whether dragging should be constrained to the bounds of the parent control.
    /// </summary>
    [StyledProperty]
    public partial bool ConstrainToParentBounds { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerPressedEvent, Pressed);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, Released);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, Moved);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
        }
    }

    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed && AssociatedObject?.Parent is Control parent && IsEnabled)
        {
            _parent = parent;
            _start = e.GetPosition(_parent);
            _transforms.Clear();

            AddTransform(AssociatedObject);
            foreach (var control in TargetControls)
            {
                AddTransform(control);
            }

            _captured = true;
#if UNO
            // Avalonia captures the pointer implicitly on press; WinUI only does so for touch.
            AssociatedObject.CapturePointer(e.Pointer);
#endif
        }
    }

    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured && e.InitialPressMouseButton == MouseButton.Left)
        {
            EndDrag();
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_captured)
        {
            EndDrag();
        }
    }

    private void Moved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (!_captured || !properties.IsLeftButtonPressed || _parent is null || !IsEnabled)
        {
            return;
        }

        var position = e.GetPosition(_parent);
        var dx = position.X - _start.X;
        var dy = position.Y - _start.Y;
        _start = position;

        foreach (var kvp in _transforms)
        {
            var element = kvp.Key;
            var transform = kvp.Value;

            var newX = transform.X + dx;
            var newY = transform.Y + dy;

            if (ConstrainToParentBounds && element.Parent is Control p)
            {
                var parentBounds = p.Bounds;
                var bounds = element.Bounds;

                var minX = -bounds.X;
                var minY = -bounds.Y;
                var maxX = parentBounds.Width - bounds.Width - bounds.X;
                var maxY = parentBounds.Height - bounds.Height - bounds.Y;

                newX = Math.Min(Math.Max(newX, minX), maxX);
                newY = Math.Min(Math.Max(newY, minY), maxY);
            }

            transform.X = newX;
            transform.Y = newY;
        }
    }

    private void AddTransform(Control? element)
    {
        if (element is null)
        {
            return;
        }

        if (element.RenderTransform is TranslateTransform tr)
        {
            _transforms[element] = tr;
        }
        else
        {
            var t = new TranslateTransform();
            element.RenderTransform = t;
            _transforms[element] = t;
        }
    }

    private void EndDrag()
    {
        _captured = false;
        _parent = null;
        _transforms.Clear();
    }
}
