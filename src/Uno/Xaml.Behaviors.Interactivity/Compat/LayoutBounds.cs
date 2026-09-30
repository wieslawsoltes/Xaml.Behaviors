// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Xaml.Interactivity;

/// <summary>
/// Identifies the Avalonia <c>Visual.BoundsProperty</c> (WinUI has no bounds dependency property).
/// </summary>
internal sealed class LayoutBoundsProperty
{
    private LayoutBoundsProperty()
    {
    }

    /// <summary>Gets the singleton identifier.</summary>
    public static LayoutBoundsProperty Instance { get; } = new();
}

/// <summary>
/// Uno Platform counterparts of the Avalonia <c>Visual.Bounds</c> members used by the shared sources.
/// </summary>
/// <remarks>
/// Avalonia bounds are the arranged rectangle of an element relative to its parent (render transforms excluded).
/// WinUI exposes the same values as <see cref="UIElement.ActualOffset"/> and the actual size; changes are
/// observed through <see cref="FrameworkElement.SizeChanged"/> and <see cref="FrameworkElement.LayoutUpdated"/>.
/// </remarks>
internal static class LayoutBoundsExtensions
{
    extension(UIElement)
    {
        /// <summary>Gets the identifier of the bounds of an element.</summary>
        public static LayoutBoundsProperty BoundsProperty => LayoutBoundsProperty.Instance;
    }

    extension(UIElement element)
    {
        /// <summary>Gets the arranged bounds of the element relative to its parent.</summary>
        public Rect Bounds => GetBounds(element);
    }

    /// <summary>
    /// Gets the arranged bounds of an element relative to its parent.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="property">The bounds identifier.</param>
    /// <returns>The bounds.</returns>
    public static Rect GetValue(this UIElement element, LayoutBoundsProperty property)
    {
        _ = property;
        return GetBounds(element);
    }

    /// <summary>
    /// Gets an observable that produces the current bounds of an element and then every change.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="property">The bounds identifier.</param>
    /// <returns>The observable.</returns>
    public static IObservable<Rect> GetObservable(this UIElement element, LayoutBoundsProperty property)
    {
        _ = property;
        return new BoundsObservable(element);
    }

    private static Rect GetBounds(UIElement element)
    {
        var offset = element.ActualOffset;
        double width;
        double height;
        if (element is FrameworkElement frameworkElement)
        {
            width = frameworkElement.ActualWidth;
            height = frameworkElement.ActualHeight;
        }
        else
        {
            var size = element.ActualSize;
            width = size.X;
            height = size.Y;
        }

        return new Rect(offset.X, offset.Y, Math.Max(0, width), Math.Max(0, height));
    }

    private sealed class BoundsObservable(UIElement element) : IObservable<Rect>
    {
        public IDisposable Subscribe(IObserver<Rect> observer)
        {
            var last = GetBounds(element);
            observer.OnNext(last);

            if (element is not FrameworkElement frameworkElement)
            {
                return DisposableAction.Empty;
            }

            void Publish()
            {
                var current = GetBounds(element);
                if (current != last)
                {
                    last = current;
                    observer.OnNext(current);
                }
            }

            SizeChangedEventHandler sizeChanged = (_, _) => Publish();
            EventHandler<object> layoutUpdated = (_, _) => Publish();
            frameworkElement.SizeChanged += sizeChanged;
            frameworkElement.LayoutUpdated += layoutUpdated;

            return DisposableAction.Create(() =>
            {
                frameworkElement.SizeChanged -= sizeChanged;
                frameworkElement.LayoutUpdated -= layoutUpdated;
            });
        }
    }
}
