// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Xaml.Interactivity;

/// <summary>
/// Emulates an Avalonia <see cref="RoutingStrategies.Direct"/> subscription on top of a WinUI routed event, which
/// always bubbles: the handler only receives the events raised on the element itself, not the ones that bubble up
/// from its descendants.
/// </summary>
internal abstract class DirectRouteFilter
{
    /// <summary>
    /// Creates the filter of a Direct subscription.
    /// </summary>
    /// <param name="element">The element the handler is added to.</param>
    /// <param name="routedEvent">The WinUI routed event, or <see langword="null"/> for the focus CLR events.</param>
    /// <returns>
    /// The filter, or <see langword="null"/> when Uno Platform already raises the event only on the element whose state
    /// changes: <c>PointerEntered</c> and <c>PointerExited</c> are raised once on every element the pointer enters or
    /// leaves (like the Avalonia direct events), with the element under the pointer as the original source.
    /// </returns>
    public static DirectRouteFilter? Create(UIElement element, RoutedEvent? routedEvent)
    {
        if (routedEvent == UIElement.PointerEnteredEvent || routedEvent == UIElement.PointerExitedEvent)
        {
            return null;
        }

        if (routedEvent == UIElement.PointerCaptureLostEvent)
        {
            return new PointerCaptureRouteFilter(element);
        }

        return new SourceRouteFilter(element);
    }

    /// <summary>
    /// Returns whether the event was raised on the element of the subscription.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    /// <returns><see langword="true"/> to invoke the handler.</returns>
    public abstract bool Accepts(RoutedEventArgs e);

    /// <summary>
    /// Releases the resources of the filter when the subscription is removed.
    /// </summary>
    public virtual void Detach()
    {
    }

    /// <summary>
    /// Accepts the events whose original source is the element (key, focus, tap and pointer events).
    /// </summary>
    private sealed class SourceRouteFilter(UIElement element) : DirectRouteFilter
    {
        public override bool Accepts(RoutedEventArgs e) => ReferenceEquals(e.OriginalSource, element);
    }

    /// <summary>
    /// Accepts the <c>PointerCaptureLost</c> events of the captures the element held.
    /// </summary>
    /// <remarks>
    /// Uno Platform raises <c>PointerCaptureLost</c> on the element that loses the capture, but its original source is
    /// the element last hit by the pointer (for example the text of a button). The filter tracks the pointers the
    /// element captured, as seen by the pointer events that reach it, to recognize the captures it loses.
    /// </remarks>
    private sealed class PointerCaptureRouteFilter : DirectRouteFilter
    {
        private readonly UIElement _element;
        private readonly PointerEventHandler _track;
        private readonly List<uint> _captured = [];

        public PointerCaptureRouteFilter(UIElement element)
        {
            _element = element;
            _track = Track;
            element.AddHandler(UIElement.PointerPressedEvent, _track, true);
            element.AddHandler(UIElement.PointerMovedEvent, _track, true);
            element.AddHandler(UIElement.PointerReleasedEvent, _track, true);
            element.AddHandler(UIElement.PointerCanceledEvent, _track, true);
        }

        public override bool Accepts(RoutedEventArgs e)
        {
            var held = e is PointerRoutedEventArgs { Pointer: { } pointer } && _captured.Remove(pointer.PointerId);
            return held || ReferenceEquals(e.OriginalSource, _element);
        }

        public override void Detach()
        {
            _element.RemoveHandler(UIElement.PointerPressedEvent, _track);
            _element.RemoveHandler(UIElement.PointerMovedEvent, _track);
            _element.RemoveHandler(UIElement.PointerReleasedEvent, _track);
            _element.RemoveHandler(UIElement.PointerCanceledEvent, _track);
            _captured.Clear();
        }

        private void Track(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer is not { } pointer)
            {
                return;
            }

            var id = pointer.PointerId;
            if (HasCapture(id))
            {
                if (!_captured.Contains(id))
                {
                    _captured.Add(id);
                }
            }
            else
            {
                _captured.Remove(id);
            }
        }

        private bool HasCapture(uint pointerId)
        {
            var captures = _element.PointerCaptures;
            if (captures is null)
            {
                return false;
            }

            for (var i = 0; i < captures.Count; i++)
            {
                if (captures[i].PointerId == pointerId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
